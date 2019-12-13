using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DataReef.TM.DataAccess.Prototype
{    

    //TODO: Prototype
    public class Repository : IRepository
    {
        private DataContext dataContext;
        private AuthorizationFiltersProvider authProvider;       
        public Repository(AuthorizationFiltersProvider authProvider)
        {
            this.dataContext = new DataContext();
            this.authProvider = authProvider;
        }
        public IQueryable<T> GetEntities<T>() where T : EntityBase
        {
            var authorizationFilter = authProvider.GetEntityFilter<T>();

            IQueryable<T> set = dataContext.Set<T>().Where(authorizationFilter);
            return new AuthorizationQueryable<T>(set, this);
        }

        public DbEntityEntry<T> GetEntry<T>(T entity) where T : EntityBase
        {
            return dataContext.Entry(entity);
        }

        public TEntity Add<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            //todo:  if(entity.IsNew)
            //else -> Update            
            //attach to context
            var e = dataContext.Set<TEntity>().Attach(entity);

            var authorization = authProvider.GetEntityFilter<TEntity>();

            //var allEntities = dataContext.Set<TEntity>().Where(authorization).Select(p => p.Guid).Contains(e.Guid);

            var isAuthorized = authorization.Compile().Invoke(entity);
            if (isAuthorized)
            {
                dataContext.Set<TEntity>().Add(e);
                dataContext.SaveChanges();
                return e;
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
