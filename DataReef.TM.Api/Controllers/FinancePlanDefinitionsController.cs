using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using System.Net.Http;
using System.Net;
using DataReef.TM.Api.Classes.Requests;
using System.Web.Http.Description;
using DataReef.TM.Models.DataViews.Financing;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/financeplandefinitions")]
    public class FinancePlanDefinitionsController : EntityCrudController<FinancePlanDefinition>
    {
        private readonly Lazy<IFinancePlanDefinitionService> _financePlanDefinitionService;
        public FinancePlanDefinitionsController(IDataService<FinancePlanDefinition> dataService,
            Lazy<IFinancePlanDefinitionService> financePlanDefinitionService,
            ILogger logger) : base(dataService, logger)
        {
            _financePlanDefinitionService = financePlanDefinitionService;
        }

        [Route("{financePlanDefinitionId:guid}/creditcheckurls")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SmartBOARDCreditCheck>))]
        public async Task<IHttpActionResult> GetCreditCheckUrl(Guid financePlanDefinitionId)
        {
            return Ok(_financePlanDefinitionService.Value.GetCreditCheckUrlForFinancePlanDefinition(financePlanDefinitionId));
        }


        [Route("{proposalid:guid}/sunlightloanstatus")]
        [HttpGet]
        public async Task<IHttpActionResult> GetSunlightloanstatus(Guid proposalId)
        {
            return Ok(_financePlanDefinitionService.Value.GetSunlightloanstatus(proposalId));
        }


        [Route("{proposalid:guid}/Sunlightsendloandocs")]
        [HttpGet]
        public async Task<IHttpActionResult> Sunlightsendloandocs(Guid proposalId)
        {
            return Ok(_financePlanDefinitionService.Value.Sunlightsendloandocs(proposalId));
        }

        [Route("{financePlanDefinitionId:guid}/{propertyId:guid}/creditcheckurls")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SmartBOARDCreditCheck>))]
        public async Task<IHttpActionResult> GetPropertyCreditCheckUrl(Guid financePlanDefinitionId, Guid propertyID)
        {
            return Ok(_financePlanDefinitionService.Value.GetCreditCheckUrlForFinancePlanDefinitionAndPropertyID(financePlanDefinitionId, propertyID));
        }

        #region Forbidden methods

        //public override FinancePlanDefinition Post(FinancePlanDefinition item)
        //{
        //    throw new HttpResponseException(HttpStatusCode.Forbidden);
        //}

        //public override ICollection<FinancePlanDefinition> PostMany(List<FinancePlanDefinition> items)
        //{
        //    throw new HttpResponseException(HttpStatusCode.Forbidden);
        //}

        //public override FinancePlanDefinition Put(FinancePlanDefinition item)
        //{
        //    throw new HttpResponseException(HttpStatusCode.Forbidden);
        //}

        public override async Task<HttpResponseMessage> Delete(FinancePlanDefinition item)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        public override async Task<HttpResponseMessage> DeleteMany([FromBody] IDsListWrapperRequest req)
        {
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }

        #endregion
    }
}