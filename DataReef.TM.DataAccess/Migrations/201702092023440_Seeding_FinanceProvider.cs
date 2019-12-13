namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Seeding_FinanceProvider : DbMigration
    {
        public override void Up()
        {
            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Seed_FinanceProvider_Data.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            Sql(@"DELETE FROM [finance].[OUAssociation]
                  DELETE FROM [finance].[FinanceDetails]
                  DELETE FROM [finance].[planDefinitions]
                  DELETE FROM [finance].[Providers]");
        }
    }
}
