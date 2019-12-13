using System;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.PRMI;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Manages integration with velocify
    /// </summary>
    [RoutePrefix("api/v1/velocify")]
    public class VelocifyController : ApiController
    {
        private readonly IVelocifyService _velocifyService;

        public VelocifyController(IVelocifyService velocifyService)
        {
            this._velocifyService = velocifyService;
        }

        [Route("")]
        [HttpPost]
        [ResponseType(typeof(VelocifyResponse))]
        public IHttpActionResult SendProposal([FromBody]VelocifyRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            if (request.ReferenceID == Guid.Empty)
                return BadRequest("Invalid reference id");

            var response = _velocifyService.SendProposal(null, request);

            return Ok(response);
        }

        [Route("{ouID}")]
        [HttpPost]
        [ResponseType(typeof(VelocifyResponse))]
        public IHttpActionResult SendProposal([FromUri] Guid? ouID, [FromBody]VelocifyRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");
            if (request.ReferenceID == Guid.Empty)
                return BadRequest("Invalid reference id");

            var response = _velocifyService.SendProposal(ouID, request);

            return Ok(response);
        }
    }
}
