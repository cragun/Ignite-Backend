using DataReef.Core.Classes;
using DataReef.Core.Logging;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DataReef.TM.Contracts.Faults;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DataViews.Geo;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for Territories
    /// </summary>
    [RoutePrefix("api/v1/territories")]
    public class TerritoriesController : EntityCrudController<Territory>
    {
        private readonly ITerritoryService _territoryService;
        private readonly IPropertyService _propertyService;
        private readonly IInquiryService _inquiryService;

        public TerritoriesController(ITerritoryService territoryService,
                                     IPropertyService propertyService,
                                     IInquiryService inquiryService,
                                     ILogger logger)
            : base(territoryService, logger)
        {
            _territoryService = territoryService;
            _propertyService = propertyService;
            _inquiryService = inquiryService;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Property>))]
        [Route("{territoryID:guid}/properties")]
        public IHttpActionResult GetPropertiesByStatus(Guid territoryID, string disposition, string propertyNameSearch = null, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            var ret = _propertyService.GetTerritoryPropertiesByStatusPaged(territoryID, disposition, propertyNameSearch, pageIndex, itemsPerPage, include, exclude);
            return Ok<ICollection<Property>>(ret);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Property>))]
        [Route("{territoryID:guid}/properties/withproposal")]
        public IHttpActionResult GetPropertiesWithProposal(Guid territoryID, string propertyNameSearch = null, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            var ret = _propertyService.GetTerritoryPropertiesWithProposal(territoryID, propertyNameSearch, pageIndex, itemsPerPage, include, exclude);
            return Ok(ret);
        }

        [HttpGet]
        [Route("{territoryID:guid}/propertycount")]
        public IHttpActionResult GetPropertiesCount(Guid territoryID)
        {
            var territory = _territoryService.Get(territoryID);
            if (territory == null)
                return NotFound();

            var propertyCount = _territoryService.GetPropertiesCount(territory);

            return Ok(new GenericResponse<int> { Response = propertyCount });
        }

        [HttpPost]
        [Route("wkt/propertiescount")]
        [ResponseType(typeof(GenericResponse<long>))]
        public IHttpActionResult GetPropertiesCountForShale([FromBody] GenericRequest<string> request)
        {
            var propertiesCount = _territoryService.GetPropertiesCountForWKT(request.Request);
            return Ok(new GenericResponse<long> { Response = propertiesCount });
        }

        [HttpGet]
        [Route("{territoryID:Guid}/inquiryStats")]
        [ResponseType(typeof(ICollection<InquiryStatisticsForOrganization>))]
        public IHttpActionResult GetTerritorySummaryForCustomStatuses(Guid territoryID, DateTime? specifiedDay = null)
        {
            var data = _territoryService.GetInquiryStatisticsForTerritory(territoryID, null, specifiedDay);
            return Ok(data);
        }

        [HttpGet]
        [Route("")]
        [ResponseType(typeof(ICollection<Territory>))]
        public IHttpActionResult GetForCurrentUserAndOU([FromUri] Guid ouID, [FromUri] Guid personID, string include = "", string fields = "")
        {
            ICollection<Territory> ret = _territoryService.GetForCurrentUserAndOU(ouID, personID, include, fields);
            return Ok<ICollection<Territory>>(ret);
        }

        [HttpPost]
        [Route("validateName")]
        [ResponseType(typeof(SaveResult))]
        public HttpResponseMessage IsNameAvailable([FromBody]GuidAndNameRequest req)
        {
            var result = _territoryService.IsNameAvailable(req.Guid, req.Name);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [Route("getbyshapesversion")]
        [ResponseType(typeof(ICollection<Territory>))]
        public IHttpActionResult GetByShapesVersion([FromBody]ICollection<TerritoryShapeVersion> shapesVersion, Guid ouid, Guid? personID = null, bool deletedItems = false, string include = "")
        {
            var territories = _territoryService.GetByShapesVersion(ouid, personID, shapesVersion, deletedItems, include).ToList();

            return Ok(territories);
        }

        [HttpPost]
        [Route("{territoryID:Guid}/archive")]
        [ResponseType(typeof(Territory))]
        public IHttpActionResult SetArchivedStatus(Guid territoryID, [FromBody]GenericRequest<bool> request)
        {
            if(request == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var territory = _territoryService.SetArchiveStatus(territoryID, request.Request);

            return Ok(territory);
        }

        protected override string PrepareEntityForNavigationPropertiesAttachment(Territory entity)
        {
            var includeProperties = new List<string>();

            if (entity.Shapes != null)
            {
                entity
                    .Shapes
                    .ToList()
                    .ForEach(s => s.TerritoryID = entity.Guid);
                includeProperties.Add("Shapes");
            }
            if (entity.Assignments != null)
            {
                entity
                    .Assignments
                    .ToList()
                    .ForEach(t => t.TerritoryID = entity.Guid);
                includeProperties.Add("Assignments");
            }
            if (entity.Prescreens != null)
            {
                entity
                    .Prescreens
                    .ToList()
                    .ForEach(a => a.TerritoryID = entity.Guid);
                includeProperties.Add("Prescreens");
            }

            return string.Join(",", includeProperties);
        }

        /// <summary>
        /// Checks whether a Territory is valid: has WKT and Shapes.
        /// </summary>
        /// <param name="item"></param>
        public void CheckTerritoryValidity(Territory item)
        {
            if (item.Shapes == null || item.Shapes.Count == 0 || String.IsNullOrWhiteSpace(item.WellKnownText))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        public override Territory Post(Territory item)
        {
            CheckTerritoryValidity(item);

            return base.Post(item);
        }

        public override Territory Patch(System.Web.Http.OData.Delta<Territory> item)
        {
            CheckTerritoryValidity(item.GetEntity());

            return base.Patch(item);
        }

        public override ICollection<Territory> PostMany(List<Territory> items)
        {
            if (items == null || !items.Any())
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            items.ForEach(i => CheckTerritoryValidity(i));

            return base.PostMany(items);
        }

        public override Territory Put(Territory item)
        {
            CheckTerritoryValidity(item);

            return base.Put(item);
        }


    }
}
