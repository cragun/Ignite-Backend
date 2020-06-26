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
using DataReef.TM.Services;
using DataReef.TM.Services.Services.FinanceAdapters.Sunlight;
using System.Text;
using RestSharp;

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

        [Route("{financePlanDefinitionId:guid}/{propertyId:guid}/creditcheckurls")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SmartBOARDCreditCheck>))]
        public async Task<IHttpActionResult> GetPropertyCreditCheckUrl(Guid financePlanDefinitionId, Guid propertyID)
        {
            return Ok(_financePlanDefinitionService.Value.GetCreditCheckUrlForFinancePlanDefinitionAndPropertyID(financePlanDefinitionId, propertyID));
        }


        public class SunlightProjects
        {
            public string returnCode { get; set; }
            public List<Models.DTOs.Solar.Finance.Products> projects { get; set; }
        }
        public class SunlightProducts
        {
            public string returnCode { get; set; }
            public List<Models.DTOs.Solar.Finance.Products> products { get; set; }
        }

        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["Sunlight.test.url"];

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }
        
        public IRestResponse Test()
        {
            SunlightProducts req = new SunlightProducts();
            Models.DTOs.Solar.Finance.Products product = new Models.DTOs.Solar.Finance.Products();

            product.loanType = "Single";
            product.term = 300;
            product.apr = 3.99;
            product.isACH = true;
            product.stateName = "Texas";

            req.products = new List<Models.DTOs.Solar.Finance.Products>();
            req.products.Add(product);

            string AuthUsername = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Username"];
            string AuthPassword = System.Configuration.ConfigurationManager.AppSettings["Sunlight.Auth.Password"];

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(AuthUsername + ":" + AuthPassword));
            var request = new RestRequest($"/product/fetchproducts/", Method.POST);
            request.AddJsonBody(req);
            request.AddHeader("Authorization", "Basic " + svcCredentials);
            string token = "00DJ0000003HLkU!AR4AQA_G63cMTXNK9VDMFKnabv6OOH6yPOEmRG_n5TKKFfOGQghYAEPmzKs0.q2X6.EcfDI48TeOyvmi8wW5MHd77ST.MO19";
            request.AddHeader("SFAccessToken", "Bearer " + token);

            var response = client.Execute(request);
            return response;
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