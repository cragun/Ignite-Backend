using System.Data.Common;
using System.Data.Entity;
using Microsoft.WindowsAzure;
using DataReef.Sync.Contracts.Models;

namespace DataReef.Sync.Services.Database
{
    public class SyncDataContext : DbContext
    {
        public SyncDataContext(DbConnection dbConnection)
            : base(dbConnection, false)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public SyncDataContext() : base(CloudConfigurationManager.GetSetting("ConnectionStringName"))
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<SyncSession> SyncSessions { get; set; }
        public DbSet<Delta> Deltas { get; set; }
    }
}
