using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Managers CRUD for OUAssociations
    /// </summary>
    [RoutePrefix("api/v1/ouassociations")]
    public class OUAssociationsController : EntityCrudController<OUAssociation>
    {
        private readonly IOUAssociationService _associationService;
        private readonly IOUService            _ouService;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dataService"></param>
        /// <param name="logger"></param>
        public OUAssociationsController(IOUAssociationService associationService, IOUService ouService, ILogger logger) :
            base(associationService, logger)
        {
            _associationService = associationService;
            _ouService          = ouService;
        }

        public override async Task<ICollection<OUAssociation>> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "")
        {
            var results = await base.List(deletedItems, pageNumber, itemsPerPage, include, exclude, fields);
            if (include.Contains("OU"))
            {
                results = _ouService.PopulateAssociationsOUs(results);
            }
            return results;
        }
        /// <summary>
        /// This method will strip the response of any Association that is redundant because of the role
        /// </summary>
        /// <param name="deletedItems"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="include"></param>
        /// <param name="exclude"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("smart")]
        public async Task<ICollection<OUAssociation>> SmartList(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "")
        {
            string filters = FilterStringFromQueryStrings(Request.GetQueryNameValuePairs());

            filters = filters ?? string.Empty;

            var entityList = _associationService.SmartList(deletedItems, pageNumber, itemsPerPage, filters, include, exclude, fields);
            if (include.Contains("OU"))
            {
                entityList = _ouService.PopulateAssociationsOUs(entityList);
            }

            SetupSerialization(entityList, include, exclude, fields);
            
            return entityList;
        }


        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            OUsControllerCacheInvalidation();

            return await base.DeleteByGuid(guid);
        }

        public override async Task<OUAssociation> Post(OUAssociation item)
        {
            OUsControllerCacheInvalidation();

            return await base.Post(item);
        }

        public override async Task<OUAssociation> Patch(System.Web.Http.OData.Delta<OUAssociation> item)
        {
            OUsControllerCacheInvalidation();

            return await base.Patch(item);
        }

        public override async Task<HttpResponseMessage> Delete(OUAssociation item)
        {
            OUsControllerCacheInvalidation();

            return await base.Delete(item);
        }

        public override async Task<ICollection<OUAssociation>> PostMany(List<OUAssociation> items)
        {
            OUsControllerCacheInvalidation();

            return await base.PostMany(items);
        }

        public override async Task<OUAssociation> Put(OUAssociation item)
        {
            OUsControllerCacheInvalidation();

            return await base.Put(item);
        }

        /// <summary>
        /// Invalidate cache for OUsController GET methods.
        /// </summary>
        public void OUsControllerCacheInvalidation()
        {
            var cache               = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);
            string controllerName   = typeof(OUsController).FullName;

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