using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Reports;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{    
    [RoutePrefix("api/v1/reports")]
    public class ReportsController : ApiController
    {
        private readonly IReportingServices _reportingService;
        private readonly ICsvReportingService _csvReportingService;
        private readonly IOUService _ouservice;

        public ReportsController(IReportingServices reportingService, ICsvReportingService csvReportingService, IOUService ouservice)
        {
            _reportingService = reportingService;
            _csvReportingService = csvReportingService;
            _ouservice = ouservice;
        }

        /// <summary>
        /// Gets the data needed for the specified report
        /// </summary>
        /// <param name="reportName">the name of the report</param>
        /// <param name="ouID">the organizational unit guid</param>
        /// <param name="specifiedDay">The date of a day that will be added to the report dates along with Today, WeekToDate, MonthToDate, YearToDate</param>
        /// <param name="StartRangeDay">The date of a day that choose from dateRange Start</param>
        /// <param name="EndRangeDay">The date of a day that choose from dateRange End</param>
        /// <param name="proptype">proptype = tabtype for ios</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{reportName}")]
        public async Task<IHttpActionResult> GenerateReport(string reportName, Guid ouID, DateTime? specifiedDay = null, DateTime? StartRangeDay = null, DateTime? EndRangeDay = null, string proptype = null)
        {
            try
            {
                switch (reportName)
                {
                    case "OrganizationReport": return Ok<ICollection<OrganizationReportRow>>(_reportingService.GetOrganizationReport(ouID, specifiedDay, StartRangeDay, EndRangeDay));
                    case "SalesRepresentativeReport": return Ok<ICollection<SalesRepresentativeReportRow>>(await _reportingService.GetSalesRepresentativeReport(ouID, specifiedDay, StartRangeDay, EndRangeDay, proptype));
                    case "OrganizationSelfTrackedReport": return Ok<ICollection<OrganizationSelfTrackedReportRow>>(await _reportingService.GetOrganizationSelfTrackedReport(ouID, specifiedDay));
                    default: return BadRequest();
                }
            }
            catch (ArgumentException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }
        }

        [HttpGet]
        [Route("csv")]                                                      
        public async Task<IHttpActionResult> GenerateCsvReport(CsvReportType reportType, Guid ouID, ReportingPeriod reportingPeriod, DateTime? specifiedDay = null)
        {
            byte[] fileBytes;
            try
            {
                switch (reportType)
                {                    
                    case CsvReportType.OrganizationSelfTrackedReport:
                        fileBytes = _csvReportingService.GenerateOrganizationSelfTrackedReport(ouID, specifiedDay, reportingPeriod);
                        break;
                    case CsvReportType.SalesRepresentativeSelfTrackedReport:
                        fileBytes = _csvReportingService.GenerateSalesRepSelfTrackedReport(ouID, specifiedDay, reportingPeriod);
                        break;
                    default: return BadRequest();
                }
            }
            catch (ArgumentException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(ex.Message) });
            }

            if(fileBytes == null)
            {
                return BadRequest();
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };

            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = "report.csv"
            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return ResponseMessage(result);
        }
    }
}
