using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.TM.Api.JsonFormatter;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Mortgage;
using Newtonsoft.Json;
using DataReef.TM.Api.Classes.Requests;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/mortgage")]
    public class MortgageController : ApiController
    {
        private readonly IMortgageService _mortgageService;

        public MortgageController(IMortgageService mortgageService)
        {
            _mortgageService = mortgageService;
        }

        [HttpGet]
        [Route("{propertyId:guid}")]
        [ResponseType(typeof(MortgageSource))]
        public async Task<IHttpActionResult> GetMortgageDetails(Guid propertyId)
        {
            if (propertyId == Guid.Empty)
                return Content(HttpStatusCode.PreconditionFailed, nameof(propertyId));

            MortgageSource mortgageDetail;
            try
            {
                mortgageDetail = _mortgageService.GetMortgageDetails(propertyId) ?? new MortgageSource();
            }
            catch(Exception e)
            {
                mortgageDetail = new MortgageSource();
            }            

            return Json(mortgageDetail, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new MortgageContractResolver()
            });
        }

        [HttpPost]
        [Route("")]
        [ResponseType(typeof(ICollection<MortgageSource>))]
        public async Task<IHttpActionResult> GetMortgageDetails([FromBody] MortgageRequest request)
        {
            if (request == null)
                return Content(HttpStatusCode.PreconditionFailed, nameof(request));

            List<MortgageSource> response;
            try
            {
                var mortgageDetails = _mortgageService.GetMortgageDetailsFiltered(request);
                response = mortgageDetails.Detail.Total > 0
                    ? mortgageDetails.Detail.Items.Select(i => i.Source).ToList()
                    : new List<MortgageSource>();
            }
            catch(Exception e)
            {
                response = new List<MortgageSource>();
            }

            return Ok(response);
        }

    }
}
