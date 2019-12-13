using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations.Microsoft;
using DataReef.Integrations.Microsoft.PowerBI.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.PubSubMessaging;
using DataReef.TM.Models.Reporting.Settings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class InquiryService : DataService<Inquiry>, IInquiryService
    {
        private readonly IPersonService _personService;
        private readonly IPropertyService _propertyService;
        private readonly Lazy<IPowerBIBridge> _pbiBridge;
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IOUSettingService> _ouSettingService;

        public InquiryService(ILogger logger,
            IPersonService personService,
            IPropertyService propertyService,
            Lazy<IPowerBIBridge> pbiBridge,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IOUService> ouService,
            Lazy<IOUSettingService> ouSettingService)
            : base(logger, unitOfWorkFactory)
        {
            _personService = personService;
            _propertyService = propertyService;
            _ouService = ouService;
            _pbiBridge = pbiBridge;
            _ouSettingService = ouSettingService;
        }

        public override Inquiry Update(Inquiry entity)
        {
            var inquiry = base.Update(entity);

            if (inquiry.SaveResult.Success)
            {
                UpdateLatestStatus(inquiry.PropertyID, inquiry.Disposition);

                var pbi = new PBI_DispositionChanged
                {
                    InquiryID = entity.Guid,
                    Disposition = entity.Disposition.ToString(),
                    OUID = entity.OUID,
                    PropertyID = entity.PropertyID,
                    UserID = SmartPrincipal.UserId,
                    IsLead = entity.IsLead ? "yes" : "no"
                };
                _pbiBridge.Value.PushDataAsync(pbi);
                // base.ProcessApiWebHooks(entity, EventDomain.Disposition, EventAction.Changed, property.Guid);
            }
            return inquiry;
        }

        public override Inquiry Insert(Inquiry entity)
        {
            var ret = base.Insert(entity);
            if (ret == null)
            {
                entity.SaveResult = SaveResult.SuccessfulInsert;
                return entity;
            }

            if (ret.SaveResult.Success)
            {
                var ouid = ret.OUID;
                if (!ouid.HasValue)
                {
                    using (DataContext dc = new DataContext())
                    {
                        ouid = dc
                                    .Properties
                                    .Include(p => p.Territory)
                                    .FirstOrDefault(p => p.Guid == ret.PropertyID)?
                                    .Territory?
                                    .OUID;
                    }
                }

                if (!entity.IsNew.HasValue)
                {
                    _ouService.Value.ProcessEvent(new EventMessage
                    {
                        EventSource = "Inquiry",
                        EventAction = EventActionType.Insert,
                        EventEntity = ret,
                        OUID = ouid,
                        EventEntityGuid = ret.Guid
                    });
                }

                var pbi = new PBI_DispositionChanged
                {
                    InquiryID = entity.Guid,
                    Disposition = entity.Disposition.ToString(),
                    OUID = entity.OUID,
                    PropertyID = entity.PropertyID,
                    UserID = SmartPrincipal.UserId,
                    IsLead = entity.IsLead ? "yes" : "no"
                };
                _pbiBridge.Value.PushDataAsync(pbi);

                try
                {
                    var property = UpdateLatestStatus(entity.PropertyID, entity.Disposition);

                    base.ProcessApiWebHooks(property.Guid, ApiObjectType.Customer, EventDomain.Disposition, EventAction.Created, property.Guid);
                }
                catch (Exception)
                {
                }
            }

            return ret;
        }

        public override ICollection<Inquiry> InsertMany(ICollection<Inquiry> entities)
        {
            var existIDs = Exist(entities);
            var newEntities = entities?
                            .Where(e => !existIDs.Contains(e.Guid))?
                            .ToList();

            return base.InsertMany(newEntities);
        }

        public bool IsInquiryFirstForUser(Guid inquiryId, Guid userId)
        {
            using(var dc = new DataContext())
            {
                var inquiry = dc
                    .Inquiries
                    .FirstOrDefault(i => i.Guid == inquiryId);
                

                if (inquiry != null && inquiry.OUID.HasValue)
                {
                    var ouSettings = _ouSettingService.Value.GetSettingsByOUID(inquiry.OUID.Value);
                    var exclusionSetting = ouSettings?.FirstOrDefault(o => o.Name == "Inquiries.Onboarding.Exclusion.OUs");

                    if (exclusionSetting != null)
                    {
                        var excludedOUIds = exclusionSetting.GetValue<List<Guid>>();
                        var ancestors = _ouService.Value.GetAncestorsForOU(inquiry.OUID.Value);

                        if(excludedOUIds?.Any() == true && ancestors?.Any() == true)
                        {
                            ancestors.Add(new GuidNamePair { Guid = inquiry.OUID.Value, Name = string.Empty});
                            if(excludedOUIds.Intersect(ancestors.Select(x => x.Guid))?.Count() != 0)
                            {
                                //at least one of the OU ancestors could be found in the exlusion list. return false
                                return false;
                            }
                        }
                    }
                }

                //if the inquiry exists and is the only one attached to the person return true, otherwise return false
                if(dc.Inquiries.Any(i => i.Guid == inquiryId && i.PersonID == userId))
                {
                    return dc.Inquiries.Count(i => i.Guid != inquiryId && i.PersonID == userId) == 0;
                }

                return false;
            }
        }

        public InquiryStatisticsByDate GetWorkingRepsDisposition(IEnumerable<Guid> repIds, IEnumerable<string> dispositions, DateTime? specifiedDay)
        {
            if(repIds?.Any() != true || dispositions?.Any() != true)
            {
                return new InquiryStatisticsByDate();
            }
            var dates = new StatisticDates(specifiedDay);
            using (DataContext dc = new DataContext())
            {
                var inquiryDates = dc.Inquiries
                                    .Where(i => i.IsDeleted == false &&
                                                repIds.Contains(i.PersonID) &&
                                                dispositions.Contains(i.Disposition))
                                    .Select(i => new { PersonID = i.PersonID, DateCreated = i.DateCreated, Disposition = i.Disposition }).ToList();

                
                return new InquiryStatisticsByDate
                {
                    AllTime = repIds.Count(x => inquiryDates.Any(i => i.PersonID == x)),
                    ThisYear = repIds.Count(x => inquiryDates.Any(id => id.PersonID == x && id.DateCreated >= dates.YearStart)),
                    ThisMonth = repIds.Count(x => inquiryDates.Any(id => id.PersonID == x && id.DateCreated >= dates.MonthStart)),
                    ThisWeek = repIds.Count(x => inquiryDates.Any(id => id.PersonID == x && id.DateCreated >= dates.CurrentWeekStart)),
                    Today = repIds.Count(x => inquiryDates.Any(id => id.PersonID == x && id.DateCreated >= dates.TodayStart)),
                    SpecifiedDay = specifiedDay.HasValue ? repIds.Count(x => inquiryDates.Any(id => id.PersonID == x && id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value)) : 0
                };
            }
                
        }

        public ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForOrganizationTerritories(ICollection<Guid> territoryIds, IEnumerable<OUReportingSettingsItem> reportItems, DateTime? specifiedDay, IEnumerable<Guid> excludedReps = null)
        {
            var inquiryStatistics = new List<InquiryStatisticsForOrganization>();

            using (DataContext dc = new DataContext())
            {
                var dates = new StatisticDates(specifiedDay);
                excludedReps = excludedReps ?? new List<Guid>();

                if (reportItems?.Any() == true)
                {
                    //dispositions to include when making the call to the db
                    var searchDispositions = new HashSet<string>();
                    foreach (var repItem in reportItems)
                    {
                        if(repItem.IncludedDispositions?.Any() == true)
                        {
                            searchDispositions.UnionWith(repItem.IncludedDispositions);
                        }

                        if(repItem.ConditionalIncludedDispositions?.Any() == true)
                        {
                            searchDispositions.UnionWith(
                                repItem
                                .ConditionalIncludedDispositions
                                .Where(x => !string.IsNullOrEmpty(x.FinalDisposition))
                                .Select(x => x.FinalDisposition));
                        }

                    }

                    var inquiryDates = dc.Inquiries
                                    .Where(i => !excludedReps.Contains(i.PersonID) &&
                                                territoryIds.Contains(i.Property.TerritoryID) &&
                                                i.IsDeleted == false &&
                                                searchDispositions.Contains(i.Disposition))
                                    .Select(i => new { Guid = i.Guid, PersonID = i.PersonID, DateCreated = i.DateCreated, Disposition = i.Disposition, OldDisposition = i.OldDisposition }).ToList();


                    foreach (var col in reportItems)
                    {
                        var inquiryDatesForIncludedDispositions = inquiryDates.Where(id => col.IncludedDispositions.Contains(id.Disposition)).ToList();

                        //add conditional dispositions if they are configured
                        if(col.ConditionalIncludedDispositions?.Any() == true)
                        {
                            foreach(var disp in col.ConditionalIncludedDispositions)
                            {
                                var conditionalInquiryDates = inquiryDates.Where(x =>
                                {
                                    if (x.Disposition.Equals(disp.FinalDisposition, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (string.IsNullOrEmpty(x.OldDisposition))
                                        {
                                            return disp.InitialDispositions.Contains(string.Empty);
                                        }

                                        return disp.InitialDispositions.Contains(x.OldDisposition);
                                    }
                                    return false;
                                });

                                inquiryDatesForIncludedDispositions.AddRange(conditionalInquiryDates);
                                inquiryDatesForIncludedDispositions = inquiryDatesForIncludedDispositions.DistinctBy(x => x.Guid).ToList();
                            }
                        }

                        inquiryStatistics.Add(new InquiryStatisticsForOrganization
                        {
                            Name = col.ColumnName,
                            Actions = new InquiryStatisticsByDate
                            {
                                AllTime = inquiryDatesForIncludedDispositions.Count(),
                                ThisYear = inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= dates.YearStart),
                                ThisMonth = inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= dates.MonthStart),
                                ThisWeek = inquiryDatesForIncludedDispositions.Count(id => id.DateCreated.Date >= dates.CurrentWeekStart),
                                Today = inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= dates.TodayStart),
                                SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value) : 0
                            },
                            People = new InquiryStatisticsByDate
                            {
                                AllTime = inquiryDatesForIncludedDispositions.GroupBy(id => id.PersonID).Count(),
                                ThisYear = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.PersonID).Count(),
                                ThisMonth = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.PersonID).Count(),
                                ThisWeek = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.PersonID).Count(),
                                Today = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.PersonID).Count(),
                                SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value).GroupBy(id => id.PersonID).Count() : 0
                            }
                        });
                    }
                }



                return inquiryStatistics;
            }
        }

        public ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForSalesPeopleTerritories(ICollection<Guid> territoryIds, IEnumerable<PersonReportingSettingsItem> reportItems, DateTime? specifiedDay, IEnumerable<Guid> excludedReps = null)
        {
            var inquiryStatistics = new List<InquiryStatisticsForPerson>();

            using (DataContext dc = new DataContext())
            {
                var dates = new StatisticDates(specifiedDay);
                excludedReps = excludedReps ?? new List<Guid>();

                if (reportItems?.Any() == true)
                {
                    //dispositions to include when making the call to the db
                    var searchDispositions = new HashSet<string>();
                    foreach (var repItem in reportItems)
                    {
                        if(repItem.IncludedDispositions?.Any() == true)
                        {
                            searchDispositions.UnionWith(repItem.IncludedDispositions);
                        }

                        if (repItem.ConditionalIncludedDispositions?.Any() == true)
                        {
                            searchDispositions.UnionWith(
                                repItem
                                .ConditionalIncludedDispositions
                                .Where(x => !string.IsNullOrEmpty(x.FinalDisposition))
                                .Select(x => x.FinalDisposition));
                        }
                    }

                    var inquiryDates = dc.Inquiries
                                    .Where(i => !excludedReps.Contains(i.PersonID) &&
                                                territoryIds.Contains(i.Property.TerritoryID) &&
                                                i.IsDeleted == false &&
                                                searchDispositions.Contains(i.Disposition))
                                    .Select(i => new { Guid = i.Guid, PersonID = i.PersonID, DateCreated = i.DateCreated, Disposition = i.Disposition, OldDisposition = i.OldDisposition }).ToList();

                    var peopleIds = inquiryDates.Select(p => p.PersonID).Distinct();

                    
                    foreach(var col in reportItems)
                    {
                        if(col.IncludedDispositions?.Any() != true && col.ConditionalIncludedDispositions?.Any() != true)
                        {
                            continue;
                        }
                        var inquiryDatesForDisposition = inquiryDates.Where(id => col.IncludedDispositions?.Contains(id.Disposition) == true)?.ToList();
                        
                        //add conditional dispositions if they are configured
                        if (col.ConditionalIncludedDispositions?.Any() == true)
                        {
                            foreach (var disp in col.ConditionalIncludedDispositions)
                            {
                                var conditionalInquiryDates = inquiryDates.Where(x =>
                                {
                                    if (x.Disposition.Equals(disp.FinalDisposition, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (string.IsNullOrEmpty(x.OldDisposition))
                                        {
                                            return disp.InitialDispositions.Contains(string.Empty);
                                        }

                                        return disp.InitialDispositions.Contains(x.OldDisposition);
                                    }
                                    return false;
                                });

                                inquiryDatesForDisposition.AddRange(conditionalInquiryDates);
                                inquiryDatesForDisposition = inquiryDatesForDisposition.DistinctBy(x => x.Guid).ToList();
                            }
                        }


                        foreach (var personId in peopleIds)
                        {
                            
                            var personInquiryDates = inquiryDatesForDisposition.Where(id => id.PersonID == personId).ToList();

                            inquiryStatistics.Add(new InquiryStatisticsForPerson
                            {
                                PersonId = personId,
                                Name = col.ColumnName,
                                Actions = new InquiryStatisticsByDate
                                {
                                    AllTime = personInquiryDates.Count(),
                                    ThisYear = personInquiryDates.Count(id => id.DateCreated >= dates.YearStart),
                                    ThisMonth = personInquiryDates.Count(id => id.DateCreated >= dates.MonthStart),
                                    ThisWeek = personInquiryDates.Count(id => id.DateCreated.Date >= dates.CurrentWeekStart),
                                    Today = personInquiryDates.Count(id => id.DateCreated >= dates.TodayStart),
                                    SpecifiedDay = specifiedDay.HasValue ? personInquiryDates.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).Count() : 0
                                },
                                DaysActive = new InquiryStatisticsByDate
                                {
                                    AllTime = personInquiryDates.GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisYear = personInquiryDates.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisMonth = personInquiryDates.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisWeek = personInquiryDates.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    Today = personInquiryDates.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    SpecifiedDay = specifiedDay.HasValue ? personInquiryDates.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).GroupBy(id => id.DateCreated.Date).Count() : 0
                                }
                            });
                        }
                    }
                }

                return inquiryStatistics;
            }
        }

        public ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForPerson(Guid personId, ICollection<string> dispositions, DateTime? specifiedDay)
        {
            var inquiryStatistics = new List<InquiryStatisticsForPerson>();

            if(dispositions?.Any() != true)
            {
                var rootOU = _ouService
                                    .Value
                                    .ListRootsForPerson(personId, string.Empty, string.Empty, string.Empty)
                                    .FirstOrDefault();
                if (rootOU != null)
                {
                    var reportSettings =
                            _ouSettingService
                            .Value
                            .GetSettingsByOUID(rootOU.Guid)
                            ?.FirstOrDefault(s => s.Name == OUSetting.OU_Reporting_Settings)
                            ?.GetValue<OUReportingSettings>();

                    if(reportSettings != null && reportSettings.PersonReportItems?.Any() == true)
                    {
                        var searchDispositions = new HashSet<string>();
                        foreach (var repItem in reportSettings.PersonReportItems)
                        {
                            if (repItem.IncludedDispositions?.Any() == true)
                            {
                                searchDispositions.UnionWith(repItem.IncludedDispositions);
                            }

                            if (repItem.ConditionalIncludedDispositions?.Any() == true)
                            {
                                searchDispositions.UnionWith(
                                    repItem
                                    .ConditionalIncludedDispositions
                                    .Where(x => !string.IsNullOrEmpty(x.FinalDisposition))
                                    .Select(x => x.FinalDisposition));
                            }
                        }
                        dispositions = searchDispositions;
                    }

                    using (DataContext dc = new DataContext())
                    {
                        var dates = new StatisticDates(specifiedDay);

                        var inquiryDates = dc.Inquiries
                                            .Where(i => i.PersonID == personId &&
                                                        i.IsDeleted == false &&
                                                        dispositions.Contains(i.Disposition)
                                                  )
                                            .Select(i => new { Guid = i.Guid, PersonID = i.PersonID, DateCreated = i.DateCreated, Disposition = i.Disposition, OldDisposition = i.OldDisposition }).ToList();

                        foreach (var col in reportSettings.PersonReportItems)
                        {
                            var inquiryDatesForDisposition = inquiryDates.Where(id => col.IncludedDispositions?.Contains(id.Disposition) == true)?.ToList();

                            //add conditional dispositions if they are configured
                            if (col.ConditionalIncludedDispositions?.Any() == true)
                            {
                                foreach (var disp in col.ConditionalIncludedDispositions)
                                {
                                    var conditionalInquiryDates = inquiryDates.Where(x =>
                                    {
                                        if (x.Disposition.Equals(disp.FinalDisposition, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            if (string.IsNullOrEmpty(x.OldDisposition))
                                            {
                                                return disp.InitialDispositions.Contains(string.Empty);
                                            }

                                            return disp.InitialDispositions.Contains(x.OldDisposition);
                                        }
                                        return false;
                                    });

                                    inquiryDatesForDisposition.AddRange(conditionalInquiryDates);
                                    inquiryDatesForDisposition = inquiryDatesForDisposition.DistinctBy(x => x.Guid).ToList();
                                }
                            }

                            inquiryStatistics.Add(new InquiryStatisticsForPerson
                            {
                                PersonId = personId,
                                Name = col.ColumnName,
                                Actions = new InquiryStatisticsByDate
                                {
                                    AllTime = inquiryDatesForDisposition.Count(),
                                    ThisYear = inquiryDatesForDisposition.Count(id => id.DateCreated >= dates.YearStart),
                                    ThisMonth = inquiryDatesForDisposition.Count(id => id.DateCreated >= dates.MonthStart),
                                    ThisWeek = inquiryDatesForDisposition.Count(id => id.DateCreated.Date >= dates.CurrentWeekStart),
                                    Today = inquiryDatesForDisposition.Count(id => id.DateCreated >= dates.TodayStart),
                                    SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).Count() : 0
                                },
                                DaysActive = new InquiryStatisticsByDate
                                {
                                    AllTime = inquiryDatesForDisposition.GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisYear = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisMonth = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisWeek = inquiryDatesForDisposition.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    Today = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).GroupBy(id => id.DateCreated.Date).Count() : 0
                                }
                            });
                        }
                    }

                }
            }

            

            return inquiryStatistics;
        }

        public string GetLatestPropertyDisposition(Guid propertyID, Guid? skipInquiryId = null)
        {
            using(var dc = new DataContext())
            {
                var latestInquiry = 
                    dc
                    .Inquiries
                    .Where(i => i.PropertyID == propertyID && i.Guid != skipInquiryId)
                    .OrderByDescending(i => i.DateCreated)
                    .FirstOrDefault();

                return latestInquiry?.Disposition;
            }
        }

        private Property UpdateLatestStatus(Guid propertyId, string newDisposition)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var property = uow.Get<Property>().Include(p => p.Territory).FirstOrDefault(p => p.Guid == propertyId);
                if (property == null)
                {
                    return null;
                }

                bool territoryNeedsSave = false;

                if (property.LatestDisposition != newDisposition)
                {
                    property.LatestDisposition = newDisposition;
                    territoryNeedsSave = true;
                }

                if (territoryNeedsSave && property.Territory != null)
                {
                    property.Territory.Updated(SmartPrincipal.UserId);
                }
                uow.SaveChanges();
                return property;
            }
        }
    }

    internal class StatisticDates
    {
        internal DateTime CurrentWeekStart { get; set; }
        internal DateTime YearStart { get; set; }
        internal DateTime MonthStart { get; set; }
        internal DateTime TodayStart { get; set; }
        internal DateTime? SpecifiedStart { get; set; }
        internal DateTime? SpecifiedEnd { get; set; }
        internal bool HasSpecifiedDay { get; set; }

        internal StatisticDates(DateTime? specifiedDay)
        {
            var deviceDate = SmartPrincipal.DeviceDate.Date;
            var offset = -SmartPrincipal.DeviceDate.Offset;

            if (HasSpecifiedDay = specifiedDay.HasValue)
            {
                SpecifiedStart = specifiedDay.Value.Add(offset);
                SpecifiedEnd = SpecifiedStart.Value.AddDays(1);
            }

            CurrentWeekStart = deviceDate.AddDays(-(deviceDate.DayOfWeek - Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek)).Add(offset);

            YearStart = new DateTime(deviceDate.Year, 1, 1).Add(offset);
            MonthStart = new DateTime(deviceDate.Year, deviceDate.Month, 1).Add(offset);

            TodayStart = deviceDate.Add(offset);
        }
    }
}
