using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Organizations
    /// </summary>
    [RoutePrefix("organizations")]
    public class OrganizationsController : ApiController
    {

        private IOUService ouService;

        public OrganizationsController(IOUService ouService)
        {
            this.ouService = ouService;
        }

        /// <summary>
        /// Get the organization hierarchy for the given id
        /// </summary>
        /// <param name="id">The organization id</param>
        /// <returns>Organization Data View</returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ResponseType(typeof(GenericResponse<List<OrganizationDataView>>))]
        public IHttpActionResult GetOrganization(Guid id)
        {

            List<OrganizationDataView> content = new List<OrganizationDataView>();

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);
            if (!ouGuids.Contains(id))
            {
                return NotFound();
            }

            var ou = this.ouService.Get(id);
            var ret = new GenericResponse<List<OrganizationDataView>>(content);

            OrganizationDataView dv = new OrganizationDataView(ou);
            ret.Response.Add(dv);

            return Ok(ret);

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("webhook")]
        public IHttpActionResult TriggerWebHook()
        {
            

            OU ou = new OU();
            ou.Guid = Guid.NewGuid();
            ou.Name = "Cragun";
            ou.ParentID = new Guid("C5F80842-F4EB-425E-A93D-0571E357D18A");
            ou.OrganizationType = TM.Models.Enums.OUType.IntegratedDealer;
            ou.RootOrganizationID = ou.ParentID;
            ou.TenantID = 0;
            ou.Version = 1;
            ou.AccountID = new Guid("9205625D-D618-4372-90A8-ED2CCB9B6C0E");

            ou.Shapes = new List<OUShape>();
            OUShape ous = new OUShape();
            ous.OUID = ou.Guid;
            ous.Guid = Guid.NewGuid();
            ous.ShapeTypeID = "city";
            ou.Shapes.Add(ous);

            this.ouService.Insert(ou);

            return Ok();
        }

        /// <summary>
        /// The the organizations hierarchy
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")] // this will not work if the request is NOT authenticated
        [ResponseType(typeof(GenericResponse<List<OrganizationDataView>>))]
        public IHttpActionResult GetOrganizations()
        {

            List<OrganizationDataView> content = new List<OrganizationDataView>();

            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = this.ouService.GetHierarchicalOrganizationGuids(guidList);
            var ous = this.ouService.GetMany(ouGuids);
            var ret = new GenericResponse<List<OrganizationDataView>>(content);

            foreach (var ou in ous)
            {
                OrganizationDataView dv = new OrganizationDataView(ou);
                ret.Response.Add(dv);
            }
         
            return Ok(ret);

        }
    }
}