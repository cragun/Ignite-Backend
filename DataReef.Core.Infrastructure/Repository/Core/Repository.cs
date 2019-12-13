using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DataReef.Core.Attributes;
using DataReef.Core.Classes;

namespace DataReef.Core.Infrastructure.Repository.Core
{
    [Service(typeof(IRepository))]
    public class Repository : IRepository
    {
        protected readonly Func<IDataContextFactory> ContextFactory;
        protected readonly Func<IEntityInterceptorProvider> InterceptorProvider;
        protected DbContext DbContext;
        private readonly DbConnection _connection;

        public Repository(Func<IDataContextFactory> contextFactory, Func<IEntityInterceptorProvider> interceptorProvider)
        {
            ContextFactory = contextFactory;
            InterceptorProvider = interceptorProvider;
        }

        internal Repository(Func<IDataContextFactory> contextFactory, Func<IEntityInterceptorProvider> interceptorProvider, DbConnection connection)
        {
            ContextFactory = contextFactory;
            InterceptorProvider = interceptorProvider;
            _connection = connection;
        }

        public virtual IQueryable<T> Get<T>() where T : DbEntity
        {
            EnsureInitialized();
            var entityQuery = InterceptSelect(DbContext.Set<T>().OfType<T>().AsNoTracking());
            return entityQuery;
        }

        public void InvokeStoredProcedure(StoredProcedure storedProcedure)
        {
            EnsureInitialized();

            if (storedProcedure.CommandTimeout.HasValue)
                DbContext.Database.CommandTimeout = storedProcedure.CommandTimeout;

            if (storedProcedure.ReturnType == null)
            {
                var result = DbContext.Database.ExecuteSqlCommand(storedProcedure.SqlCommand, storedProcedure.SqlParams);
                storedProcedure.Read(result);
            }
            else
            {
                var result = DbContext.Database.SqlQuery(storedProcedure.ReturnType, storedProcedure.SqlCommand, storedProcedure.SqlParams);
                storedProcedure.Read(result);
            }

            DbContext.Database.CommandTimeout = null;
        }

        protected virtual void EnsureInitialized()
        {
            if (DbContext != null)
                return;

            var context = ContextFactory();

            DbContext = _connection != null ? context.GetDataContext(_connection) : context.GetDataContext();
        }

        protected virtual IQueryable<T> InterceptSelect<T>(IQueryable<T> query) where T : DbEntity
        {
            var selector = query;
            var interceptors = InterceptorProvider().GetInterceptors();

            foreach (var entityInterceptor in interceptors)
                selector = entityInterceptor.InterceptSelect(selector);

            return selector;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DbContext?.Dispose();
            }
        }

        public void SaveChanges()
        {
            DbContext?.SaveChanges();
        }
    }
}
