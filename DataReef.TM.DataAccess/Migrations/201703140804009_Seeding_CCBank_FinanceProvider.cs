namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Seeding_CCBank_FinanceProvider : DbMigration
    {
        public override void Up()
        {
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Seed_CCBank_FinancePlan_and_details.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            Sql(@"DELETE FROM finance.FinanceDetails
                    WHERE FinancePlanDefinitionID IN
	                    (SELECT Guid
	                    FROM finance.PlanDefinitions
	                    WHERE ProviderID = '24b8a5ec-3819-48c1-a4a5-d49a6041b56e')

                    DELETE FROM finance.OUAssociation
                    WHERE FinancePlanDefinitionID IN
	                    (SELECT Guid
	                    FROM finance.PlanDefinitions
	                    WHERE ProviderID = '24b8a5ec-3819-48c1-a4a5-d49a6041b56e')

                    DELETE FROM finance.PlanDefinitions
                    WHERE ProviderID = '24b8a5ec-3819-48c1-a4a5-d49a6041b56e'

                    DELETE FROM finance.Providers
                    WHERE Guid = '24b8a5ec-3819-48c1-a4a5-d49a6041b56e'");
        }
    }
}
