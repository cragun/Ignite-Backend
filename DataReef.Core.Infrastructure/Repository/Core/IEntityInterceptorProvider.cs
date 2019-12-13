using System.Collections.Generic;

namespace DataReef.Core.Infrastructure.Repository.Core
{
    public interface IEntityInterceptorProvider
    {
        IEnumerable<IEntityInterceptor> GetInterceptors();
    }
}
