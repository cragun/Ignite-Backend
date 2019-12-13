using System.Data.Common;
using System.Data.Entity;

namespace DataReef.Core.Infrastructure.Repository
{
    public interface IDataContextFactory
    {
        DbConnection GetConnection();

        DbContext GetDataContext();

        DbContext GetDataContext(DbConnection connection);

        string GetConnectionString();
    }
}
