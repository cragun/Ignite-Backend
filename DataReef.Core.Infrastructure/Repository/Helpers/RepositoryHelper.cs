using System;
using DataReef.Core.Infrastructure.Repository.Core;
using Microsoft.Practices.ServiceLocation;

namespace DataReef.Core.Infrastructure.Repository.Helpers
{
    //  TODO: remove this class
    public static class RepositoryHelper
    {
        public static IRepository CreateRepository()
        {
            var contextFactory = ServiceLocator.Current.GetInstance<Func<IDataContextFactory>>();
            var interceptorProvider = ServiceLocator.Current.GetInstance<Func<IEntityInterceptorProvider>>();

            return new Core.Repository(contextFactory, interceptorProvider);
        }

        public static IUnitOfWork CreateUnitOfWork()
        {
            var contextFactory = ServiceLocator.Current.GetInstance<Func<IDataContextFactory>>();
            var interceptorProvider = ServiceLocator.Current.GetInstance<Func<IEntityInterceptorProvider>>();

            return new UnitOfWork(contextFactory, interceptorProvider);
        }
    }
}
