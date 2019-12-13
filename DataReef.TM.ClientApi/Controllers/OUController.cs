using DataReef.TM.ClientApi.Models;
using DataReef.TM.ClientApi.Responses;
using DataReef.TM.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DataReef.TM.ClientApi.Controllers
{
    [RoutePrefix("organizations")]
    [AllowAnonymous]
    public class OUController : ApiController
    {
        private IOUService ouService;


        public OUController(IOUService ouService)
        {
            this.ouService = ouService;
        }

        [HttpGet]
        [Route("")]
        public GenericResponse<List<OUModel>> GetOrganizations()
        {
            try
            {
                //todo: get from currentPrincipal
                Guid rootOUID = new Guid("C5F80842-F4EB-425E-A93D-0571E357D18A");

                var guids = this.ouService.GetHierarchicalOrganizationGuids(new List<Guid>() { rootOUID });
                var ous = ouService.GetMany(guids);

                List<OUModel> list = new List<OUModel>();
                foreach(var ou in ous)
                {
                    OUModel oum = new OUModel()
                    {
                        ID = ou.Guid,
                        Name = ou.Name,
                        ParentID = ou.ParentID
                    };

                    list.Add(oum);
                }

                return new GenericResponse<List<OUModel>>(list);


            }
            catch (Exception ex)
            {
                GenericResponse<List<OUModel>> ret = new GenericResponse<List<OUModel>>(null);
                return ret;


            }
        }
    }
}
