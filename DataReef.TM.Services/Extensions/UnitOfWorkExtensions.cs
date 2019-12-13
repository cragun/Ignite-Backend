using System.Collections.Generic;
using System.Linq;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.Models;

namespace DataReef.TM.Services.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static void Delete<T>(this IUnitOfWork uow, T entity) where T : EntityBase
        {
            entity.IsDeleted = true;
            uow.Update(entity);
        }

        public static void DeleteMany<T>(this IUnitOfWork uow, IEnumerable<T> entities) where T : EntityBase
        {
            if (entities == null || !entities.Any())
                return;

            entities.ToList().ForEach(uow.Delete);
        }
    }
}
