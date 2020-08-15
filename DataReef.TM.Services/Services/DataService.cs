using DataReef.Core.Attributes;
using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Core.Services;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DataService<T> : ServiceBase<T>, IDataService<T> where T : EntityBase
    {
        protected readonly Func<IUnitOfWork> UnitOfWorkFactory;

        public DataService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
        }

        public virtual ICollection<T> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            using (var repository = UnitOfWorkFactory())
            {
                IQueryable<T> setQuery = repository.Get<T>();

                AssignIncludes(include, ref setQuery);
                AssignFilters(filter, ref setQuery);
                setQuery = ApplyDeletedFilter(deletedItems, setQuery);

                if (itemsPerPage > 0)
                {
                    setQuery = setQuery.OrderBy(i => i.Name).Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage);
                }

                IList<T> returnData = setQuery.ToList();

                if (!deletedItems)
                {
                    returnData = RemoveDeletedItems(returnData);
                }

                return returnData;
            }
        }

        public virtual T Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            using (var repository = UnitOfWorkFactory())
            {
                IQueryable<T> setQuery = repository.Get<T>();

                AssignIncludes(include, ref setQuery);

                var returnData = setQuery.FirstOrDefault(qq => qq.Guid == uniqueId);

                if (!deletedItems)
                {
                    RemoveDeletedItems(returnData);
                }

                return returnData;
            }
        }

        public virtual ICollection<T> GetMany(IEnumerable<Guid> uniqueIds, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            using (var repository = UnitOfWorkFactory())
            {
                IQueryable<T> setQuery = repository.Get<T>();
                AssignIncludes(include, ref setQuery);

                IList<T> returnData = setQuery.Where(qq => uniqueIds.Contains(qq.Guid)).ToList();

                if (!deletedItems)
                {
                    RemoveDeletedItems(returnData);
                }

                return returnData;
            }
        }

        public virtual ICollection<Guid> GetDeletedIDs(IEnumerable<Guid> uniqueIds)
        {
            return GetMany(uniqueIds).Where(e => e.IsDeleted).Select(e => e.Guid).ToList();
        }

        //  TODO: this method should be removed
        /// <summary>
        /// There are scenarios when we want to do operations across multiple services in one transaction.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public virtual T Insert(T entity, DataContext dataContext)
        {
            try
            {
                //TODO: MAJOR take this from Smart

                if (SmartPrincipal.IsValid)
                {
                    Guid personID = SmartPrincipal.UserId;
                    entity.CreatedByID = personID;
                }

                dataContext.Set<T>().Add(entity);
                dataContext.SaveChanges();

                entity.SaveResult = SaveResult.SuccessfulInsert;

                return entity;
            }
            catch (DbEntityValidationException ex)
            {
                SaveResult errorResult = SaveResult.FromException(ex, DataAction.Insert);

                if (ex.EntityValidationErrors.Any())
                {
                    errorResult.ExceptionMessage += " [Validation Errors] ";
                    foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
                    {
                        errorResult.ExceptionMessage += "[ ";
                        errorResult.ExceptionMessage += "ENTITY: " + dbEntityValidationResult.Entry.Entity + " ---> PROPERTIES: ";
                        foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                            errorResult.ExceptionMessage += dbValidationError.PropertyName + " - " + dbValidationError.ErrorMessage + " | ";

                        errorResult.ExceptionMessage += " ]";
                    }
                }

                entity.SaveResult = errorResult;
                return entity;
            }
            catch (Exception ex)
            {
                SaveResult errorResult = SaveResult.FromException(ex, DataAction.Insert);
                entity.SaveResult = errorResult;
                return entity;
            }
        }

        public virtual T Insert(T entity)
        {
            return InsertMany(new[] { entity }).FirstOrDefault();
        }

        public virtual ICollection<T> InsertMany(ICollection<T> entities)
        {
            if (entities?.Any() != true)
            {
                return entities;
            }

            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    foreach (var entity in entities)
                    {
                        uow.Add(entity);
                    }

                    uow.SaveChanges(new DataSaveOperationContext
                    {
                        DataAction = DataAction.Insert
                    });

                    foreach (var entity in entities)
                        entity.SaveResult = SaveResult.SuccessfulInsert;

                    return entities;
                }
            }
            catch (DbEntityValidationException ex)
            {
                SaveResult errorResult = SaveResult.FromException(ex, DataAction.Insert);
                SaveResult setErrorResult = SaveResult.FromException(new Exception("Failed due to another entity that was in this query set."), DataAction.Insert);

                if (ex.EntityValidationErrors.Any())
                {
                    errorResult.ExceptionMessage += " [Validation Errors] ";
                    foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
                    {
                        errorResult.ExceptionMessage += "[ ";
                        errorResult.ExceptionMessage += "ENTITY: " + dbEntityValidationResult.Entry.Entity + " ---> PROPERTIES: ";
                        foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                            errorResult.ExceptionMessage += dbValidationError.PropertyName + " - " + dbValidationError.ErrorMessage + " | ";

                        errorResult.ExceptionMessage += " ]";
                    }
                }

                var problematicEntities = ex.EntityValidationErrors.Select(e => e.Entry.Entity).ToList();

                foreach (var entity in entities)
                    entity.SaveResult = problematicEntities.Contains(entity) ? errorResult : setErrorResult;

                return entities;
            }
            catch (Exception ex)
            {
                SaveResult errorResult = SaveResult.FromException(ex, DataAction.Insert);

                foreach (var entity in entities)
                    entity.SaveResult = errorResult;

                return entities;
            }
        }

        //  This method should be removed
        public virtual U Update<U>(U entity, DataContext dataContext, bool saveContext = true) where U : EntityBase
        {
            bool releaseDataContext = dataContext == null;
            try
            {
                dataContext = dataContext ?? new DataContext();

                dataContext.Set<U>().Attach(entity);

                var state = entity.Id == 0 ? EntityState.Added : EntityState.Modified;

                if (state == EntityState.Added)
                {
                    // this should cover the Independent Association cases used in Solar models
                    // there's a change this Guid might be in the DB.
                    // we want to make sure it's not to 
                    if (dataContext.Set<U>().Any(u => u.Guid == entity.Guid))
                    {
                        state = EntityState.Modified;
                    }
                }

                dataContext.Entry(entity).State = state;
                entity.Updated(SmartPrincipal.UserId);
                if (saveContext)
                {
                    dataContext.SaveChanges();
                }
                entity.SaveResult = SaveResult.SuccessfulUpdate;

                // TODO: move to separate class or common functionality method
                //this.Process(entity, DataAction.Update);

                return entity;
            }
            catch (Exception ex)
            {
                SaveResult errorResult = SaveResult.FromException(ex, DataAction.Update);
                entity.SaveResult = errorResult;
                return entity;
            }
            finally
            {
                if (releaseDataContext && dataContext != null) dataContext.Dispose();
            }
        }

        public virtual T Update(T entity)
        {
            return UpdateMany(new[] { entity }).FirstOrDefault();
        }

        public virtual ICollection<T> UpdateMany(ICollection<T> entities)
        {
            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    uow.UpdateMany(entities);

                    uow.SaveChanges(new DataSaveOperationContext
                    {
                        DataAction = DataAction.Update
                    });

                    foreach (var entity in entities)
                    {
                        entity.SaveResult = SaveResult.SuccessfulUpdate;
                    }

                    return entities;
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorResult = SaveResult.FromException(ex, DataAction.Insert);
                var setErrorResult = SaveResult.FromException(new Exception("Failed due to another entity that was in this query set."), DataAction.Update);

                if (ex.EntityValidationErrors.Any())
                {
                    errorResult.ExceptionMessage += " [Validation Errors] ";
                    foreach (var dbEntityValidationResult in ex.EntityValidationErrors)
                    {
                        errorResult.ExceptionMessage += "[ ";
                        errorResult.ExceptionMessage += "ENTITY: " + dbEntityValidationResult.Entry.Entity + " ---> PROPERTIES: ";
                        foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                            errorResult.ExceptionMessage += dbValidationError.PropertyName + " - " + dbValidationError.ErrorMessage + " | ";

                        errorResult.ExceptionMessage += " ]";
                    }
                }

                var problematicEntities = ex.EntityValidationErrors.Select(e => e.Entry.Entity).ToList();

                foreach (var entity in entities)
                    entity.SaveResult = problematicEntities.Contains(entity) ? errorResult : setErrorResult;

                return entities;
            }
            catch (Exception ex)
            {
                SaveResult errorResult = SaveResult.FromException(ex, DataAction.Update);

                foreach (var entity in entities)
                    entity.SaveResult = errorResult;

                return entities;
            }
        }

        //  this method should be removed
        public virtual T Update(T entity, DataContext dataContext)
        {
            return Update<T>(entity, dataContext);
        }

        protected string UpdateNavigationProperties<U>(U entity, string propParent = "", DataContext dataContext = null, bool saveContext = true) where U : EntityBase
        {
            var attachedProperties = typeof(U)
                                .GetProperties()
                                .Where(p => Attribute.IsDefined(p, typeof(AttachOnUpdateAttribute)))
                                .ToList();

            if (attachedProperties.Count == 0)
            {
                return string.Empty;
            }

            var include = new List<string>();

            foreach (var property in attachedProperties)
            {
                var propType = property.GetType();
                var propValue = property.GetValue(entity);

                var propArray = propValue as object[];

                if (propValue != null && (propArray == null || propArray.Length > 0))
                {
                    include.Add(string.IsNullOrEmpty(propParent) ? property.Name : string.Format("{0}.{1}", propParent, property.Name));
                }

                var entityType = property.PropertyType.GetGenericArguments().FirstOrDefault();

                if (entityType == null)
                {
                    throw new ApplicationException("Make sure you only add the AttachOnUpdate Attribute to Collections<T>!");
                }

                MethodInfo method = this.GetType().GetMethod("AttachNavigationProperties");
                MethodInfo generic = method.MakeGenericMethod(entityType);
                generic.Invoke(this, new object[] { propValue, dataContext, saveContext });
            }

            return string.Join(",", include);
        }

        protected string UpdateNavigationProperties(T entity)
        {
            return UpdateNavigationProperties<T>(entity);
        }

        public virtual SaveResult Delete(Guid uniqueId)
        {
            return DeleteMany(new[] { uniqueId }).FirstOrDefault();
        }

        public virtual ICollection<SaveResult> DeleteMany(Guid[] uniqueIds)
        {
            var results = new List<SaveResult>();

            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    var entities = uow.Get<T>().Where(eb => uniqueIds.Contains(eb.Guid)).ToArray();

                    foreach (var uniqueId in uniqueIds)
                    {
                        var entity = entities.FirstOrDefault(e => e.Guid == uniqueId);
                        var result = TryEntitySoftDelete(uniqueId, entity);
                        if (entity != null)
                        {
                            OnDelete(entity);
                        }

                        results.Add(result);
                    }

                    uow.SaveChanges(new DataSaveOperationContext
                    {
                        DataAction = DataAction.Delete
                    });

                    OnDeletedMany(entities);
                }
            }
            catch (Exception ex)
            {
                foreach (var saveResult in results)
                    saveResult.Success = false;

                results.Add(SaveResult.FromException(ex, DataAction.Delete));
            }

            return results;
        }

        public virtual SaveResult Activate(Guid uniqueId)
        {
            return ActivateMany(new[] { uniqueId }).FirstOrDefault();
        }

        public virtual ICollection<SaveResult> ActivateMany(Guid[] uniqueIds)
        {
            var results = new List<SaveResult>();

            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    var entities = uow.Get<T>().Where(eb => uniqueIds.Contains(eb.Guid)).ToArray();

                    foreach (var uniqueId in uniqueIds)
                    {
                        var entity = entities.FirstOrDefault(e => e.Guid == uniqueId);
                        var result = TryEntitySoftActivate(uniqueId, entity);
                        if (entity != null)
                        {
                            OnActivate(entity);
                        }

                        results.Add(result);
                    }

                    uow.SaveChanges(new DataSaveOperationContext
                    {
                        DataAction = DataAction.Update
                    });

                    OnActivatedMany(entities);
                }
            }
            catch (Exception ex)
            {
                foreach (var saveResult in results)
                    saveResult.Success = false;

                results.Add(SaveResult.FromException(ex, DataAction.Update));
            }

            return results;
        }

        public void AttachNavigationProperties<U>(ICollection<U> properties, DataContext dc = null, bool saveContext = true) where U : EntityBase
        {
            if (properties == null || properties.Count == 0)
                return;

            var needToCreateDataContext = dc == null;

            if (needToCreateDataContext)
            {
                dc = new DataContext();
            }

            var propertyIds = properties.Select(p => p.Guid);

            var existingIds = dc
                                .Set<U>()
                                .Where(u => propertyIds.Contains(u.Guid))
                                .Select(u => u.Guid)
                                .ToList();

            foreach (var item in properties)
            {
                dc.Set<U>().Attach(item);

                var state = existingIds.Contains(item.Guid) ? EntityState.Modified : EntityState.Added;

                dc.Entry(item).State = state;

                if (state == EntityState.Modified)
                {
                    item.Version++;
                }
                if (saveContext)
                {
                    try
                    {
                        dc.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        dc.Entry(item).State = EntityState.Unchanged;
                    }
                }

                // there are cases, when a nav property needs to update children navigation properties
                UpdateNavigationProperties(item, string.Empty, dc, saveContext);

                // we need to verify the existing again .. because the previous method might add it to the DB.
                existingIds = dc
                                .Set<U>()
                                .Where(u => propertyIds.Contains(u.Guid))
                                .Select(u => u.Guid)
                                .ToList();
            }

            if (needToCreateDataContext)
            {
                dc.Dispose();
            }
        }

        protected virtual void OnDelete(T entity)
        { }

        protected virtual void OnDeletedMany(T[] entities)
        { }

        protected static SaveResult TryEntitySoftDelete(Guid uniqueId, T entity)
        {
            SaveResult result;
            if (entity != null)
            {
                entity.IsDeleted = true;

                result = SaveResult.SuccessfulDeletion;
                result.EntityUniqueId = uniqueId;
            }
            else
            {
                result = new SaveResult
                {
                    Action = DataAction.None,
                    EntityUniqueId = uniqueId,
                    Success = false
                };
            }

            return result;
        }

        protected virtual void OnActivate(T entity)
        { }

        protected virtual void OnActivatedMany(T[] entities)
        { }
        protected static SaveResult TryEntitySoftActivate(Guid uniqueId, T entity)
        {
            SaveResult result;
            if (entity != null)
            {
                entity.IsDeleted = false;

                result = SaveResult.SuccessfulUpdate;
                result.EntityUniqueId = uniqueId;
            }
            else
            {
                result = new SaveResult
                {
                    Action = DataAction.None,
                    EntityUniqueId = uniqueId,
                    Success = false
                };
            }

            return result;
        }

        protected List<Guid> Exist(ICollection<T> entities, DataContext context = null)
        {
            if (entities?.Any() != true)
            {
                return null;
            }

            var needToDispose = context == null;
            context = context ?? new DataContext();

            var ids = entities.Select(e => e.Guid).ToList();

            var result = context
                            .Set<T>()
                            .Where(item => ids.Contains(item.Guid))
                            .Select(e => e.Guid).ToList();
            if (needToDispose)
            {
                context.Dispose();
            }
            return result;
        }
    }
}
