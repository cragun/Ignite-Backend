using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.OUs;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;
using GoogleMaps.LocationServices;
using System.Threading.Tasks;
using DataReef.Auth.Helpers;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Models.DTOs.QuotasCommitments;
//using Serilog;
//using Serilog.Context;
//using System.Web;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/quotascommitments")]
    public class QuotasCommitmentsController : EntityCrudController<QuotasCommitment>
    {
        private readonly IQuotasCommitmentsService quotasCommitmentsService;
        public QuotasCommitmentsController(IQuotasCommitmentsService quotasCommitmentsService, Core.Logging.ILogger logger) : base(quotasCommitmentsService, logger)
        {
            this.quotasCommitmentsService = quotasCommitmentsService;
        }

        [HttpGet, AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(AdminQuotas))]
        [Route("gettype")]
        public async Task<IHttpActionResult> GetQuotasType()
        {
            return Ok(quotasCommitmentsService.GetQuotasType());
        }


        [HttpPost, AllowAnonymous, InjectAuthPrincipal]
        [Route("roles/users")]
        public async Task<IHttpActionResult> GetUsersFromRoleType(QuotasCommitment request)
        {
            return Ok(new { Response = await quotasCommitmentsService.GetUsersFromRoleType(request.RoleID) });
        }

        //add quotas by admin
        [HttpPost, Route("addQuotas")]
        public async Task<IHttpActionResult> InsertQuotas(QuotasCommitment request)
        {
            var ret = quotasCommitmentsService.InsertQuotas(request);
            return Ok(ret);
        }

        [HttpGet]
        [Route("quota/report")]
        public async Task<IHttpActionResult> GetQuotasReport()
        {
            var ret = quotasCommitmentsService.GetQuotasReport();
            return Ok(new { Response = ret });
        }

        [HttpPost]
        [Route("person/report")]
        public async Task<IHttpActionResult> GetQuotasReportByPerson(QuotasCommitment req)
        {
            var ret = quotasCommitmentsService.GetQuotasReportByPerson(req);
            return Ok(new { Response = ret });
        }

        [HttpPost]
        [Route("commitement/report")]
        public async Task<IHttpActionResult> GetCommitementsReport(QuotasCommitment req)
        {
            var ret = quotasCommitmentsService.GetQuotasCommitementsReport(req);
            return Ok(new { Response = ret });
        }

        //add Commitments by user
        [HttpPost, Route("addCommitments")]
        public async Task<IHttpActionResult> InsertCommitments(QuotasCommitment request)
        {
            var ret = quotasCommitmentsService.InsertCommitments(request);
            return Ok(ret);
        }

        //[HttpPost, Route("daterange")]
        //public async Task<IHttpActionResult> GetQuotasDateRange(QuotasCommitment request)
        //{
        //    var ret = quotasCommitmentsService.GetQuotasDateRange(request);
        //    return Ok(new { Response = ret });
        //} 

        [HttpPost, Route("isSetCommitments")]
        public async Task<IHttpActionResult> IsCommitmentsSetByUser(QuotasCommitment request)
        {
            return Ok(new { Response = quotasCommitmentsService.IsCommitmentsSetByUser(request) });

        }
    }
}