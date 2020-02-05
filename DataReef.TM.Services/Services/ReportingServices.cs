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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

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

        public ICollection<OrganizationReportRow> GetOrganizationReport(Guid startOUID, DateTime? specifiedDay)
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
                        .Where(o => !o.IsDeleted && (o.Guid == startOUID || o.ParentID == startOUID))
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
                        .Distinct()
                        .ToList();
                    

                    var reportRow = new OrganizationReportRow
                    {
                        Id = ou.Guid,
                        OfficeName = ou.Name,
                        TotalReps = reps.Count,
                        WorkingReps = _inquiryService.Value.GetWorkingRepsDisposition(reps, reportSettings?.WorkingRepsDispositions, specifiedDay),
                        InquiryStatistics = _ouService.Value.GetInquiryStatisticsForOrganization(ou.Guid, reportSettings, specifiedDay, repExclusionList)
                    };
                    results.Add(reportRow);
                }
            }
            return results;
        }

        public ICollection<SalesRepresentativeReportRow> GetSalesRepresentativeReport(Guid startOUID, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay)
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



            var inquiryStatistics = _ouService.Value.GetInquiryStatisticsForSalesPeople(startOUID, reportSettings, specifiedDay, StartRangeDay, EndRangeDay, repExclusionList);
            var peopleIds = inquiryStatistics.Select(i => i.PersonId).Distinct().ToList();
            var people = _personService.Value.GetMany(peopleIds).Where(p => !repExclusionList.Contains(p.Guid));
            foreach (var personId in peopleIds)
            {
                var person = people.SingleOrDefault(p => p.Guid == personId);

                var reportRow = NormalizeSalesRepresentativeReportRow(
                        personId, 
                        person != null ? string.Format("{0} {1}", person.FirstName, person.LastName) : "???", 
                        inquiryStatistics.Where(i => i.PersonId == personId).ToList(), 
                        reportSettings);
                
                results.Add(reportRow);
            }
            return results;
        }

        public ICollection<OrganizationSelfTrackedReportRow> GetOrganizationSelfTrackedReport(Guid startOUID, DateTime? specifiedDay)
        {

            using (var context = new DataContext())
            {

                // check if user has access to the ouID                
                if (!UserHasAccessToOU(SmartPrincipal.UserId, startOUID))
                {
                    return new List<OrganizationSelfTrackedReportRow>();
                }

                return context
                        .OUs
                        .Where(o => (o.Guid == startOUID || o.ParentID == startOUID))
                        .ToList()
                        .Select(ou => new OrganizationSelfTrackedReportRow
                        {
                            Id = ou.Guid,
                            OfficeName = ou.Name,
                            SelfTrackedStatistics = _personKPIService.Value.GetSelfTrackedStatisticsForOrganization(ou.Guid, specifiedDay)
                        })
                        .ToList();
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

        private SalesRepresentativeReportRow NormalizeSalesRepresentativeReportRow(Guid personId, string name, IEnumerable<InquiryStatisticsForPerson> stats, OUReportingSettings settings)
        {
            SalesRepresentativeReportRow row = new SalesRepresentativeReportRow
            {
                Id = personId,
                Name = name,
                InquiryStatistics = new List<InquiryStatisticsForPerson>()
            };
            if (settings == null)
            {
                return null;
            }

            foreach(var col in settings.PersonReportItems)
            {
                var matchingStat = stats?.FirstOrDefault(x => x.Name == col.ColumnName);
                if(matchingStat != null)
                {
                    row.InquiryStatistics.Add(matchingStat);
                }
                else
                {
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
            return row;
        }
    }
}
