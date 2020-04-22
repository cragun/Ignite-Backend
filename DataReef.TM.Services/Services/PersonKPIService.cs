using DataReef.Core.Extensions;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DataViews.SelfTrackedKPIs;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Reports;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using DataReef.TM.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PersonKPIService : DataService<PersonKPI>, IPersonKPIService
    {
        private readonly Lazy<IBlobService> _blobService;
        private readonly Lazy<IPersonSettingService> _personSettingService;

        public PersonKPIService(ILogger logger, 
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IBlobService> blobService,
            Lazy<IPersonSettingService> personSettingService) : base(logger, unitOfWorkFactory)
        {
            this._blobService = blobService;
            this._personSettingService = personSettingService;
        }

        public ICollection<PersonKPI> ListKPIsFromDate(DateTime date, bool includeDeleted = false)
        {
            using (var ctx = UnitOfWorkFactory())
            {
                var query = ctx.Get<PersonKPI>().Where(x => x.DateCreated >= date);

                if (!includeDeleted)
                {
                    query.Where(x => !x.IsDeleted);
                }

                return query.ToList();
            }
        }

        public string SaveKPIScreenShot(string screenshotBase64, DateTime date)
        {
            var uniqueImgIdentifier = Guid.NewGuid();
            var originalImageBytes = Convert.FromBase64String(screenshotBase64);
            
            using (var ms = new MemoryStream(originalImageBytes))
            {
                var img = System.Drawing.Image.FromStream(ms);

                var imageName = $"users/{SmartPrincipal.UserId}/tallies/{date.ToString("yyyy-MM-dd_HH-mm-ss")}";

                return _blobService.Value.UploadByNameGetFileUrl(imageName, new BlobModel { Content = originalImageBytes, ContentType = img.RawFormat.GetMimeType() }, BlobAccessRights.PublicRead);
            }
        }

        public void ResetKPIs(DateTime date)
        {
            // set person setting
            var talliesSettings =
                _personSettingService
                .Value
                .GetSettings(SmartPrincipal.UserId, PersonSettingGroupType.Tallies)
                ?? new Dictionary<string, ValueTypePair<SettingValueType, string>>();

            var settingValue = new ValueTypePair<SettingValueType, string>(SettingValueType.String, date.ToString());

            //dictionary handles insert or update
            talliesSettings[PersonSetting.TalliesLastResetDate] = settingValue;

            _personSettingService.Value.SetSettings(SmartPrincipal.UserId, PersonSettingGroupType.Tallies, talliesSettings);
        }

        public override ICollection<PersonKPI> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            var personFilter = $"PersonID={SmartPrincipal.UserId}";
            if (!string.IsNullOrEmpty(filter))
            {
                personFilter = string.Join("&", new List<string>() { filter, personFilter });
            }
            return base.List(deletedItems, pageNumber, itemsPerPage, personFilter, include, exclude, fields);
        }

        public override PersonKPI Insert(PersonKPI entity)
        {
            if (entity.Value == 0)
            {
                throw new Exception($"KPI must have a non-zero value");
            }

            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new Exception("KPI must have a name");
            }
            return base.Insert(entity);
        }

        public override PersonKPI Update(PersonKPI entity)
        {
            if (entity.Value == 0)
            {
                throw new Exception($"KPI must have a non-zero value");
            }

            if (string.IsNullOrEmpty(entity.Name))
            {
                throw new Exception("KPI must have a name");
            }
            return base.Update(entity);
        }

        public ICollection<SelfTrackedStatistics> GetSelfTrackedStatisticsForOrganization(Guid ouId, DateTime? specifiedDay)
        {
            var result = new List<SelfTrackedStatistics>();

            using (DataContext dc = new DataContext())
            {
                var dates = new StatisticDates(specifiedDay);

                var ouTeamIds = dc
                            .OUAssociations
                            .Where(oua => oua.OUID == ouId)
                            .Select(oua => oua.PersonID)
                            .Distinct()
                            .ToList();

                return dc
                        .PersonKPIs
                        .Where(p => ouTeamIds.Contains(p.PersonID))
                        .ToList()
                        .Select(p => new SimplePersonKPI(p))
                        .GroupBy(p => p.Name)
                        .Select(g => g.ToStatisticRow(dates))
                        .ToList();
            }
        }

        public ICollection<SelfTrackedStatistics> GetSelfTrackedStatisticsForPerson(Guid personID, DateTime? specifiedDay)
        {
            using (DataContext dc = new DataContext())
            {
                var dates = new StatisticDates(specifiedDay);

                return dc
                        .PersonKPIs
                        .Where(p => p.PersonID == personID).ToList()
                        .GroupBy(p => p.PersonID)
                        .SelectMany(g => g.ToStatisticRows(dates))
                        .ToList();
            }
        }

        public ICollection<InquiryStatisticsForPerson> GetSelfTrackedStatisticsForSalesPeopleTerritories(ICollection<Guid> personIds, IEnumerable<PersonReportingSettingsItem> reportItems, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay, IEnumerable<Guid> excludedReps = null)
        {
            var kpiStatistics = new List<InquiryStatisticsForPerson>();

            using (DataContext dc = new DataContext())
            {
                var dates = new StatisticDates(specifiedDay);
                excludedReps = excludedReps ?? new List<Guid>();

                if (reportItems?.Any() == true)
                {
                    //self tracked kpis to include when making the call to the db
                    var searchKpis = new HashSet<string>();
                    foreach (var repItem in reportItems)
                    {
                        if(repItem.IncludedPersonKpis?.Any() == true)
                        {
                            searchKpis.UnionWith(repItem.IncludedPersonKpis);
                        }
                    }

                    var kpiDates = dc.PersonKPIs
                        .Where(p => !excludedReps.Contains(p.PersonID) &&
                                            personIds.Contains(p.PersonID) &&
                                                p.IsDeleted == false &&
                                                searchKpis.Contains(p.Name))
                                    .Select(i => new { PersonID = i.PersonID, DateCreated = i.DateCreated, Kpi = i.Name, Value = i.Value }).ToList();


                    foreach (var col in reportItems)
                    {
                        if(col.IncludedPersonKpis?.Any() != true)
                        {
                            continue;
                        }
                        var kpiDatesForKpi = kpiDates.Where(kd => col.IncludedPersonKpis.Contains(kd.Kpi));
                        foreach (var personId in personIds)
                        {

                            var personKpiDates = kpiDatesForKpi.Where(id => id.PersonID == personId).ToList();

                            kpiStatistics.Add(new InquiryStatisticsForPerson
                            {
                                PersonId = personId,
                                Name = col.ColumnName,
                                Actions = new InquiryStatisticsByDate
                                {
                                    AllTime = personKpiDates.Sum(x => x.Value),
                                    ThisYear = personKpiDates.Where(id => id.DateCreated >= dates.YearStart).Sum(x => x.Value),
                                    ThisMonth = personKpiDates.Where(id => id.DateCreated >= dates.MonthStart).Sum(x => x.Value),
                                    ThisWeek = personKpiDates.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).Sum(x => x.Value),
                                    Today = personKpiDates.Where(id => id.DateCreated >= dates.TodayStart).Sum(x => x.Value),
                                    ThisQuarter = personKpiDates.Where(id => id.DateCreated >= dates.QuaterStart).Sum(x => x.Value),
                                    SpecifiedDay = specifiedDay.HasValue ? personKpiDates.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).Sum(x => x.Value) : 0,
                                    RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? personKpiDates.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay).Sum(x => x.Value) : 0
                                },
                                DaysActive = new InquiryStatisticsByDate
                                {
                                    AllTime = personKpiDates.GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisYear = personKpiDates.Where(id => id.DateCreated >= dates.YearStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisMonth = personKpiDates.Where(id => id.DateCreated >= dates.MonthStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisWeek = personKpiDates.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    Today = personKpiDates.Where(id => id.DateCreated >= dates.TodayStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    ThisQuarter = personKpiDates.Where(id => id.DateCreated >= dates.QuaterStart).GroupBy(id => id.DateCreated.Date).Count(),
                                    SpecifiedDay = specifiedDay.HasValue ? personKpiDates.Where(id => id.DateCreated >= dates.SpecifiedStart && id.DateCreated < dates.SpecifiedEnd).GroupBy(id => id.DateCreated.Date).Count() : 0,
                                    RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? personKpiDates.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated < EndRangeDay).GroupBy(id => id.DateCreated.Date).Count() : 0
                                }
                            });

                            if(col.ColumnName == "ClockHours")
                            {
                                var PersonClockTime = dc.PersonClockTime.Where(id => id.PersonID == personId).Select(i => new { PersonID = i.PersonID, DateCreated = i.DateCreated, ClockMin = i.ClockMin }).ToList();

                                if (PersonClockTime != null)
                                {
                                    kpiStatistics.Add(new InquiryStatisticsForPerson
                                    {
                                        PersonId = personId,
                                        Name = col.ColumnName,
                                        Actions = new InquiryStatisticsByDate
                                        {
                                            AllTime = PersonClockTime.Sum(x => x.ClockMin),
                                            ThisYear = PersonClockTime.Where(id => id.DateCreated >= dates.YearStart).Sum(x => x.ClockMin),
                                            ThisMonth = PersonClockTime.Where(id => id.DateCreated >= dates.MonthStart).Sum(x => x.ClockMin),
                                            ThisWeek = PersonClockTime.Where(id => id.DateCreated.Date >= dates.CurrentWeekStart).Sum(x => x.ClockMin),
                                            Today = PersonClockTime.Where(id => id.DateCreated >= dates.TodayStart).Sum(x => x.ClockMin),
                                            SpecifiedDay = specifiedDay.HasValue ? PersonClockTime.Where(id => id.DateCreated >= dates.SpecifiedStart.Value && id.DateCreated < dates.SpecifiedEnd.Value).Sum(x => x.ClockMin) : 0,
                                            ThisQuarter = PersonClockTime.Where(id => id.DateCreated >= dates.QuaterStart).Sum(x => x.ClockMin),
                                            RangeDay = (StartRangeDay.HasValue && EndRangeDay.HasValue) ? PersonClockTime.Where(id => id.DateCreated >= StartRangeDay && id.DateCreated <= EndRangeDay).Sum(x => x.ClockMin) : 0
                                        },
                                        DaysActive = new InquiryStatisticsByDate
                                        { }

                                    });
                                }
                            }

                        }
                    }
                }

                return kpiStatistics;
            }
        }
    }
}
