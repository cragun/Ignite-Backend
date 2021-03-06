using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/externalcredentials")]
    public class ExternalCredentialsController : EntityCrudController<ExternalCredential>
    {
        public ExternalCredentialsController(IDataService<ExternalCredential> dataService, ILogger logger) : base(dataService, logger)
        {
        }

        /// <summary>
        /// This method is here to allow Credentials change for a given Guid
        /// </summary>
        /// <param name="req"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("set/{id}")]
        public async Task<IHttpActionResult> SetCredentials([FromBody]DataReef.Integrations.Core.Models.Credential req, Guid id)
        {
            var cred = await Get(id);
            cred.Username = req.username;
            cred.Password = req.password;
            await Put(cred);
            return Ok();
        }

    }
}