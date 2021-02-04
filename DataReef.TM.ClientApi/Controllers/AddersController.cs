using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using System.Net;
using System.Web.Http.Description;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Adders
    /// </summary>
    [RoutePrefix("adders")]
    public class AddersController : ApiController
    {
        private readonly IOUSettingService _ouSettingService;
        private readonly IOUService _ouService;


        public AddersController(IOUSettingService ouSettingService,IOUService ouService)
        {
            _ouSettingService = ouSettingService;
            _ouService = ouService;
        }

        /// <summary>
        /// Get all adders in organization
        /// </summary>
        /// <param name="ouID">The organization id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAdders(Guid? ouID)
        {
            try
            {
                if (ouID==null || ouID == Guid.Empty)
                {
                    return Content(HttpStatusCode.PreconditionFailed, "Missing ouID parameter (Guid of the Organization Unit).  adders?ouid=foobar");
                }

                try
                {

                    List<OrganizationDataView> content = new List<OrganizationDataView>();

                    var guidList = new List<Guid>() { SmartPrincipal.OuId };
                    var ouGuids = _ouService.GetHierarchicalOrganizationGuids(guidList);
                    if (!ouGuids.Contains(ouID.Value))
                    {
                        return Content(HttpStatusCode.Unauthorized, "You are not authorized for this OU");
                    }


                    var settings = Task.Run(() => _ouSettingService.GetSettings(ouID.Value, null)).Result;
                    var adderSetting = settings.FirstOrDefault(s => s.Key == "Adders");

                    var json = adderSetting.Value;

                    if (!string.IsNullOrWhiteSpace(json.Value))
                    {
                        JArray ja = JArray.Parse(json.Value);
                        return Ok<JArray>(ja);
                    }
                    else
                    {
                        return Content(HttpStatusCode.NotFound, "Missing Adder Data For This OU");
                    }
                }
                catch (Exception)
                {
                    return Content(HttpStatusCode.NotFound, "Missing Adder Data For This OU");
                }



                return Ok();



            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}