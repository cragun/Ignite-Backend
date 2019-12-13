using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models;

namespace DataReef.TM.ClientApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("keyvalues")]
    public class KeyValueController : ApiController
    {
        private readonly IKeyValueService _keyValueService;
        private readonly IOUService _ouService;

        public KeyValueController(IKeyValueService kvService,IOUService ouService)
        {
            _keyValueService = kvService;
            _ouService = ouService;
        }


        /// <summary>
        /// Updates or Inserts the KeyValue into the objects property bag
        /// </summary>
        /// <param name="KeyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        public IHttpActionResult Upsert(KeyValue kv)
        {
            var guidList = new List<Guid>() { SmartPrincipal.OuId };

            try
            {
                var kvOut = _keyValueService.Upsert(kv);
                var response = KeyValueDataView.FromDbModel(kvOut);

                var ret = new GenericResponse<KeyValueDataView>(response);
                return Ok(ret);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
    }
}