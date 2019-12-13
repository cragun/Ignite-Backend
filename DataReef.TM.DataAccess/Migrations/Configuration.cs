using DataReef.TM.DataAccess.Database;
using System.Data.Entity.Migrations;

namespace DataReef.TM.DataAccess.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;

            // New timeout in seconds per command
            CommandTimeout = 60 * 5;
        }

        protected override void Seed(DataContext context)
        {
            //this should only be used when seeding a new database.
            //Seeds.SeedDevelopment(context);


            //stored procs, triggers, etc
            Seeds.PostProcess(context);

        }
    }
}
