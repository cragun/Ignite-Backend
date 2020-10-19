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

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/quotascommitments")]
    public class QuotasCommitmentsController : EntityCrudController<QuotasCommitment>
    {
        private readonly IQuotasCommitmentsService quotasCommitmentsService;
        public QuotasCommitmentsController(IQuotasCommitmentsService quotasCommitmentsService, ILogger logger) : base(quotasCommitmentsService, logger)
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

        [HttpGet, AllowAnonymous, InjectAuthPrincipal] 
        [Route("roles/users")]
        public async Task<IHttpActionResult> GetUsersFromRoleType(QuotasCommitment request)
        {
            return Ok(quotasCommitmentsService.GetUsersFromRoleType(request.RoleID));
        }

        //add quotas by admin
        [HttpPost, Route("addQuotas")]
        public async Task<IHttpActionResult> InsertQuotas(QuotasCommitment request)
        {
            var ret = quotasCommitmentsService.InsertQuotas(request);
            return Ok(ret);
        }

        [HttpGet]
        [ResponseType(typeof(List<AdminQuotas>))] 
        public async Task<IHttpActionResult> GetQuotasReport()
        {
            return Ok(quotasCommitmentsService.GetQuotasReport());
        }


    }
}