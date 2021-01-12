using DataReef.Auth.Helpers;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Properties
    /// </summary>
    [RoutePrefix("api/v1/properties")]
    public class PropertiesController : EntityCrudController<Property>
    {
        private readonly IPropertyService propertyService;
        private readonly ISunnovaAdapter sunnovaAdapter;
        private readonly Func<IPropertyAttachmentService> _propertyAttachmentServiceFactory;

        public PropertiesController(IPropertyService propertyService,
                                    ILogger logger,
                                    ISunnovaAdapter sunnovaAdapter,
                                    Func<IPropertyAttachmentService> propertyAttachmentServiceFactory)
            : base(propertyService, logger)
        {
            this.propertyService = propertyService;
            this.sunnovaAdapter = sunnovaAdapter;
            _propertyAttachmentServiceFactory = propertyAttachmentServiceFactory;
        }

        /// <summary>
        /// / Gets all Territories regardless of Lat - Long, for a property
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("{propertyID:guid}/territories/{apiKey}")]
        [ResponseType(typeof(IEnumerable<Territories>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetTerritoriesForProperty(Guid propertyID, string apiKey)
        {
            try
            {
                var result = await propertyService.GetTerritoriesList(propertyID, apiKey);
                return Ok(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// / Gets all Territories base on apikey only
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="Lat"></param>
        /// <param name="Long"></param>
        /// <returns></returns>
        [Route("GetTerritoryList/{apiKey}")]
        //[ResponseType(typeof(IEnumerable<Territories>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetTerritoryListbyApikey(string apiKey, double Lat, double Long)
        {
            try
            {
                bool checkTime = CryptographyHelper.checkTime(apiKey);
                string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

                var result = await propertyService.GetTerritoryListbyApikey(DecyptApiKey, Lat, Long);
                if (result == null || result.Count() <= 0)
                {
                    return Ok("Leads Couldn't be created because  it is outside of the Territory.");
                }
                return Ok(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public class masterrslt
        {
            public IEnumerable<Territories> list { get; set; }
            public double Lat { get; set; }
            public double Long { get; set; }
        }

        /// <summary>
        /// Gets all Master Territories base on apikey and lat-long
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="Lat"></param>
        /// <param name="Long"></param>
        /// <returns></returns>
        [Route("GetTerritoryListMaster/{apiKey}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetTerritoryListbyApikeyMaster(string apiKey, double Lat, double Long)
        {
            try
            {
                bool checkTime = CryptographyHelper.checkTime(apiKey);
                string DecyptApiKey = CryptographyHelper.getDecryptAPIKey(apiKey);

                var result = await propertyService.GetTerritoryListbyApikeyMaster(DecyptApiKey, Lat, Long);
                if (result == null || result.Count() <= 0)
                {
                    return Ok("Leads Couldn't be created because  it is outside of the Territory.");
                }

                masterrslt mslt = new masterrslt();
                mslt.list = result;
                mslt.Lat = Lat;
                mslt.Long = Long;

                return Ok(mslt);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// / Gets all Territories , apikey base on lat-long
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="Lat"></param>
        /// <param name="Long"></param>
        /// <returns></returns>
        [Route("Sb/GetTerritoryNApikey/{apiKey}")]
        [ResponseType(typeof(IEnumerable<TerritoryApikey>))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetTerritoryNApikey(string apiKey, double Lat, double Long)
        {
            try
            {
                bool checkTime = CryptographyHelper.checkTime(apiKey);

                var result = await propertyService.TerritoryNApikey(Lat, Long);
                return Ok(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// / Gets all Territories regardless of Lat - Long, for a property
        /// </summary>
        /// <param name="propertyID"></param>
        /// <returns></returns>
        [Route("GetEsid/{propertyID:guid}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetEsidByAddressPropertyid(Guid propertyID)
        {
            try
            {
                var result = await propertyService.GetEsidByAddress(propertyID);
                return Ok(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// / Check Is Property available or not
        /// </summary>
        /// <param name="igniteId"></param>
        /// <returns></returns>
        [Route("IsPropertyAvailable/{igniteId}")]
        [HttpGet]
        public async Task<IHttpActionResult> IsPropertyAvailable(long igniteId)
        {
            try
            {
                var result = await propertyService.IsPropertyAvailable(igniteId);
                //return Ok(result);
                return Json(new { result = result });
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// / Gets all inquiries regardless of person, for a property
        /// </summary>
        /// <param name="propertyID"></param>
        /// <returns></returns>
        [Route("{propertyID:guid}/inquiries")]
        [ResponseType(typeof(ICollection<Inquiry>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetInquiriesForProperty(Guid propertyID)
        {
            try
            {
                Property property = this.propertyService.Get(propertyID, "Inquiries");
                if (property == null || property.Inquiries == null)
                {
                    return NotFound();
                }

                List<Inquiry> ret = property.Inquiries.ToList();
                return Ok<List<Inquiry>>(ret);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        [Route("{propertyID:guid}/inquiries/{personID:guid}")]
        [ResponseType(typeof(ICollection<Inquiry>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetInquiriesForPropertyAndPerson(Guid propertyID, Guid personID)
        {
            try
            {
                Property property = this.propertyService.Get(propertyID, "Inquiries");
                if (property == null || property.Inquiries == null)
                {
                    return NotFound();
                }

                List<Inquiry> ret = property.Inquiries.Where(ii => ii.PersonID == personID).ToList();

                return Ok<List<Inquiry>>(ret);

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [Route("{propertyID:guid}/powerconsumptions")]
        [ResponseType(typeof(ICollection<PropertyPowerConsumption>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetPropertyPowerConsumptions(Guid propertyID)
        {
            var property = propertyService.Get(propertyID, "PowerConsumptions");
            if (property?.PowerConsumptions == null)
                return NotFound();

            var powerConsumptions = property
                                    .PowerConsumptions
                                    .Where(p => !p.IsDeleted)
                                    .ToList();

            // JsonSerializer puts all the powerConsumptions into the property, and does not serialize the content in the array.
            foreach (var powerConsumption in powerConsumptions)
            {
                if (powerConsumption?.Property?.PowerConsumptions != null)
                {
                    powerConsumption.Property.PowerConsumptions = null;
                }
            }

            return Ok(powerConsumptions);
        }


        /// <summary>
        /// Get method used by SmartBoard to retrieve all the attachments for a property
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="apiKey"></param>
        [HttpGet, Route("attachments/{propertyID}/{apiKey}")]
        [AllowAnonymous, InjectAuthPrincipal]
        public async Task<IEnumerable<SBAttachmentDataDTO>> GetPropertyAttachments(long propertyID, string apiKey)
        {
            return _propertyAttachmentServiceFactory().GetSBAttachmentData(propertyID, apiKey);
        }


        protected override string PrepareEntityForNavigationPropertiesAttachment(Property entity)
        {
            // on Andrei's request, always send the Occupants and PropertyBags back
            return "Occupants,PropertyBag,Attributes";
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            OUsControllerCacheInvalidation();

            return await base.DeleteByGuid(guid);
        }

        public override async Task<Property> Post(Property item)
        {
            OUsControllerCacheInvalidation();
            try
            {
                return await base.Post(item);
            }
            catch (HttpResponseException ex)
            {
                // Get the exception and send the message as plain text.
                var response = ex.Response.Content.ReadAsStringAsync().Result;
                var data = new { Message = "" };
                data = JsonConvert.DeserializeAnonymousType(response, data);
                if (!string.IsNullOrWhiteSpace(data.Message))
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(data.Message)
                    });
                }
                throw ex;
            }
        }

        public override async Task<Property> Patch(System.Web.Http.OData.Delta<Property> item)
        {
            OUsControllerCacheInvalidation();

            return await base.Patch(item);
        }

        public override async Task<HttpResponseMessage> Delete(Property item)
        {
            OUsControllerCacheInvalidation();

            return await base.Delete(item);
        }

        public override async Task<ICollection<Property>> PostMany(List<Property> items)
        {
            OUsControllerCacheInvalidation();

            return await base.PostMany(items);
        }

        public override async Task<Property> Put(Property item)
        {
            OUsControllerCacheInvalidation();

            return await base.Put(item);
        }

        /// <summary>
        /// Invalidate cache for OUsController GET methods.
        /// </summary>
        public void OUsControllerCacheInvalidation()
        {
            var cache = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);
            string controllerName = typeof(OUsController).FullName;

            foreach (var key in cache.AllKeys)
            {
                if (key.StartsWith(controllerName, StringComparison.CurrentCultureIgnoreCase))
                {
                    cache.Remove(key);
                }
            }
        }


        //[AllowAnonymous]
        //[Route("sunnovatoken")]
        //[HttpGet]
        //public async Task<string> sunnovatoken()
        //{
        //    try
        //    {
        //        return sunnovaAdapter.GetSunnovaToken();
        //    }
        //    catch (System.Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// send data to sunnova website
        /// </summary>
        /// <param name="propertyID"></param>
        /// <returns></returns>
        [Route("SendLeadSunnova/{propertyID:guid}")]
        [HttpGet]
        public async Task<IHttpActionResult> SendLeadSunnova(Guid propertyID)
        {
            try
            {
                var result = await propertyService.SendLeadSunnova(propertyID);
                return Ok(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        /// get territories from address
        [Route("territories")] 
        [HttpPost]
        public async Task<IHttpActionResult> GetTerritoriesFromAddress(Property req)
        {
            try
            {
                var result =   propertyService.GetTerritoriesFromAddress(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
