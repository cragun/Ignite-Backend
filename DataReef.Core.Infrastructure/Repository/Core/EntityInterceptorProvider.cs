using System.Collections.Generic;
using System.Linq;
using DataReef.Core.Attributes;

namespace DataReef.Core.Infrastructure.Repository.Core
{
    [Service(typeof(IEntityInterceptorProvider), ServiceScope.Application)]
    internal class EntityInterceptorProvider : IEntityInterceptorProvider
    {
        private readonly IEnumerable<IEntityInterceptor> _entityInterceptors;

        public EntityInterceptorProvider(IEntityInterceptor[] entityInterceptors)
        {
            _entityInterceptors = entityInterceptors;
        }

        public IEnumerable<IEntityInterceptor> GetInterceptors()
        {
            return _entityInterceptors ?? Enumerable.Empty<IEntityInterceptor>();
        }
    }
}
