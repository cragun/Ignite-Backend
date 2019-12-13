using DataReef.TM.Models;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DataReef.TM.DataAccess.Prototype
{
    public interface IRepository
    {
        IQueryable<T> GetEntities<T>() where T : EntityBase;

        DbEntityEntry<T> GetEntry<T>(T entity) where T : EntityBase;
    }
}
