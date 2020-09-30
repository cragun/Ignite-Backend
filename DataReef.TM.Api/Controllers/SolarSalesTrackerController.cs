using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/sst")]
    public class SolarSalesTrackerController : ApiController
    {
        private readonly Func<ISolarSalesTrackerAdapter> _solarSalesTrackerAdapterFactory;

        public SolarSalesTrackerController(Func<ISolarSalesTrackerAdapter> solarSalesTrackerAdapterFactory)
        {
            _solarSalesTrackerAdapterFactory = solarSalesTrackerAdapterFactory;
        }

        [Route("{financePlanID:guid}")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitSolarData(Guid financePlanID)
        {
            var response = _solarSalesTrackerAdapterFactory().SubmitSolarData(financePlanID);

            return Ok(response);
        }

        [HttpPost, AllowAnonymous, Route("proposal/attach/echo")]
        public async Task<SBProposalAttachRequest> AttachProposalEcho(SBProposalAttachRequest req)
        {
            return req;
        }

        [HttpPost, AllowAnonymous, Route("lead/submit/echo")]
        public async Task<SBLeadCreateRequest> SubmitLeadEcho(SBLeadCreateRequest req)
        {
            return req;
        }

    }
}
