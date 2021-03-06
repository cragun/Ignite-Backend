using CsvHelper;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DTOs.Reports;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

namespace DataReef.TM.Services
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ReportingServices : IReportingServices
    {
        private readonly ILogger _logger = null;
        private readonly Lazy<IOUService> _ouService = null;
        private readonly Lazy<IOUSettingService> _ouSettingService = null;
        private readonly Lazy<IPersonService> _personService = null;
        private readonly Lazy<IPersonKPIService> _personKPIService = null;
        private readonly Lazy<IInquiryService> _inquiryService = null;

        public ReportingServices(ILogger logger,
            Lazy<IOUService> ouService,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<IPersonService> personService,
            Lazy<IPersonKPIService> personKPIService,
            Lazy<IInquiryService> inquiryService)
        {
            _logger = logger;
            _ouService = ouService;
            _ouSettingService = ouSettingService;
            _personService = personService;
            _personKPIService = personKPIService;
            _inquiryService = inquiryService;
        }

        public ICollection<OrganizationReportRow> GetOrganizationReport(Guid startOUID, DateTime? specifiedDay, DateTime? StartRangeDay , DateTime? EndRangeDay )
        {
            var results = new List<OrganizationReportRow>();

            using (var context = new DataContext())
            {
                List<OU> ous = null;

                // if it's root OU, we get root OU id for authenticated person
                if (context.OUs.Any(o => o.Guid == startOUID && o.ParentID == null))
                {
                    startOUID = _ouService
                                    .Value
                                    .ListRootsForPerson(SmartPrincipal.UserId, string.Empty, string.Empty, string.Empty)
                                    .FirstOrDefault()?
                                    .Guid ?? startOUID;
                }

                ous = context
                        .OUs
                        .Where(o => !o.IsDeleted && (o.Guid == startOUID || o.ParentID == startOUID)).AsNoTracking()
                        .ToList();



                foreach (var ou in ous)
                {
                    var ouSettings = _ouSettingService
                        .Value
                        .GetSettingsByOUID(ou.Guid);

                    var repExclusionList =
                        ouSettings
                        ?.FirstOrDefault(s => s.Name == "Reporting.Exclusion.Guids")
                        ?.GetValue<List<Guid>>() ?? new List<Guid>();

                    var reportSettings =
                        ouSettings
                        ?.FirstOrDefault(s => s.Name == OUSetting.OU_Reporting_Settings)
                        ?.GetValue<OUReportingSettings>();

                    var reps = _ouService
                        .Value
                        .ConditionalGetActivePeopleIDsForCurrentAndSubOUs(ou.Guid, true)
                        .Distinct();


                    var reportRow = new OrganizationReportRow
                    {
                        Id = ou.Guid,
                        OfficeName = ou.Name,
                        TotalReps = reps.Count(),
                        WorkingReps = _inquiryService.Value.GetWorkingRepsDisposition(reps, reportSettings?.WorkingRepsDispositions, specifiedDay, StartRangeDay, EndRangeDay),
                        InquiryStatistics = _ouService.Value.GetInquiryStatisticsForOrganization(ou.Guid, reportSettings, specifiedDay, StartRangeDay, EndRangeDay, repExclusionList)
                    };
                    results.Add(reportRow);
                }
            }
            return results;
        }

        public async Task<ICollection<SalesRepresentativeReportRow>> GetSalesRepresentativeReport(Guid startOUID, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay, string proptype)
        {
            var results = new List<SalesRepresentativeReportRow>();

            using (var context = new DataContext())
            {
                // if the start OUID is a root ou guid, we'll use the first root for authenticated user
                if (context.OUs.Any(o => o.Guid == startOUID && o.ParentID == null))
                {
                    startOUID = _ouService
                                .Value
                                .ListRootsForPerson(SmartPrincipal.UserId, string.Empty, string.Empty, string.Empty)
                                .FirstOrDefault()?
                                .Guid ?? startOUID;
                }
            }

            var ouSettings = _ouSettingService
                        .Value
                        .GetSettingsByOUID(startOUID);

            var repExclusionList =
                ouSettings
                ?.FirstOrDefault(s => s.Name == "Reporting.Exclusion.Guids")
                ?.GetValue<List<Guid>>() ?? new List<Guid>();

            var reportSettings =
                        ouSettings
                        ?.FirstOrDefault(s => s.Name == OUSetting.OU_Reporting_Settings)
                        ?.GetValue<OUReportingSettings>();

            var inquiryStatistics = await _ouService.Value.GetInquiryStatisticsForSalesPeople(startOUID, reportSettings, specifiedDay, StartRangeDay, EndRangeDay, repExclusionList);
            var peopleIds = inquiryStatistics.Select(i => i.PersonId).Distinct();

            var peopleIdsWithOuAss = _personService.Value.GetMany(peopleIds, "OUAssociations", "", "", true).Where(p => !repExclusionList.Contains(p.Guid));

            foreach (var personId in peopleIds)
            {
                var person = peopleIdsWithOuAss.Where(x => x.Guid == personId && x.OUAssociations.Any(y => (y.RoleType == OURoleType.Member || y.RoleType == OURoleType.Manager))).FirstOrDefault();

                if (person == null)
                {
                    continue;
                }

                var reportRow = NormalizeSalesRepresentativeReportRow(
                    personId,
                    person != null ? string.Format("{0} {1}", person.FirstName, person.LastName) : "???",
                    person != null ? person.IsDeleted : false,
                    inquiryStatistics.Where(i => i.PersonId == personId),
                    reportSettings,
                    proptype);

                if (reportRow.InquiryStatistics.Count() > 0)
                {
                    results.Add(reportRow);
                }

            }
            return results;
        }

        public async Task<ICollection<OrganizationSelfTrackedReportRow>> GetOrganizationSelfTrackedReport(Guid startOUID, DateTime? specifiedDay)
        {

            using (var context = new DataContext())
            {

                // check if user has access to the ouID                
                if (!UserHasAccessToOU(SmartPrincipal.UserId, startOUID))
                {
                    return new List<OrganizationSelfTrackedReportRow>();
                }

                return (await context
                         .OUs
                         .Where(o => (o.Guid == startOUID || o.ParentID == startOUID)).AsNoTracking().ToListAsync())
                         .Select(ou => new OrganizationSelfTrackedReportRow
                         {
                             Id = ou.Guid,
                             OfficeName = ou.Name,
                             SelfTrackedStatistics = _personKPIService.Value.GetSelfTrackedStatisticsForOrganization(ou.Guid, specifiedDay)
                         }).ToList();
            }
        }

        public ICollection<SalesRepresentativeSelfTrackedReportRow> GetSalesRepresentativeSelfTrackedReport(Guid startOUID, DateTime? specifiedDay)
        {
            var results = new List<SalesRepresentativeSelfTrackedReportRow>();

            using (var context = new DataContext())
            {
                // check if user has access to the ouID                
                if (!UserHasAccessToOU(SmartPrincipal.UserId, startOUID))
                {
                    return results;
                }

                var orgAndSubOrgIDs = _ouService.Value.GetOUAndChildrenGuids(startOUID);

                var ouTeamIds = context
                            .OUAssociations
                            .Where(oua => orgAndSubOrgIDs.Contains(oua.OUID))
                            .Select(oua => oua.PersonID)
                            .Distinct()
                            .ToList();

                return context
                        .PersonKPIs
                        .Include("Person")
                        .Where(p => ouTeamIds.Contains(p.PersonID))
                        .DistinctBy(x => x.PersonID)
                        .ToList()
                        .Select(p => new SalesRepresentativeSelfTrackedReportRow
                        {
                            Id = p.Guid,
                            Name = $"{p.Person.FirstName} {p.Person.LastName}",
                            SelfTrackedStatistics = _personKPIService.Value.GetSelfTrackedStatisticsForPerson(p.PersonID, specifiedDay)
                        })
                        .ToList();
            }
        }

        private bool UserHasAccessToOU(Guid userID, Guid startOUID)
        {
            var rootOrgIds = _ouService.Value.ListRootGuidsForPerson(userID);
            if (rootOrgIds == null)
            {
                return false;
            }
            if (!rootOrgIds.Contains(startOUID))
            {
                if (_ouService.Value.GetHierarchicalOrganizationGuids(rootOrgIds)?.Contains(startOUID) != true)
                {
                    return false;
                }
            }

            return true;
        }

        private SalesRepresentativeReportRow NormalizeSalesRepresentativeReportRow(Guid personId, string name, bool IsDeleted, IEnumerable<InquiryStatisticsForPerson> stats, OUReportingSettings settings, string proptype)
        {
            bool isallZero = true;
            bool isallZeroproptype = true;
            SalesRepresentativeReportRow row = new SalesRepresentativeReportRow
            {
                Id = personId,
                Name = name,
                IsDeleted = IsDeleted,
                InquiryStatistics = new List<InquiryStatisticsForPerson>()
            };
            if (settings == null)
            {
                return null;
            }
            foreach (var col in settings.PersonReportItems)
            {
                var matchingStat = stats?.FirstOrDefault(x => x.Name == col.ColumnName);               

                if (matchingStat != null)
                {
                    if (IsDeleted == true)
                    {
                        if(!string.IsNullOrEmpty(proptype))
                        {
                        	long isitzero = Convert.ToInt64(matchingStat.Actions.GetType().GetProperty(proptype).GetValue(matchingStat.Actions));
                            isallZeroproptype = isallZeroproptype && (isitzero == 0);
                            long isitzeroa = Convert.ToInt64(matchingStat.DaysActive.GetType().GetProperty(proptype).GetValue(matchingStat.DaysActive));
                            isallZeroproptype = isallZeroproptype && (isitzeroa == 0);
                        }
                        else
                        {
                            isallZeroproptype = false;
                        }
                        
                        isallZero = isallZero && matchingStat.Actions.GetType().GetProperties().All(p => int.Equals((p.GetValue(matchingStat.Actions) as int?), 0));
                        isallZero = isallZero && matchingStat.DaysActive.GetType().GetProperties().All(p => int.Equals((p.GetValue(matchingStat.DaysActive) as int?), 0));
                    }


                    if (col.ColumnName == "CAPP(%)")
                    {
                        var ApptsCAPP = stats.Where(x => x.Name == "Appts CAPP").FirstOrDefault();
                        var OPP = stats.Where(x => x.Name == "OPP").FirstOrDefault();

                        if (ApptsCAPP != null && OPP != null)
                        {
                        	  row.InquiryStatistics.Add(new Models.DataViews.Inquiries.InquiryStatisticsForPerson
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
                                    AllTime = (ApptsCAPP.DaysActive.AllTime > 0 && OPP.DaysActive.AllTime > 0) ? (ApptsCAPP.DaysActive.AllTime * 100) / OPP.DaysActive.AllTime : 0,
                                    ThisYear = (ApptsCAPP.DaysActive.ThisYear > 0 && OPP.DaysActive.ThisYear > 0) ? (ApptsCAPP.DaysActive.ThisYear * 100) / OPP.DaysActive.ThisYear : 0,
                                    ThisMonth = (ApptsCAPP.DaysActive.ThisMonth > 0 && OPP.DaysActive.ThisMonth > 0) ? (ApptsCAPP.DaysActive.ThisMonth * 100) / OPP.DaysActive.ThisMonth : 0,
                                    ThisWeek = (ApptsCAPP.DaysActive.ThisWeek > 0 && OPP.DaysActive.ThisWeek > 0) ? (ApptsCAPP.DaysActive.ThisWeek * 100) / OPP.DaysActive.ThisWeek : 0,
                                    Today = (ApptsCAPP.DaysActive.Today > 0 && OPP.DaysActive.Today > 0) ? (ApptsCAPP.DaysActive.Today * 100) / OPP.DaysActive.Today : 0,
                                    SpecifiedDay = (ApptsCAPP.DaysActive.SpecifiedDay > 0 && OPP.DaysActive.SpecifiedDay > 0) ? (ApptsCAPP.DaysActive.SpecifiedDay * 100) / OPP.DaysActive.SpecifiedDay : 0,
                                    ThisQuarter = (ApptsCAPP.DaysActive.ThisQuarter > 0 && OPP.DaysActive.ThisQuarter > 0) ? (ApptsCAPP.DaysActive.ThisQuarter * 100) / OPP.DaysActive.ThisQuarter : 0,
                                    RangeDay = (ApptsCAPP.DaysActive.RangeDay > 0 && OPP.DaysActive.RangeDay > 0) ? (ApptsCAPP.DaysActive.RangeDay * 100) / OPP.DaysActive.RangeDay : 0
                                }
                            });
                        }
                    }
                    else
                    {
                        row.InquiryStatistics.Add(matchingStat);
                    }                    
                }
                else {
                    //add the object with default values
                    row.InquiryStatistics.Add(new Models.DataViews.Inquiries.InquiryStatisticsForPerson
                    {
                        PersonId = personId,
                        Name = col.ColumnName,
                        Actions = new InquiryStatisticsByDate(),
                        DaysActive = new InquiryStatisticsByDate()
                    });
                }

            }
            if(isallZeroproptype == true && IsDeleted == true)
            {
                row.InquiryStatistics = new List<InquiryStatisticsForPerson>();
            }
            if (isallZero == true && IsDeleted == true)
            {
                row.InquiryStatistics = new List<InquiryStatisticsForPerson>();
            }
            return row;
        }
    }
}
