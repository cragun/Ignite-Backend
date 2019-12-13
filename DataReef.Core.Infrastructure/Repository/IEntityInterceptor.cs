using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DataReef.Core.Classes;

namespace DataReef.Core.Infrastructure.Repository
{
    public enum InterceptorType
    {
        None = 0,
        DataFilter = 1
    }

    public interface IEntityInterceptor
    {
        /// <summary>
        /// Intercepts the select of queries.
        /// </summary>
        /// <returns>The new version of the query.</returns>
        IQueryable<T> InterceptSelect<T>(IQueryable<T> query) where T : DbEntity;

        /// <summary>
        /// Intercepts added entities before save.
        /// </summary>
        void InterceptAdded(DbEntityEntry entry, DataSaveOperationContext dataSaveOperationContext);

        /// <summary>
        /// Intercepts updated entities.
        /// </summary>
        void InterceptUpdated(DbEntityEntry entry, DataSaveOperationContext dataSaveOperationContext);

        /// <summary>
        /// Intercepts modified entities after save.
        /// </summary>
        void InterceptPostSave(IEnumerable<EntityAndState> modifiedEntities, DbContext dbContext);

        /// <summary>
        /// Interceptor priority, lower executes first.
        /// </summary>
        int Priority { get; }

        InterceptorType Type { get; }
    }
}
