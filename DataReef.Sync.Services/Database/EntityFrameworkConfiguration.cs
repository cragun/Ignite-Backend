using DataReef.Core.Infrastructure.DatabaseDesign;
using System.Data.Entity;

namespace DataReef.Sync.Services.Database
{
    public class EntityFrameworkConfiguration : DbConfiguration
    {
        public EntityFrameworkConfiguration()
        {
            //Add the non-clustered suport for Guid PK 
            SetMigrationSqlGenerator("System.Data.SqlClient", () => new NonClusteredPrimaryKeySqlMigrationSqlGenerator());
        }
    }
}