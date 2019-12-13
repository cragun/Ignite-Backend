using Amazon.Runtime.Internal.Util;
using CsvHelper;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class CsvReportingService : ICsvReportingService
    {
        private readonly Lazy<IOUService> _ouService = null;
        private readonly Lazy<IPersonService> _personService = null;
        private readonly Lazy<IPersonKPIService> _personKPIService = null;
        private readonly Lazy<IBlobService> _blobService = null;
        private readonly Lazy<IReportingServices> _reportingServices = null;
        private readonly Lazy<IPropertyService> _propertyService = null;

        public CsvReportingService(
            Lazy<IOUService> ouService,
            Lazy<IPersonService> personService,
            Lazy<IPersonKPIService> personKPIService,
            Lazy<IBlobService> blobService,
            Lazy<IReportingServices> reportingServices,
            Lazy<IPropertyService> propertyService)
        {
            _ouService = ouService;
            _personService = personService;
            _personKPIService = personKPIService;
            _blobService = blobService;
            _reportingServices = reportingServices;
            _propertyService = propertyService;
        }

        public byte[] GenerateOrganizationSelfTrackedReport(Guid startOUID, DateTime? specifiedDay, ReportingPeriod reportingPeriod)
        {
            var reportRows = _reportingServices.Value.GetOrganizationSelfTrackedReport(startOUID, specifiedDay).ToList();
            if (reportRows?.Any() == true)
            {
                //initialize with known commmon columns
                var knownHeaders = new List<string> { "Office" };

                //add dynamic columns
                var dynamicHeaders = new List<string>();
                dynamicHeaders.AddRange(reportRows.SelectMany(x => x.SelfTrackedStatistics?.Select(s => s.KPIName)).Distinct().ToList());
                knownHeaders.AddRange(dynamicHeaders);

                var fileContent = new List<List<string>>();

                reportRows.ForEach(x =>
                {
                    var kpiForOrgList = new List<string> { x.OfficeName };
                    kpiForOrgList
                    .AddRange(
                        dynamicHeaders.Select(
                            kpi =>
                                x.SelfTrackedStatistics
                                ?.FirstOrDefault(s => s.KPIName.Equals(kpi))
                                ?.GetStatisticForReportingPeriod(reportingPeriod)
                                 .ToString() ?? string.Empty));

                    fileContent.Add(kpiForOrgList);
                });

                return GenerateFileBytes(knownHeaders, fileContent);
            }

            return null;
        }

        public byte[] GenerateSalesRepSelfTrackedReport(Guid ouID, DateTime? specifiedDay, ReportingPeriod reportingPeriod)
        {
            var reportRows = _reportingServices.Value.GetSalesRepresentativeSelfTrackedReport(ouID, specifiedDay).ToList();
            if (reportRows?.Any() == true)
            {
                //initialize with known commmon columns
                var knownHeaders = new List<string> { "Name" };

                //add dynamic columns
                var dynamicHeaders = new List<string>();
                dynamicHeaders.AddRange(reportRows.SelectMany(x => x.SelfTrackedStatistics?.Select(s => s.KPIName)).Distinct().ToList());
                knownHeaders.AddRange(dynamicHeaders);

                var fileContent = new List<List<string>>();

                reportRows.ForEach(x =>
                {
                    var kpiForOrgList = new List<string> { x.Name };
                    kpiForOrgList
                    .AddRange(
                        dynamicHeaders.Select(
                            kpi =>
                                x.SelfTrackedStatistics
                                ?.FirstOrDefault(s => s.KPIName.Equals(kpi))
                                ?.GetStatisticForReportingPeriod(reportingPeriod)
                                 .ToString() ?? string.Empty));

                    fileContent.Add(kpiForOrgList);
                });

                return GenerateFileBytes(knownHeaders, fileContent);
            }

            return null;
        }

        public byte[] GeneratePropertyCsvReport(string wkt)
        {
            var properties = _propertyService.Value.GetPropertiesInShape(wkt);

            List<List<string>> fileRows = new List<List<string>>();
            foreach (var property in properties)
            {
                var rowData = new List<string>();
                rowData.Add(property.GetFormattedAddress());
                rowData.Add(property.Name);
                rowData.Add(property.GetMainOccupant()?.GetFullName());
                rowData.Add(property.GetFormattedAddress());
                fileRows.Add(rowData);
            }

            return GenerateFileBytes(new List<string> { "Property Address", "Owner", "Occupant", "Mailing Address" }, fileRows);
        }

        private byte[] GenerateFileBytes(List<string> headers, List<List<string>> fileContent)
        {
            using (var memStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        //write headers
                        headers.ForEach(x => csvWriter.WriteField(x));
                        csvWriter.NextRecord();

                        //write content
                        fileContent.ForEach(x =>
                        {
                            x.ForEach(l => csvWriter.WriteField(l));
                            csvWriter.NextRecord();
                        });
                    }
                }

                return memStream.ToArray();
            }
        }
    }
}
