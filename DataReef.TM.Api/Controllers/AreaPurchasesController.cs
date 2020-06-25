using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.ZipCodes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/areapurchases")]
    public class AreaPurchasesController : EntityCrudController<AreaPurchase>
    {
        private IAreaPurchaseService _areaPurchaseService;

        public AreaPurchasesController(IAreaPurchaseService areaPurchaseService, ILogger logger)
            : base(areaPurchaseService, logger)
        {
            _areaPurchaseService = areaPurchaseService;
        }

        [HttpGet]
        [Route("ou/{ouid:guid}")]
        [ResponseType(typeof(ICollection<AreaPurchase>))]
        public async Task<IHttpActionResult> GetPurchasesForOU(Guid ouid)
        {
            var purchases = _areaPurchaseService.GetPurchasesForOU(ouid);
            return Ok<ICollection<AreaPurchase>>(purchases);
        }

        [HttpGet]
        [Route("pending")]
        [ResponseType(typeof(ICollection<AreaPurchaseDto>))]
        public async Task<IHttpActionResult> GetAllPendingPurchases()
        {
            var purchases = _areaPurchaseService.GetAllPendingPurchases();
            return Ok<ICollection<AreaPurchaseDto>>(purchases);
        }
        
        [HttpPut]
        [Route("process")]
        public async Task<IHttpActionResult> UpdateProcessedPurchases(ICollection<AreaPurchase> processedPurchases)
        {
            _areaPurchaseService.UpdateProcessedPurchases(processedPurchases);
            return Ok(new {});
        }
    }
}
