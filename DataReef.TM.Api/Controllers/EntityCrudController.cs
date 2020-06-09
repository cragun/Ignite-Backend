using DataReef.Core.Classes;
using DataReef.Core.Logging;
using DataReef.TM.Api.Bootstrap;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Api.Common;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;

namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Entity controller that handles default CRUD operations.
    /// </summary>
    // NOTE: Do no decorate base controllers with [AllowAnonymous] attribute!!! The default attribute has Inherited = true and will propagate to inheriting controllers.
    [GenericRoutePrefix("api/v1/{controller}")]
    public class EntityCrudController<T> : ApiController where T : EntityBase
    {
        private readonly IDataService<T> _dataService;
        private readonly ILogger _logger;

        /// <summary>
        /// Default <see cref="EntityCrudController{T}" /> constructor.
        /// </summary>
        /// <param name="dataService">Data service of type {T}</param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EntityCrudController(IDataService<T> dataService, ILogger logger)
        {
            if (dataService == null) throw new ArgumentNullException("dataService");
            if (logger == null) throw new ArgumentNullException("logger");

            this._dataService = dataService;
            this._logger = logger;
        }

        #region Crud operations

        /// <summary>
        /// The list action provides pagination and filtering for a type of entity.
        /// </summary>
        /// <param name="deletedItems">Include deleted items.</param>
        /// <param name="pageNumber">The page number. The number of pages is dependent on the displayer <see cref="itemsPerPage"/> amount.</param>
        /// <param name="itemsPerPage">How many items per page.</param>
        /// <param name="include">Included related complex types.</param>
        /// <param name="exclude">Included related complex types.</param>
        /// <param name="fields">Only select a specific set of fields.</param>
        /// <returns>Returns a collection of entities.</returns>
        /// <exception cref="Exception">Generic exception</exception>
        [HttpGet]
        [GenericRoute("")]
        [CrudApiAction]
        public virtual async Task<ICollection<T>> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "")
        {
            try
            {
                return await GetEntitiesFiltered(null, deletedItems, pageNumber, itemsPerPage, include, exclude, fields);
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Returns an entity.
        /// </summary>
        /// <param name="guid">The unique entity id.</param>
        /// <param name="include">List of Navigation Properties to include in the return data.</param>
        /// <param name="exclude">The explicit fields that should not get returned.</param>
        /// <param name="fields">The explicit fields that should get returned.</param>
        /// <returns>The entity identified by the unique id or a NotFound exception.</returns>
        [HttpGet]
        [CrudApiAction]
        [GenericRoute("{guid:guid}")]
        public virtual async Task<T> Get(Guid guid, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            var entity = this._dataService.Get(guid, include, exclude, fields, deletedItems);

            if (entity == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            //this.AssignNavigationSerialization(include, entity);
            //this.AssignSerializationFields(fields, entity);
            entity.SetupSerialization(include, exclude, fields);
            return entity;
        }

        /// <summary>
        /// Return a list of entities corresponding to there specific ids.
        /// </summary>
        /// <param name="delimitedStringOfGuids">A list of comma (,) or pipe (|) separated unique ids.</param>
        /// <param name="exclude">List of Navigation Properties to include in the return data.</param>
        /// <param name="include">List of Navigation Properties to include in the return data.</param>
        /// <param name="fields">The explicit fields that should get returned.</param>
        /// <returns>A list of entities.</returns>
        [HttpGet]
        [CrudApiAction]
        [GenericRoute("")]
        public virtual async Task<IEnumerable<T>> GetMany(string delimitedStringOfGuids, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            var stringUniqueIds = delimitedStringOfGuids.Split(',', '|');
            var uniqueIds = new List<Guid>();

            foreach (var uniqueIdString in stringUniqueIds)
            {
                try
                {
                    uniqueIds.Add(Guid.Parse(uniqueIdString));
                }
                catch
                {
                    //ingore
                }
            }

            var entityList = this._dataService.GetMany(uniqueIds, include, exclude, fields, deletedItems);

            SetupSerialization(entityList, include, exclude, fields);
            //this.AssignNavigationSerialization(include, entityList);
            //this.AssignSerializationFields(fields, entityList);
            return entityList;
        }

        /// <summary>
        /// Returns an associated collection belonging to the <typeparamref name="T"/> type.
        /// </summary>
        /// <param name="guid">The unique id of the entity</param>
        /// <param name="collectionName">The name of the associated collection parameter.</param>
        /// <param name="pageNumber">The page number. The number of pages is dependent on the displayer <see cref="itemsPerPage"/> amount.</param>
        /// <param name="itemsPerPage">How many items per page.</param>
        /// <param name="include">Included related complex types.</param>
        /// <param name="exclude">Included related complex types.</param>
        /// <param name="fields">Only select a specific set of fields.</param>
        /// <returns>A list of objects corresponding to the collection.</returns>
        [HttpGet]
        [CrudApiAction]
        [GenericRoute("{guid:guid}/{collectionName}", Order = 1000)]
        [ResponseType(typeof(object))] // unable to be determined at compile time
        public virtual async Task<IHttpActionResult> GetCollection(Guid guid, string collectionName, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            var collectionPropertyInfo = typeof(T).GetProperties().FirstOrDefault(ppi => String.Equals(ppi.Name, collectionName, StringComparison.CurrentCultureIgnoreCase));

            if (collectionPropertyInfo == null || !typeof(IEnumerable).IsAssignableFrom(collectionPropertyInfo.PropertyType))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("The request you've made was invalid. Check the request data.")
                });


            StringBuilder includeBuilder = new StringBuilder();
            includeBuilder.Append(collectionPropertyInfo.Name);

            if (!string.IsNullOrWhiteSpace(include))
            {
                string[] includes = include.Split(',', '|');

                foreach (string i in includes)
                {
                    includeBuilder.Append("," + collectionPropertyInfo.Name + "." + i);
                }
            }

            //var additionalInclude = string.IsNullOrWhiteSpace(include) ? collectionPropertyInfo.Name : collectionPropertyInfo.Name + "." + include;


            var entity = this._dataService.Get(guid, includeBuilder.ToString(), exclude, fields, deletedItems);

            if (entity == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            var associatedCollection = (collectionPropertyInfo.GetValue(entity) as IEnumerable<EntityBase>);

            if (associatedCollection == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            var filteredAssociatedCollection = associatedCollection.Where(e => !e.IsDeleted);

            foreach (var assoficatedEntity in filteredAssociatedCollection)
            {
                ClearProperty(assoficatedEntity, entity); // remove the root property so serialization loop dose not happen 

                //this.AssignNavigationSerialization(include, assoficatedEntity);
                //this.AssignSerializationFields(fields, assoficatedEntity);
                assoficatedEntity.SetupSerialization(include, exclude, fields);
            }

            return this.Ok(filteredAssociatedCollection);
        }


        /// <summary>
        /// Returns deleted entities IDs.
        /// </summary>
        /// <param name="request">The request oject containing the IDs list from which to filter the deleted ones.</param>
        /// <returns>The collection of IDs for the deleted entities.</returns>
        [HttpPost]
        [CrudApiAction]
        [GenericRoute("~/api/v1/deleted/{controller}")]
        [ResponseType(typeof(IDsListWrapperRequest))]
        public virtual async Task<IDsListWrapperRequest> GetDeletedIDs(IDsListWrapperRequest request)
        {
            request.IDs = this._dataService.GetDeletedIDs(request.IDs).ToList();
            return request;
        }

        /// <summary>
        /// The post method adds a new entity of type T.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The persisted item and OK if the operation is successful. Returns InternalServerError if the operation has failed or BadRequest if the request is faulty.</returns>
        [HttpPost]
        [CrudApiAction]
        [GenericRoute("")]
        public virtual async Task<T> Post(T item)
        {
            if (item == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("The request you've made was invalid. Check the request data.")
                });

            //first add a new guid to the item
            if (item.Guid == Guid.Empty)
                item.Guid = Guid.NewGuid();

            T itemData;

            try
            {
                string include = PrepareEntityForNavigationPropertiesAttachment(item);

                itemData = this._dataService.Insert(item);

                itemData.SetupSerialization(include, null, null);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }

            if (itemData.SaveResult == null)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Service Saved, but Service Failed to return a SaveResult.  This is a bad practice.  Fix It"));
            }

            if (!itemData.SaveResult.Success)
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, itemData.SaveResult.Exception + " " + itemData.SaveResult.ExceptionMessage));

            return itemData;
        }

        /// <summary>
        /// The post method adds a list of new entity of type T.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>The persisted items and OK if the operation is successful. Returns InternalServerError if the operation has failed or BadRequest if the request is faulty.</returns>
        [HttpPost]
        [CrudApiAction]
        [GenericRoute("bulk", Order = 10)]
        public virtual async Task<ICollection<T>> PostMany(List<T> items)
        {
            if (items == null || !items.Any())
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("The request you've made was invalid. Check the request data.")
                });

            //first add a new guid to the item
            foreach (var item in items)
                if (item.Guid == Guid.Empty)
                    item.Guid = Guid.NewGuid();

            ICollection<T> itemData;

            try
            {
                itemData = this._dataService.InsertMany(items);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex + " " + ex.InnerException)
                });
            }

            if (itemData.Any(i => i.SaveResult == null))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Service Saved, but Service Failed to return a SaveResult.  This is a bad practice.  Fix It")
                });
            }

            if (itemData.Any(i => !i.SaveResult.Success))
            {
                var exceptions = new HashSet<string>();
                var exceptionMessage = new List<string>();

                foreach (var failedItems in itemData.Where(i => !i.SaveResult.Success))
                {
                    if (!exceptions.Contains(failedItems.SaveResult.Exception))
                        exceptions.Add(failedItems.SaveResult.Exception);

                    exceptionMessage.Add(failedItems.SaveResult.ExceptionMessage);
                }

                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(exceptions.Aggregate((i, j) => i + " | " + j) + " Messages: " + exceptionMessage.Aggregate((i, j) => i + " | " + j))
                });
            }

            return itemData;
        }

        /// <summary>
        /// Delete a entity.
        /// </summary>
        /// <param name="item">The object to delete.</param>
        /// <returns>Returns Accepted if the operation is successful or InternalServerError if the object could not be deleted or BadRequest if the request is faulty.</returns>
        [HttpDelete]
        [CrudApiAction]
        [GenericRoute("")]
        public virtual async Task<HttpResponseMessage> Delete(T item)
        {
            return await DeleteByGuid(item.Guid);
        }

        /// <summary>
        /// Delete a entity.
        /// </summary>
        /// <param name="guid">The unique id of the object to delete.</param>
        /// <returns>Returns Accepted if the operation is successful or InternalServerError if the object could not be deleted or BadRequest if the request is faulty.</returns>
        [HttpDelete]
        [CrudApiAction]
        [GenericRoute("delete/{guid:guid}")]
        public virtual async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            try
            {
                SaveResult result = this._dataService.Delete(guid);

                if (!result.Success)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(result.Exception + " " + result.ExceptionMessage)
                    });

                return Request.CreateResponse(HttpStatusCode.Accepted, result);
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex + " " + ex.InnerException)
                });
            }
        }

        /// <summary>
        /// Using POST because RestKit won't allow DELETE with body
        /// </summary>
        /// <param name="req">Because of RestKit (who cannot send a plain array) we need to wrap the list in an object.</param>
        /// <returns></returns>
        [HttpPost]
        [CrudApiAction]
        [GenericRoute("deletemany")]
        [ResponseType(typeof(ICollection<SaveResult>))]
        public virtual async Task<HttpResponseMessage> DeleteMany([FromBody]IDsListWrapperRequest req)
        {
            if (req == null || req.IDs == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            try
            {
                var results = this._dataService.DeleteMany(req.IDs.ToArray());

                var failure = results
                                .Where(r => !r.Success)
                                .ToList();
                if (failure.Count > 0)
                {
                    var msg = string.Join("\r\n\r\n", failure.Select(f => f.Exception + " " + f.ExceptionMessage));

                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(msg)
                    });
                }
                return Request.CreateResponse(HttpStatusCode.Accepted, results);
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex + " " + ex.InnerException)
                });
            }
        }

        /// <summary>
        /// Update a existing entity by overwriting the data.
        /// </summary>
        /// <param name="item">The already persisted item to be overwritten.</param>
        /// <returns>Returns the updated entity and OK if success. Returns InternalServerError if the operation has failed or BadRequest if the request is faulty.</returns>
        [HttpPut]
        [CrudApiAction]
        [GenericRoute("")]
        public virtual async Task<T> Put(T item)
        {
            if (item == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("The request you've made was invalid. Check the request data.")
                });

            try
            {
                var include = PrepareEntityForNavigationPropertiesAttachment(item);

                var entity = this._dataService.Update(item);

                if (!entity.SaveResult.Success)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(entity.SaveResult.Exception + " " + entity.SaveResult.ExceptionMessage)
                    });

                entity.SetupSerialization(include, null, null);

                return entity;
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex + " " + ex.InnerException)
                });
            }
        }

        /// <summary>
        /// Performs a partial update of the entity.
        /// </summary>
        /// <param name="item">The entity cantoning just the modified data.</param>
        /// <returns>Returns the updated entity and OK if success. Returns InternalServerError if the operation has failed or BadRequest if the request is faulty.</returns>
        [HttpPatch]
        [CrudApiAction]
        [GenericRoute("")]
        public virtual async Task<T> Patch(Delta<T> item)
        {
            try
            {
                var itemEntity = item.GetEntity();

                var include = PrepareEntityForNavigationPropertiesAttachment(itemEntity);

                var entity = this._dataService.Get(itemEntity.Guid);

                if (entity == null)
                {
                    return await Post(itemEntity);
                }

                item.Patch(entity);
                entity = this._dataService.Update(entity);

                if (!entity.SaveResult.Success)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent(entity.SaveResult.Exception + " " + entity.SaveResult.ExceptionMessage)
                    });

                entity.SetupSerialization(include, null, null);

                return entity;
            }
            catch (HttpResponseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        [HttpPatch]
        [CrudApiAction]
        [GenericRoute("bulk")]
        public virtual async Task<ICollection<T>> PatchMany(List<Delta<T>> items)
        {
            try
            {
                var itemEntities = items.Select(i => i.GetEntity()).ToList();
                var includes = itemEntities.ToDictionary(i => i.Guid, i => PrepareEntityForNavigationPropertiesAttachment(i));

                var ids = itemEntities.Select(i => i.Guid).ToList();

                var entities = _dataService.GetMany(ids);
                var entitiesToInsert = new List<T>();

                foreach (var item in items)
                {
                    var itemEntity = item.GetEntity();

                    var entity = entities.FirstOrDefault(e => e.Guid == itemEntity.Guid);

                    if (entity != null)
                    {
                        item.Patch(entity);
                    }
                    else
                    {
                        entitiesToInsert.Add(itemEntity);
                    }
                }
                var result = _dataService
                                .UpdateMany(entities)
                                .ToList();

                if (entitiesToInsert.Count > 0)
                {
                    var insertedResult = _dataService.InsertMany(entitiesToInsert);
                    result.AddRange(insertedResult);
                }

                foreach (var item in result)
                {
                    if (!includes.ContainsKey(item.Guid))
                    {
                        continue;
                    }

                    var include = includes[item.Guid];
                    item.SetupSerialization(include, null, null);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// Clear references that point to the specific objects.
        /// </summary>
        /// <param name="entity">The object to have its references cleared</param>
        /// <param name="objects">Objects to clear</param>
        protected static void ClearProperty(object entity, params object[] objects)
        {
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (propertyInfo.PropertyType.IsValueType || !objects.Contains(propertyInfo.GetValue(entity)))
                    continue;

                propertyInfo.SetValue(entity, null, null);
            }
        }

        /// <summary>
        /// Remove properties from the specific object. Nullable become null. Non nullable become default.
        /// </summary>
        /// <param name="entity">The object to have its properties cleared</param>
        /// <param name="propertyNames">Properties to clear</param>
        protected static void ClearProperty(object entity, params string[] propertyNames)
        {
            foreach (var propertyInfo in entity.GetType().GetProperties())
            {
                if (!propertyNames.Contains(propertyInfo.Name))
                    continue;

                propertyInfo.SetValue(entity, null, null);
            }
        }

        protected void SetupSerialization<U>(ICollection<U> items, string include, string exclude, string fields) where U : DbEntity
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                item.SetupSerialization(include, exclude, fields);
            }
        }

        protected static string FilterStringFromQueryStrings(IEnumerable<KeyValuePair<string, string>> list)
        {
            var sb = new System.Text.StringBuilder();
            var reserved = new string[] { "deleteditems", "include", "fields", "pagenumber", "itemsperpage", "exclude" };

            foreach (KeyValuePair<string, string> kv in list)
            {
                if (!reserved.Contains(kv.Key.ToLower()))
                {
                    if (!string.IsNullOrWhiteSpace(kv.Key) && !(string.IsNullOrWhiteSpace(kv.Value)))
                    {
                        sb.AppendFormat("{0}={1}&", kv.Key, kv.Value);
                    }
                    else if (!string.IsNullOrWhiteSpace(kv.Key) && string.IsNullOrWhiteSpace(kv.Value))
                    {
                        sb.AppendFormat("{0}&", kv.Key);
                    }
                }
            }

            if (sb.Length > 0) sb = sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        protected async Task<ICollection<T>> GetEntitiesFiltered(string filters, bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "")
        {
            try
            {
                if (this._dataService == null)
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

                string queryFilters = FilterStringFromQueryStrings(Request.GetQueryNameValuePairs());

                if (!string.IsNullOrEmpty(queryFilters))
                {
                    filters = string.IsNullOrEmpty(filters) ? queryFilters : string.Format("{0}&{1}", filters, queryFilters);
                }

                // make sure filters is not null
                filters = filters ?? string.Empty;
                include = include ?? string.Empty;
                fields = fields ?? string.Empty;

                ICollection<T> entityList = this._dataService.List(deletedItems, pageNumber, itemsPerPage, filters, include, exclude, fields);

                //this.AssignNavigationSerialization(include, entityList);
                //this.AssignSerializationFields(fields, entityList);
                SetupSerialization(entityList, include, exclude, fields);
                return entityList;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.Message, ex);
                throw;
            }
        }

        protected virtual string PrepareEntityForNavigationPropertiesAttachment(T entity)
        {
            return null;
        }

        #endregion
    }
}