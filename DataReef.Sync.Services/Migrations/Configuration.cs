using DataReef.Sync.Services.Database;
using System.Data.Entity.Migrations;

namespace DataReef.Sync.Services.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SyncDataContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(SyncDataContext context)
        {
        }
    }
}
