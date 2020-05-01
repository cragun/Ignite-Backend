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
                UpdateLatestStatus(inquiry.PropertyID, inquiry.Disposition, inquiry.DispositionTypeId);

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
                    var property = UpdateLatestStatus(entity.PropertyID, entity.Disposition, entity.DispositionTypeId);

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

        public InquiryStatisticsByDate GetWorkingRepsDisposition(IEnumerable<Guid> repIds, IEnumerable<string> dispositions, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay)
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

        public ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForOrganizationTerritories(ICollection<Guid> territoryIds, IEnumerable<OUReportingSettingsItem> reportItems, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay, IEnumerable<Guid> excludedReps = null)
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
                                SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value) : 0,
                                ThisQuarter = inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= dates.QuaterStart),
                                RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? inquiryDatesForIncludedDispositions.Count(id => id.DateCreated >= StartRangeDay && id.DateCreated < EndRangeDay) : 0,
                            },
                            People = new InquiryStatisticsByDate
                            {
                                AllTime = inquiryDatesForIncludedDispositions.GroupBy(id => id.PersonID).Count(),
                                ThisYear = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.PersonID).Count(),
                                ThisMonth = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.PersonID).Count(),
                                ThisWeek = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.PersonID).Count(),
                                Today = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.PersonID).Count(),
                                SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value).GroupBy(id => id.PersonID).Count() : 0,
                                ThisQuarter = inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= dates.QuaterStart).GroupBy(id => id.PersonID).Count(),
                                RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? inquiryDatesForIncludedDispositions.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay).GroupBy(id => id.PersonID).Count() : 0,
                            }
                        });
                    }




                    var CappPropertyids = dc.Inquiries.Where(x => x.Disposition == "ProposalPresented" && x.IsDeleted == false).Select(i => i.PropertyID);

                    var opprtunityDataList = dc.Appointments
                                    .Where(i => !excludedReps.Contains(i.AssigneeID.Value) &&
                                                territoryIds.Contains(i.Property.TerritoryID) &&
                                                i.IsDeleted == false && i.GoogleEventID != null && i.AssigneeID != null 
                                                && !CappPropertyids.Contains(i.PropertyID))
                                    .Select(i => new { Guid = i.Guid, AssigneeID = i.AssigneeID, StartDate = i.StartDate }).ToList();

                    inquiryStatistics.Add(new InquiryStatisticsForOrganization
                    {
                        Name = "OPP",
                        Actions = new InquiryStatisticsByDate
                        {
                            AllTime = opprtunityDataList.Count(),
                            ThisYear = opprtunityDataList.Count(id => id.StartDate >= dates.YearStart),
                            ThisMonth = opprtunityDataList.Count(id => id.StartDate >= dates.MonthStart),
                            ThisWeek = opprtunityDataList.Count(id => id.StartDate.Date >= dates.CurrentWeekStart),
                            Today = opprtunityDataList.Count(id => id.StartDate >= dates.TodayStart),
                            SpecifiedDay = specifiedDay.HasValue ? opprtunityDataList.Count(id => id.StartDate >= dates.SpecifiedStart.Value && id.StartDate < dates.SpecifiedEnd.Value) : 0,
                            ThisQuarter = opprtunityDataList.Count(id => id.StartDate >= dates.QuaterStart),
                            RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? opprtunityDataList.Count(id => id.StartDate >= StartRangeDay && id.StartDate <= EndRangeDay) : 0
                        },
                        People = new InquiryStatisticsByDate
                        {
                            AllTime = opprtunityDataList.GroupBy(id => id.AssigneeID).Count(),
                            ThisYear = opprtunityDataList.Where(id => id.StartDate >= dates.YearStart).GroupBy(id => id.AssigneeID).Count(),
                            ThisMonth = opprtunityDataList.Where(id => id.StartDate >= dates.MonthStart).GroupBy(id => id.AssigneeID).Count(),
                            ThisWeek = opprtunityDataList.Where(id => id.StartDate.Date >= dates.CurrentWeekStart).GroupBy(id => id.AssigneeID).Count(),
                            Today = opprtunityDataList.Where(id => id.StartDate >= dates.TodayStart).GroupBy(id => id.AssigneeID).Count(),
                            SpecifiedDay = specifiedDay.HasValue ? opprtunityDataList.Where(id => id.StartDate >= dates.SpecifiedStart.Value && id.StartDate < dates.SpecifiedEnd.Value).GroupBy(id => id.AssigneeID).Count() : 0,
                            ThisQuarter = opprtunityDataList.Where(id => id.StartDate >= dates.QuaterStart).GroupBy(id => id.AssigneeID).Count(),
                            RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? opprtunityDataList.Where(id => id.StartDate >= StartRangeDay && id.StartDate <= EndRangeDay).GroupBy(id => id.AssigneeID).Count() : 0
                        }
                    });

                    var ApptsCAPP = inquiryStatistics.Where(x => x.Name == "Appts CAPP").FirstOrDefault();
                    var OPP = inquiryStatistics.Where(x => x.Name == "OPP").FirstOrDefault();

                    if(ApptsCAPP != null && OPP != null)
                    {
                        inquiryStatistics.Add(new InquiryStatisticsForOrganization
                        {
                            Name = "CAPP(%)",
                            Actions = new InquiryStatisticsByDate
                            {
                                AllTime =  (ApptsCAPP.Actions.AllTime > 0 && OPP.Actions.AllTime > 0) ? (ApptsCAPP.Actions.AllTime * 100) / OPP.Actions.AllTime : 0 ,
                                ThisYear = (ApptsCAPP.Actions.ThisYear > 0 && OPP.Actions.ThisYear > 0) ? (ApptsCAPP.Actions.ThisYear * 100) / OPP.Actions.ThisYear : 0 ,
                                ThisMonth = (ApptsCAPP.Actions.ThisMonth > 0 && OPP.Actions.ThisMonth > 0) ? (ApptsCAPP.Actions.ThisMonth * 100) / OPP.Actions.ThisMonth : 0 ,
                                ThisWeek = (ApptsCAPP.Actions.ThisWeek > 0 && OPP.Actions.ThisWeek > 0) ? (ApptsCAPP.Actions.ThisWeek * 100) / OPP.Actions.ThisWeek : 0 ,
                                Today = (ApptsCAPP.Actions.Today > 0 && OPP.Actions.Today > 0) ? (ApptsCAPP.Actions.Today * 100) / OPP.Actions.Today : 0 ,
                                SpecifiedDay = (ApptsCAPP.Actions.SpecifiedDay > 0 && OPP.Actions.SpecifiedDay > 0) ? (ApptsCAPP.Actions.SpecifiedDay * 100) / OPP.Actions.SpecifiedDay : 0 ,
                                ThisQuarter = (ApptsCAPP.Actions.ThisQuarter > 0 && OPP.Actions.ThisQuarter > 0) ? (ApptsCAPP.Actions.ThisQuarter * 100) / OPP.Actions.ThisQuarter : 0 ,
                                RangeDay = (ApptsCAPP.Actions.RangeDay > 0 && OPP.Actions.RangeDay > 0) ? (ApptsCAPP.Actions.RangeDay * 100) / OPP.Actions.RangeDay : 0 
                            } ,
                            People = new InquiryStatisticsByDate
                            {
                                AllTime = (ApptsCAPP.Actions.AllTime > 0 && OPP.Actions.AllTime > 0) ? (ApptsCAPP.People.AllTime * 100) / OPP.People.AllTime  : 0 ,
                                ThisYear = (ApptsCAPP.Actions.ThisYear > 0 && OPP.Actions.ThisYear > 0) ? (ApptsCAPP.People.ThisYear * 100) / OPP.People.ThisYear : 0 ,
                                ThisMonth = (ApptsCAPP.Actions.ThisMonth > 0 && OPP.Actions.ThisMonth > 0) ? (ApptsCAPP.People.ThisMonth * 100) / OPP.People.ThisMonth : 0 ,
                                ThisWeek = (ApptsCAPP.Actions.ThisWeek > 0 && OPP.Actions.ThisWeek > 0) ? (ApptsCAPP.People.ThisWeek * 100) / OPP.People.ThisWeek : 0 ,
                                Today = (ApptsCAPP.Actions.Today > 0 && OPP.Actions.Today > 0) ? (ApptsCAPP.People.Today * 100) / OPP.People.Today : 0 ,
                                SpecifiedDay = (ApptsCAPP.Actions.SpecifiedDay > 0 && OPP.Actions.SpecifiedDay > 0) ? (ApptsCAPP.People.SpecifiedDay * 100) / OPP.People.SpecifiedDay : 0 ,
                                ThisQuarter = (ApptsCAPP.Actions.ThisQuarter > 0 && OPP.Actions.ThisQuarter > 0) ? (ApptsCAPP.People.ThisQuarter * 100) / OPP.People.ThisQuarter : 0 ,
                                RangeDay = (ApptsCAPP.Actions.RangeDay > 0 && OPP.Actions.RangeDay > 0) ? (ApptsCAPP.People.RangeDay * 100) / OPP.People.RangeDay : 0
                            }
                        });
                    }
                    

                }



                return inquiryStatistics;
            }
        }

        public ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForSalesPeopleTerritories(ICollection<Guid> territoryIds, IEnumerable<PersonReportingSettingsItem> reportItems, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay, IEnumerable<Guid> excludedReps = null)
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
                                                 i.User.IsDeleted == false &&
                                                searchDispositions.Contains(i.Disposition))
                                    .Select(i => new { Guid = i.Guid, PersonID = i.PersonID, DateCreated = i.DateCreated, Disposition = i.Disposition, OldDisposition = i.OldDisposition, PropertyID = i.PropertyID }).ToList();

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
                                    SpecifiedDay = specifiedDay.HasValue ? personInquiryDates.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).Count() : 0,
                                    ThisQuarter = personInquiryDates.Count(id => id.DateCreated >= dates.QuaterStart),
                                    RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? personInquiryDates.Count(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay) : 0
                                },
                                DaysActive = new InquiryStatisticsByDate
                                {
                                    AllTime = personInquiryDates.GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisYear = personInquiryDates.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisMonth = personInquiryDates.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisWeek = personInquiryDates.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    Today = personInquiryDates.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    SpecifiedDay = specifiedDay.HasValue ? personInquiryDates.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).GroupBy(id => id.DateCreated.Date).Count() : 0,
                                   ThisQuarter = personInquiryDates.Where(id => id.DateCreated >= dates.QuaterStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? personInquiryDates.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated < EndRangeDay).GroupBy(id => id.DateCreated.Date).Count() : 0
                                }
                            });


                            if (col.ColumnName == "OPP")
                            {
                                var CappPropertyids = inquiryDates.Where(x => x.Disposition == "ProposalPresented" && x.PersonID == personId).Select(i => i.PropertyID);

                                var opprtunityDataList = dc.Appointments.Where(i => i.IsDeleted == false && i.GoogleEventID != null &&
                                                           i.AssigneeID != null && !CappPropertyids.Contains(i.PropertyID))
                                                .Select(i => new { Guid = i.Guid, AssigneeID = i.AssigneeID, StartDate = i.StartDate }).ToList();

                                if (opprtunityDataList != null)
                                {
                                    inquiryStatistics.Add(new InquiryStatisticsForPerson
                                    {
                                        PersonId = personId,
                                        Name = col.ColumnName,
                                        Actions = new InquiryStatisticsByDate
                                        {
                                            AllTime = opprtunityDataList.Count(),
                                            ThisYear = opprtunityDataList.Count(id => id.StartDate >= dates.YearStart),
                                            ThisMonth = opprtunityDataList.Count(id => id.StartDate >= dates.MonthStart),
                                            ThisWeek = opprtunityDataList.Count(id => id.StartDate.Date >= dates.CurrentWeekStart),
                                            Today = opprtunityDataList.Count(id => id.StartDate >= dates.TodayStart),
                                            SpecifiedDay = specifiedDay.HasValue ? opprtunityDataList.Count(id => id.StartDate >= dates.SpecifiedStart.Value && id.StartDate < dates.SpecifiedEnd.Value) : 0,
                                            ThisQuarter = opprtunityDataList.Count(id => id.StartDate >= dates.QuaterStart),
                                            RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? opprtunityDataList.Count(id => id.StartDate >= StartRangeDay && id.StartDate <= EndRangeDay) : 0
                                        },
                                        DaysActive = new InquiryStatisticsByDate
                                        {
                                            AllTime = opprtunityDataList.GroupBy(id => id.StartDate.Date).Count(),
                                            ThisYear = opprtunityDataList.Where(id => id.StartDate >= dates.YearStart).GroupBy(id => id.StartDate.Date).Count(),
                                            ThisMonth = opprtunityDataList.Where(id => id.StartDate >= dates.MonthStart).GroupBy(id => id.StartDate.Date).Count(),
                                            ThisWeek = opprtunityDataList.Where(id => id.StartDate.Date >= dates.CurrentWeekStart).GroupBy(id => id.StartDate.Date).Count(),
                                            Today = opprtunityDataList.Where(id => id.StartDate >= dates.TodayStart).GroupBy(id => id.StartDate.Date).Count(),
                                            SpecifiedDay = specifiedDay.HasValue ? opprtunityDataList.Where(id => id.StartDate >= dates.SpecifiedStart.Value && id.StartDate < dates.SpecifiedEnd.Value).GroupBy(id => id.StartDate.Date).Count() : 0,
                                            ThisQuarter = opprtunityDataList.Where(id => id.StartDate >= dates.QuaterStart).GroupBy(id => id.StartDate.Date).Count(),
                                            RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? opprtunityDataList.Where(id => id.StartDate >= StartRangeDay && id.StartDate <= EndRangeDay).GroupBy(id => id.StartDate.Date).Count() : 0
                                        }
                                    });
                                }
                            }


                            if (col.ColumnName == "ClockHours")
                            {
                                var personTotalHour = dc.PersonClockTime.Where(id => id.PersonID == personId).Select(i => new { PersonID = i.PersonID, DateCreated = i.DateCreated, ClockMin = i.ClockMin }).ToList();

                                if (personTotalHour != null)
                                {
                                    inquiryStatistics.Add(new InquiryStatisticsForPerson
                                    {
                                        PersonId = personId,
                                        Name = col.ColumnName,
                                        Actions = new InquiryStatisticsByDate
                                        {
                                            AllTime = Convert.ToInt64(Math.Round(personTotalHour.Sum(x => x.ClockMin) / (double)60)),
                                            ThisYear = Convert.ToInt64(Math.Round(personTotalHour.Where(id => id.DateCreated >= dates.YearStart).Sum(x => x.ClockMin) / (double)60)),
                                            ThisMonth = Convert.ToInt64(Math.Round(personTotalHour.Where(id => id.DateCreated >= dates.MonthStart).Sum(x => x.ClockMin) / (double)60)),
                                            ThisWeek = Convert.ToInt64(Math.Round(personTotalHour.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).Sum(x => x.ClockMin) / (double)60)),
                                            Today = Convert.ToInt64(Math.Round(personTotalHour.Where(id => id.DateCreated >= dates.TodayStart).Sum(x => x.ClockMin) / (double)60)),
                                            SpecifiedDay = Convert.ToInt64(Math.Round(specifiedDay.HasValue ? personTotalHour.Where(id => id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value).Sum(x => x.ClockMin) : 0 / (double)60)),
                                            ThisQuarter = Convert.ToInt64(Math.Round(personTotalHour.Where(id => id.DateCreated >= dates.QuaterStart).Sum(x => x.ClockMin) / (double)60)),
                                            RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? Convert.ToInt64(Math.Round(personTotalHour.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay).Sum(x => x.ClockMin) / (double)60)) : 0
                                        },
                                        DaysActive = new InquiryStatisticsByDate
                                        { }

                                    });
                                }
                            }

                            if (col.ColumnName == "CAPP(%)")
                            {
                                var ApptsCAPP = inquiryStatistics.Where(x => x.Name == "Appts CAPP").FirstOrDefault();
                                var OPP = inquiryStatistics.Where(x => x.Name == "OPP").FirstOrDefault();

                                if (ApptsCAPP != null && OPP != null)
                                {
                                    inquiryStatistics.Add(new InquiryStatisticsForPerson
                                    {
                                        PersonId = personId,
                                        Name = col.ColumnName,
                                        Actions = new InquiryStatisticsByDate
                                        {
                                            AllTime = (ApptsCAPP.Actions.AllTime > 0 && OPP.Actions.AllTime > 0) ? (ApptsCAPP.Actions.AllTime * 100) / OPP.Actions.AllTime : 0,
                                            ThisYear = (ApptsCAPP.Actions.ThisYear > 0 && OPP.Actions.ThisYear > 0) ? (ApptsCAPP.Actions.ThisYear * 100) / OPP.Actions.ThisYear : 0,
                                            ThisMonth = (ApptsCAPP.Actions.ThisMonth > 0 && OPP.Actions.ThisMonth > 0) ? (ApptsCAPP.Actions.ThisMonth * 100) / OPP.Actions.ThisMonth : 0,
                                            ThisWeek = (ApptsCAPP.Actions.ThisWeek > 0 && OPP.Actions.ThisWeek > 0) ? (ApptsCAPP.Actions.ThisWeek * 100) / OPP.Actions.ThisWeek : 0,
                                            Today = (ApptsCAPP.Actions.Today > 0 && OPP.Actions.Today > 0) ? (ApptsCAPP.Actions.Today * 100) / OPP.Actions.Today : 0,
                                            SpecifiedDay = (ApptsCAPP.Actions.SpecifiedDay > 0 && OPP.Actions.SpecifiedDay > 0) ? (ApptsCAPP.Actions.SpecifiedDay * 100) / OPP.Actions.SpecifiedDay : 0,
                                            ThisQuarter = (ApptsCAPP.Actions.ThisQuarter > 0 && OPP.Actions.ThisQuarter > 0) ? (ApptsCAPP.Actions.ThisQuarter * 100) / OPP.Actions.ThisQuarter : 0,
                                            RangeDay = (ApptsCAPP.Actions.RangeDay > 0 && OPP.Actions.RangeDay > 0) ? (ApptsCAPP.Actions.RangeDay * 100) / OPP.Actions.RangeDay : 0
                                        },
                                        DaysActive = new InquiryStatisticsByDate
                                        {
                                            AllTime = (ApptsCAPP.Actions.AllTime > 0 && OPP.Actions.AllTime > 0) ? (ApptsCAPP.DaysActive.AllTime * 100) / OPP.DaysActive.AllTime : 0,
                                            ThisYear = (ApptsCAPP.Actions.ThisYear > 0 && OPP.Actions.ThisYear > 0) ? (ApptsCAPP.DaysActive.ThisYear * 100) / OPP.DaysActive.ThisYear : 0,
                                            ThisMonth = (ApptsCAPP.Actions.ThisMonth > 0 && OPP.Actions.ThisMonth > 0) ? (ApptsCAPP.DaysActive.ThisMonth * 100) / OPP.DaysActive.ThisMonth : 0,
                                            ThisWeek = (ApptsCAPP.Actions.ThisWeek > 0 && OPP.Actions.ThisWeek > 0) ? (ApptsCAPP.DaysActive.ThisWeek * 100) / OPP.DaysActive.ThisWeek : 0,
                                            Today = (ApptsCAPP.Actions.Today > 0 && OPP.Actions.Today > 0) ? (ApptsCAPP.DaysActive.Today * 100) / OPP.DaysActive.Today : 0,
                                            SpecifiedDay = (ApptsCAPP.Actions.SpecifiedDay > 0 && OPP.Actions.SpecifiedDay > 0) ? (ApptsCAPP.DaysActive.SpecifiedDay * 100) / OPP.DaysActive.SpecifiedDay : 0,
                                            ThisQuarter = (ApptsCAPP.Actions.ThisQuarter > 0 && OPP.Actions.ThisQuarter > 0) ? (ApptsCAPP.DaysActive.ThisQuarter * 100) / OPP.DaysActive.ThisQuarter : 0,
                                            RangeDay = (ApptsCAPP.Actions.RangeDay > 0 && OPP.Actions.RangeDay > 0) ? (ApptsCAPP.DaysActive.RangeDay * 100) / OPP.DaysActive.RangeDay : 0
                                        }
                                    });
                                }
                            }
                        }
                    }


                }

                return inquiryStatistics;
            }
        }

        public ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForPerson(Guid personId, ICollection<string> dispositions, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay)
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
                                    SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).Count() : 0,
                                    ThisQuarter = inquiryDatesForDisposition.Count(id => id.DateCreated >= dates.QuaterStart),
                                    RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? inquiryDatesForDisposition.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay).Count() : 0
                                },
                                DaysActive = new InquiryStatisticsByDate
                                {
                                    AllTime = inquiryDatesForDisposition.GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisYear = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisMonth = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisWeek = inquiryDatesForDisposition.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    Today = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    SpecifiedDay = specifiedDay.HasValue ? inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).GroupBy(id => id.DateCreated.Date).Count() : 0,
                                    ThisQuarter = inquiryDatesForDisposition.Where(id => id.DateCreated >= dates.QuaterStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? inquiryDatesForDisposition.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay).GroupBy(id => id.DateCreated.Date).Count() : 0,
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

        private Property UpdateLatestStatus(Guid propertyId, string newDisposition, int? newDispositionTypeId)
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
                    UpdatePersonClockTime(property.Territory.OUID);
                    property.DispositionTypeId = newDispositionTypeId;
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

        public class PersonClock
        {
            public long ClockTimeInMin { get; set; }

            public bool IsEnabled { get; set; }
        }


        public void UpdatePersonClockTime(Guid propertyid)
        {

            var PersonClockSetting = _ouSettingService.Value.GetOUSettingForPropertyID<PersonClock>(propertyid, OUSetting.LegionOUPersonClockInfo);

            using (DataContext dc = new DataContext())
            {
                var personClockTime = dc.PersonClockTime.Where(p => p.PersonID == SmartPrincipal.UserId).ToList().Where(p => p.DateCreated.Date == DateTime.Now.Date)
                    .FirstOrDefault();

                if (personClockTime != null && PersonClockSetting.IsEnabled == true)
                {
                    if(personClockTime.ClockType == "ClockIn")
                    {
                        TimeSpan timespan = DateTime.Now - personClockTime.StartDate.Value;
                        long diffMin = (long)Math.Floor(timespan.TotalMinutes);
                        personClockTime.ClockMin = personClockTime.ClockMin + diffMin;
                        TimeSpan spWorkMin = TimeSpan.FromMinutes(personClockTime.ClockMin);
                        personClockTime.TagString = string.Format("{0:00}:{1:00}", (int)spWorkMin.TotalHours, spWorkMin.Minutes);
                        personClockTime.ClockHours = Convert.ToInt64(Math.Round(personClockTime.ClockMin / (double)60));
                    }
                    personClockTime.ClockDiff = 0;
                    personClockTime.StartDate = DateTime.Now;
                    personClockTime.EndDate = (DateTime.Now).AddMinutes(PersonClockSetting.ClockTimeInMin);
                    personClockTime.ClockType = "ClockIn";
                    personClockTime.Version += 1;
                    personClockTime.DateLastModified = DateTime.Now;
                    dc.SaveChanges();
                }
                else
                {
                    PersonClockTime personClock = new PersonClockTime();
                    personClock.Guid = Guid.NewGuid();
                    personClock.PersonID = SmartPrincipal.UserId;
                    personClock.DateCreated = DateTime.Now;
                    personClock.StartDate = DateTime.Now;
                    personClock.EndDate = (DateTime.Now).AddMinutes(PersonClockSetting.ClockTimeInMin);
                    personClock.ClockDiff = 0;
                    personClock.ClockMin = 0;
                    personClock.ClockHours = 0;
                    personClock.ClockType = "ClockIn";
                    personClock.CreatedByID = SmartPrincipal.UserId;
                    dc.PersonClockTime.Add(personClock);
                    dc.SaveChanges();
                }
            }

        }
    }
    

    internal class StatisticDates
    {
        internal DateTime CurrentWeekStart { get; set; }
        internal DateTime YearStart { get; set; }
        internal DateTime MonthStart { get; set; }
        internal DateTime TodayStart { get; set; }
        internal DateTime QuaterStart { get; set; }
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
           
            int quarterNumber = (deviceDate.Month - 1) / 3 + 1;
            QuaterStart = new DateTime(deviceDate.Year, (quarterNumber - 1) * 3 + 1, 1).Add(offset); 
            //DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
        }
    }
}
