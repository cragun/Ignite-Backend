using EntityFramework.Extensions;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DataReef.TM.DataAccess.Prototype
{
    public class ReferenceLoader : IDisposable
    {
        private IQueryable lastQuery;
        private Repository repo;
        private AuthorizationFiltersProvider authorizationProvider;

        public ReferenceLoader(Repository repo, AuthorizationFiltersProvider authorizationProvider)
        {
            this.repo = repo;
            this.authorizationProvider = authorizationProvider;
        }
        public void Dispose()
        {
            if (lastQuery != null)
                lastQuery.Load();
        }     

        public ReferenceLoader LoadReference<TEntity, TReference>(IQueryable<TEntity> entityCollection, Expression<Func<TEntity, TReference>> navigationProperty)
            where TEntity : EntityBase
            where TReference : class
        {
            var filter = authorizationProvider.GetEntityFilter<TReference>();

            foreach (var entity in entityCollection)
            {
                lastQuery = repo.GetEntry(entity).Reference(navigationProperty).Query().Where(filter).Future().AsQueryable();
            }
            return this;
        }

        //public ReferenceLoader LoadReference<TEntity>(IQueryable<TEntity> entityCollection, string navigationProperty)
        //    where TEntity : EntityBase
        //{

        //    var filter = authorizationProvider.GetEntityFilter<TReference>();

        //    foreach (var entity in entityCollection)
        //    {
        //        lastQuery = repo.GetEntry(entity).Reference<TReference>(navigationProperty).Query().Where(filter).Future().AsQueryable();
        //    }
        //    return this;
        //}

        public ReferenceLoader LoadCollection<TEntity, TReference>(IQueryable<TEntity> entityCollection, Expression<Func<TEntity, ICollection<TReference>>> collectionProperty)
            where TEntity : EntityBase
            where TReference : class
        {
            var filter = authorizationProvider.GetEntityFilter<TReference>();            
            foreach (var entity in entityCollection)
            {
                var localQuery = repo.GetEntry(entity).Collection(collectionProperty).Query().Where(filter);
                lastQuery = localQuery.Future().AsQueryable();                
            }
            return this;
        }


        public ReferenceLoader LoadCollection<TEntity>(IQueryable<TEntity> entityCollection, string collectionProperty)
            where TEntity : EntityBase
        {
            var property = typeof(TEntity).GetProperty(collectionProperty);

            var parameter = Expression.Parameter(typeof(TEntity));
            var navigationProperty = Expression.Property(parameter, property);
            var lambdaExpression = Expression.Lambda(navigationProperty, parameter);

            MethodInfo method = typeof(ReferenceLoader).GetMethods().Where(m => m.IsGenericMethod && m.Name.Equals("LoadCollection")).First();

            MethodInfo generic = method.MakeGenericMethod(new[] { typeof(TEntity), property.PropertyType.GenericTypeArguments[0] });

            //Expression<Func<Invoice, ICollection<Charge>>> test = i => i.InvoiceDetails;
            //generic.Invoke(this, new object[] { entityCollection, test });
            return this;
        }

    }
}
