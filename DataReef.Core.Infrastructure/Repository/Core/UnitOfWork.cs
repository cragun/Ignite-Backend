using DataReef.Core.Attributes;
using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Exceptions;
using DataReef.Core.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Infrastructure.Repository.Core
{
    [Service(typeof(IUnitOfWork))]
    public class UnitOfWork : Repository, IUnitOfWork
    {
        public UnitOfWork(
            Func<IDataContextFactory> contextFactory,
            Func<IEntityInterceptorProvider> interceptorProvider) : base(contextFactory, interceptorProvider)
        {
        }

        public override IQueryable<T> Get<T>()
        {
            EnsureInitialized();
            var entityQuery = InterceptSelect(DbContext.Set<T>());

            return entityQuery;
        }

        public void Add<T>(T entity) where T : DbEntity
        {
            EnsureInitialized();
            DbContext.Set<T>().Add(entity);
        }

        public void AddMany<T>(IEnumerable<T> entities) where T : DbEntity
        {
            if (entities.All(e => e == null))
                return;

            EnsureInitialized();
            DbContext.Set<T>().AddRange(entities);
        }

        public void Update<T>(T entity) where T : DbEntity
        {
            EnsureInitialized();

            var entry = DbContext.Entry(entity);
            if (entry.State != EntityState.Detached)
                return;

            var set = DbContext.Set<T>().AsNoTracking();
            var dbEntity = set.FirstOrDefault(e => e.Guid == entity.Guid);
            if (dbEntity == null)
                throw new EntityNotFoundException(entity.Guid, typeof(T));

            entry.State = EntityState.Modified;
            entry.OriginalValues.SetValues(dbEntity);
        }

        public void UpdateMany<T>(IEnumerable<T> entities) where T : DbEntity
        {
            if (entities.All(e => e == null))
                return;

            EnsureInitialized();

            var entries = entities.Select(e => DbContext.Entry(e));
            var detachedEntities = entries.Where(e => e.State == EntityState.Detached).ToList();
            var detachedEntitiesIds = detachedEntities.Select(e => e.Entity.Guid);

            var set = DbContext.Set<T>();
            IEnumerable<T> databaseEntities = set.AsNoTracking().Where(e => detachedEntitiesIds.Contains(e.Guid)).ToList();

            foreach (var entry in detachedEntities)
            {
                var dbEntity = databaseEntities.FirstOrDefault(e => e.Guid == entry.Entity.Guid);
                if (dbEntity == null)
                    throw new EntityNotFoundException(entry.Entity.Guid, typeof(T));

                entry.State = EntityState.Modified;
                entry.OriginalValues.SetValues(dbEntity);
                entry?.Entity?.Updated(SmartPrincipal.UserId);
                // For some reason, when doing a PUT, the Id does not get copied, and the save fails.
                if (entry.Entity.Id != dbEntity.Id)
                {
                    entry.Entity.Id = dbEntity.Id;
                }
            }
        }

        public void SaveChanges()
        {
            Task.Run(() => SaveChangesAsync()).GetAwaiter().GetResult();
        }

        public async Task SaveChangesAsync()
        {
            await SaveChangesAsync(new DataSaveOperationContext
            {
                DataAction = DataAction.None
            });
        }

        public void SaveChanges(DataSaveOperationContext dataSaveOperationContext)
        {
            Task.Run(() => SaveChangesAsync(dataSaveOperationContext)).GetAwaiter().GetResult();
        }

        public async Task SaveChangesAsync(DataSaveOperationContext dataSaveOperationContext)
        {
            EnsureInitialized();

            var addedEntities = DbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            var updatedEntities = DbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);

            var interceptors = InterceptorProvider().GetInterceptors().OrderBy(e => e.Priority).ToList();

            InterceptAdded(interceptors, addedEntities.ToArray(), dataSaveOperationContext);
            InterceptUpdated(interceptors, updatedEntities.ToArray(), dataSaveOperationContext);

            var modifiedEntities = DbContext.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)
                .Select(e => new EntityAndState { Entity = e.Entity, EntityState = e.State.ToDataAction() })
                .ToArray();

            try
            {
                await DbContext.SaveChangesAsync();
            }
            catch (DbEntityValidationException validationException)
            {
                var validationErrorMessage = new StringBuilder();

                foreach (var dbEntityValidationResult in validationException.EntityValidationErrors)
                {
                    if (dbEntityValidationResult.IsValid)
                        continue;

                    validationErrorMessage.AppendLine("---->   Entity validation error: " + dbEntityValidationResult.Entry.Entity.GetType().Name);

                    foreach (var validationError in dbEntityValidationResult.ValidationErrors)
                        validationErrorMessage.AppendLine(validationError.PropertyName + ": " + validationError.ErrorMessage);
                }

                throw new DbEntityValidationException(validationErrorMessage.ToString(), validationException);
            }
            catch(Exception ex)
            {

            }

            InterceptPostSave(interceptors, modifiedEntities);
        }

        protected virtual void InterceptAdded(ICollection<IEntityInterceptor> interceptors, IEnumerable<DbEntityEntry> entities, DataSaveOperationContext dataSaveOperationContext)
        {
            foreach (var entity in entities)
            {
                foreach (var entityInterceptor in interceptors)
                    entityInterceptor.InterceptAdded(entity, dataSaveOperationContext);

                // It seems that EF has "an issue" when someone changes an entry states manually and another entity is sharing the same complex property and that prop is a FK (like a GUID). Calling detect changes will mitigate that.
                DbContext.ChangeTracker.DetectChanges();
            }
        }

        protected virtual void InterceptUpdated(ICollection<IEntityInterceptor> interceptors, IEnumerable<DbEntityEntry> entitys, DataSaveOperationContext dataSaveOperationContext)
        {
            foreach (var entity in entitys)
            {
                foreach (var entityInterceptor in interceptors)
                    entityInterceptor.InterceptUpdated(entity, dataSaveOperationContext);

                // It seems that EF has "an issue" when someone changes an entry states manually and another entity is sharing the same complex property and that prop is a FK (like a GUID). Calling detect changes will mitigate that.
                DbContext.ChangeTracker.DetectChanges();
            }
        }

        protected virtual void InterceptPostSave(ICollection<IEntityInterceptor> interceptors, ICollection<EntityAndState> modifiedEntities)
        {
            Task.Factory.StartNew(() =>
            {
                var context = ContextFactory();

                var dbContext = context.GetDataContext();

                foreach (var interceptor in interceptors)
                    interceptor.InterceptPostSave(modifiedEntities, dbContext);

            }, TaskCreationOptions.PreferFairness)
            .ContinueWith(t =>
            {
                var exceptions = t?.Exception?.Flatten().InnerExceptions;
                if (exceptions == null)
                    return;

                foreach (var exception in exceptions)
                    Trace.TraceError(exception.ToString());

            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void ExecuteSQL(string query)
        {
            ContextFactory()
                    .GetDataContext()
                    .Database
                    .ExecuteSqlCommand(query);
        }
    }
}
