using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using DataReef.Core.Attributes;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.DataAccess.Database;

namespace DataReef.TM.DataAccess.Repository
{
    [Service(typeof(IDataContextFactory), ServiceScope.Application)]
    internal class TmDataContextFactory : IDataContextFactory
    {
        public DbConnection GetConnection()
        {
            var connectionString = GetConnectionString();
            return new SqlConnection(connectionString);
        }

        public DbContext GetDataContext()
        {
            var connectionString = GetConnectionString();
            return new DataContext(connectionString);
        }

        public DbContext GetDataContext(DbConnection connection)
        {
            return new DataContext(connection);
        }

        public string GetConnectionString()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["ConnectionStringName"]].ToString();

            return connectionString;
        }
    }
}
