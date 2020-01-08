using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        private readonly Func<IPropertyAttachmentService> _propertyAttachmentServiceFactory;

        public PropertiesController(IPropertyService propertyService,
                                    ILogger logger,
                                    Func<IPropertyAttachmentService> propertyAttachmentServiceFactory)
            : base(propertyService, logger)
        {
            this.propertyService = propertyService;
            _propertyAttachmentServiceFactory = propertyAttachmentServiceFactory;
        }



        /// <summary>
        /// / Gets all Territories regardless of Lat - Long, for a property
        /// </summary>
        /// <param name="propertyID"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        [Route("{propertyID:guid}/territories/{apiKey}")]
        [ResponseType(typeof(ICollection<Inquiry>))]
        [HttpGet]
        public IHttpActionResult GetTerritoriesForProperty(Guid propertyID, string apiKey)
        {
            try
            {
                var result = propertyService.GetTerritoriesList(propertyID, apiKey);
                return Ok(result);
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
        public IHttpActionResult GetInquiriesForProperty(Guid propertyID)
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
        public IHttpActionResult GetInquiriesForPropertyAndPerson(Guid propertyID, Guid personID)
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
        public IHttpActionResult GetPropertyPowerConsumptions(Guid propertyID)
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
        public IEnumerable<SBAttachmentDataDTO> GetPropertyAttachments(long propertyID, string apiKey)
        {
            return _propertyAttachmentServiceFactory().GetSBAttachmentData(propertyID, apiKey);
        }


        protected override string PrepareEntityForNavigationPropertiesAttachment(Property entity)
        {
            // on Andrei's request, always send the Occupants and PropertyBags back
            return "Occupants,PropertyBag,Attributes";
        }

        public override HttpResponseMessage DeleteByGuid(Guid guid)
        {
            OUsControllerCacheInvalidation();

            return base.DeleteByGuid(guid);
        }

        public override Property Post(Property item)
        {
            OUsControllerCacheInvalidation();
            try
            {
                return base.Post(item);
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

        public override Property Patch(System.Web.Http.OData.Delta<Property> item)
        {
            OUsControllerCacheInvalidation();

            return base.Patch(item);
        }

        public override HttpResponseMessage Delete(Property item)
        {
            OUsControllerCacheInvalidation();

            return base.Delete(item);
        }

        public override ICollection<Property> PostMany(List<Property> items)
        {
            OUsControllerCacheInvalidation();

            return base.PostMany(items);
        }

        public override Property Put(Property item)
        {
            OUsControllerCacheInvalidation();

            return base.Put(item);
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
    }
}
