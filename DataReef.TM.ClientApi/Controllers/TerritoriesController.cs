using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.ClientApi.Models;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Signatures;

namespace DataReef.TM.ClientApi.Controllers
{
    /// <summary>
    /// Territories
    /// </summary>
    [RoutePrefix("territories")]
    public class TerritoriesController : ApiController
    {
        private readonly Func<IOUService> _ouServiceFactory;
        private readonly Func<ITerritoryService> _territoryServiceFactory;
        private readonly Func<IPropertyService> _propertyServiceFactory;

        public TerritoriesController(
            Func<IOUService> ouServiceFactory,
            Func<ITerritoryService> territoryServiceFactory,
            Func<IPropertyService> propertyServiceFactory)
        {
            _ouServiceFactory = ouServiceFactory;
            _territoryServiceFactory = territoryServiceFactory;
            _propertyServiceFactory = propertyServiceFactory;
        }

        /// <summary>
        /// Get territories details
        /// </summary>
        /// <param name="pageNumber">Page number, default 1</param>
        /// <param name="itemsPerPage">Items per page, default 1000</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(GenericResponse<List<TerritoryDataView>>))]
        public IHttpActionResult GetTerritories([FromUri]int pageNumber = 1, [FromUri]int itemsPerPage = 1000)
        {
            var guidList = new List<Guid>() { SmartPrincipal.OuId };
            var ouGuids = _ouServiceFactory().GetHierarchicalOrganizationGuids(guidList);

            if (ouGuids == null || !ouGuids.Any())
                return Ok(new GenericResponse<List<TerritoryDataView>>(new List<TerritoryDataView>()));

            var response = new List<TerritoryDataView>();
            var territoryService = _territoryServiceFactory();
            foreach (var ouGuid in ouGuids)
            {
                var ouTerritories = territoryService.List(itemsPerPage: int.MaxValue, filter: $"OUID={ouGuid}").ToList();
                response.AddRange(ouTerritories.Select(t => new TerritoryDataView(t)));
            }

            response = response.GroupBy(t => t.TerritoryID).Select(g => g.First()).ToList();
            response = response.OrderBy(t => t.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return Ok(new GenericResponse<List<TerritoryDataView>>(response));
        }

        /// <summary>
        /// Get territory details
        /// </summary>
        /// <param name="territoryID">The territory id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{territoryID:guid}")]
        [ResponseType(typeof(GenericResponse<TerritoryDataView>))]
        public IHttpActionResult GetTerritory(Guid territoryID)
        {
            var territoriesResult = GetTerritories(itemsPerPage: int.MaxValue) as OkNegotiatedContentResult<GenericResponse<List<TerritoryDataView>>>;
            if (territoriesResult == null) return Ok(new GenericResponse<TerritoryDataView>(null));

            var territories = territoriesResult.Content.Response;
            if (territories.All(t => t.TerritoryID != territoryID))
                return NotFound();

            var result = territories.FirstOrDefault(t => t.TerritoryID == territoryID);

            return Ok(new GenericResponse<TerritoryDataView>(result));
        }

        /// <summary>
        /// Get properties details
        /// </summary>
        /// <param name="pageNumber">Page number, default 1</param>
        /// <param name="itemsPerPage">Items per page, default 1000</param>
        /// <returns></returns>
        [HttpGet]
        [Route("properties")]
        public IHttpActionResult GetProperties([FromUri]int pageNumber = 1, [FromUri]int itemsPerPage = 1000)
        {
            var territoriesResult = GetTerritories(itemsPerPage: int.MaxValue) as OkNegotiatedContentResult<GenericResponse<List<TerritoryDataView>>>;
            if (territoriesResult == null) return Ok(new GenericResponse<List<PropertyDataView>>(null));

            var territories = territoriesResult.Content.Response;
            var response = new List<PropertyDataView>();
            var propertyService = _propertyServiceFactory();
            foreach (var territory in territories)
            {
                var territoryProperties = propertyService.List(itemsPerPage: int.MaxValue, filter: $"TerritoryID={territory.TerritoryID}").ToList();
                response.AddRange(territoryProperties.Select(p => new PropertyDataView(p)));
            }

            response = response.GroupBy(p => p.PropertyID).Select(g => g.First()).ToList();
            response = response.OrderBy(p => p.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return Ok(new GenericResponse<List<PropertyDataView>>(response));
        }

        /// <summary>
        /// Get territory properties details
        /// </summary>
        /// <param name="territoryID">The territory id</param>
        /// <param name="pageNumber">Page number, default 1</param>
        /// <param name="itemsPerPage">Items per page, default 1000</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{territoryID:guid}/properties")]
        public IHttpActionResult GetPropertiesByTerritory(Guid territoryID, [FromUri]int pageNumber = 1, [FromUri]int itemsPerPage = 1000)
        {
            var territoryResult = GetTerritory(territoryID) as OkNegotiatedContentResult<GenericResponse<TerritoryDataView>>;
            if (territoryResult == null) return NotFound();

            var territory = territoryResult.Content.Response;
            var territoryProperties = _propertyServiceFactory().List(itemsPerPage: int.MaxValue, filter: $"TerritoryID={territory.TerritoryID}").ToList();
            var response = territoryProperties.Select(p => new PropertyDataView(p)).ToList();

            response = response.GroupBy(p => p.PropertyID).Select(g => g.First()).ToList();
            response = response.OrderBy(p => p.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

            return Ok(new GenericResponse<List<PropertyDataView>>(response));
        }
    }
}
