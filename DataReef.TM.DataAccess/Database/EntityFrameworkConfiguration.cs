using DataReef.Core.Infrastructure.DatabaseDesign;
using DataReef.TM.DataAccess.Database.Interceptors;
using System.Data.Entity;

namespace DataReef.TM.DataAccess.Database
{
    public class EntityFrameworkConfiguration : DbConfiguration
    {
        public EntityFrameworkConfiguration()
        {
            //Add the non-clustered suport for Guid PK 
            SetMigrationSqlGenerator("System.Data.SqlClient", () => new NonClusteredPrimaryKeySqlMigrationSqlGenerator());

            //Add the interceptor to all EF queries.
            AddInterceptor(new SoftDeleteInterceptor());         
        }
    }
}