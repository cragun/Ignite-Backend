using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.EPC;
using Newtonsoft.Json;

namespace DataReef.TM.ClientApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("epc")]
    public class EpcController : ApiController
    {
        private readonly Func<IOUSettingService> _ouSettingServiceFactory;
        private readonly Func<IEpcStatusService> _epcStatusServiceFactory;

        public EpcController(
            Func<IOUSettingService> ouSettingServiceFactory, 
            Func<IEpcStatusService> epcStatusServiceFactory)
        {
            _ouSettingServiceFactory = ouSettingServiceFactory;
            _epcStatusServiceFactory = epcStatusServiceFactory;
        }

        [HttpGet]
        [Route("statuses")]
        public IHttpActionResult GetEpcStatuses()
        {
            var ouid = SmartPrincipal.OuId;

            var ouSettings = _ouSettingServiceFactory().GetSettings(ouid, null).Result;
            if (!ouSettings.ContainsKey(TM.Models.OUSetting.Epc_Statuses))
                return Ok(new GenericResponse<List<string>>(new List<string>()));

            var statusesSetting = ouSettings.FirstOrDefault(s => s.Key.Equals(TM.Models.OUSetting.Epc_Statuses, StringComparison.InvariantCultureIgnoreCase)).Value.Value;
            var statuses = JsonConvert.DeserializeObject<List<string>>(statusesSetting);

            return Ok(new GenericResponse<List<string>>(statuses));
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult UploadEpcStatuses([FromBody]List<EpcStatusInput> epcStatuses)
        {
            if (epcStatuses == null)
                return BadRequest($"Invalid {nameof(epcStatuses)}");

            var ouid = SmartPrincipal.OuId;

            _epcStatusServiceFactory().UploadEpcStatuses(ouid, epcStatuses);

            return Ok();
        }
    }
}
