using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.ClientApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("customers")]
    public class CustomersController : ApiController
    {

        private IOUService ouService;

        public CustomersController(IOUService ouService)
        {
            this.ouService = ouService;
        }


        [HttpGet]
        [Route("{id:guid}")]
        public IHttpActionResult GetAccount(Guid id)
        {
            return Ok();
        }


        //[HttpGet]
        //[Route("")]
        //public IHttpActionResult GetCustomersForOu(Guid? ouID  = null,System.DateTime? startDate = null,System.DateTime? endDate = null, int pageNumber=1)
        //{
        //    List<PersonDataView> content = new List<PersonDataView>();
        //    var guidList = new List<Guid>() { SmartPrincipal.OuId };
        //    var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);

        //    Guid rootOUID = ouID.HasValue ? ouID.Value : SmartPrincipal.OuId;

        //    if (!ouGuids.Contains(rootOUID))
        //    {
        //        return StatusCode(System.Net.HttpStatusCode.Forbidden);
        //    }
            
        //    return Ok();
        //}



    }
}