using System.Collections.Generic;
using System.Threading.Tasks;
using DataReef.Core.Classes;

namespace DataReef.Core.Infrastructure.Repository
{
    public interface IUnitOfWork : IRepository
    {
        void Add<T>(T entity) where T : DbEntity;

        void AddMany<T>(IEnumerable<T> entities) where T : DbEntity;

        void Update<T>(T entity) where T : DbEntity;

        void UpdateMany<T>(IEnumerable<T> entities) where T : DbEntity;

        void SaveChanges();

        Task SaveChangesAsync();

        void SaveChanges(DataSaveOperationContext dataSaveOperationContext);

        Task SaveChangesAsync(DataSaveOperationContext dataSaveOperationContext);

        void ExecuteSQL(string query);
    }
}
