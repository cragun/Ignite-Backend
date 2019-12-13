using System;
using System.Linq;
using DataReef.Core.Classes;

namespace DataReef.Core.Infrastructure.Repository
{
    public interface IRepository : IDisposable
    {
        IQueryable<T> Get<T>() where T : DbEntity;

        void InvokeStoredProcedure(StoredProcedure storedProcedure);

        void SaveChanges();
    }
}
