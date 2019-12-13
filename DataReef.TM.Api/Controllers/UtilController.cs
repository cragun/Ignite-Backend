using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using System;
using System.Web.Http;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/util")]
    public class UtilController : ApiController
    {
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IOUSettingService> _ouSettingService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _sstAdapter;
        public UtilController(Lazy<IOUService> ouService,
                              Lazy<IOUSettingService> ouSettingService,
                              Lazy<ISolarSalesTrackerAdapter> sstAdapter)
        {
            _ouService = ouService;
            _ouSettingService = ouSettingService;
            _sstAdapter = sstAdapter;
        }


        [Route("updateequipment")]
        [HttpGet]
        [AllowAnonymous]
        [InjectAuthPrincipal]
        public IHttpActionResult UpdateEquipment()
        {
            _ouSettingService.Value.UpdateEquipment();
            return Ok();
        }


        [Route("updateSBSettings")]
        [HttpGet]
        [AllowAnonymous]
        [InjectAuthPrincipal]
        public IHttpActionResult UpdateSBSettings()
        {
            _ouSettingService.Value.UpdateSBSettings();
            return Ok();
        }

        //[Route("sb/{proposalDataID}")]
        //[HttpGet]
        //[AllowAnonymous]
        //[InjectAuthPrincipal]
        //public IHttpActionResult GetSB(Guid proposalDataID)
        //{
        //    var data = _sstAdapter.Value.BuildProposalDataModel(proposalDataID);
        //    return Ok(data);
        //}
    }
}