namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_IntegrationProvider_and_Apr_To_FinancePlanDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.PlanDefinitions", "IntegrationProvider", c => c.Int(nullable: false));
            AddColumn("finance.PlanDefinitions", "Apr", c => c.Single(nullable: false));

            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/2017-12-06_01_01_Add_LoanPal_FinancePlans.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            DropColumn("finance.PlanDefinitions", "Apr");
            DropColumn("finance.PlanDefinitions", "IntegrationProvider");
        }
    }
}
