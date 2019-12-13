namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Leaving_Only_one_generic_mortgage_finance_plan : DbMigration
    {
        public override void Up()
        {
            Sql(@"DELETE FROM [finance].OuAssociation 
                  WHERE FinancePlanDefinitionId IN
	                (
		                SELECT [Guid]
		                FROM finance.PlanDefinitions
		                where Type = 3
	                )");

            Sql(@"DELETE FROM finance.PlanDefinitions 
                  WHERE Type = 3");

            var planID = Guid.NewGuid();

            // Insert a new Mortgage plan definition
            Sql($@"INSERT INTO finance.PlanDefinitions
                  (Guid, ProviderID, Type, IsDisabled, Name, TenantID, DateCreated, Version, IsDeleted, TermInYears)
                  VALUES
                  ('{planID}', (SELECT Guid FROM finance.Providers WHERE Name = 'PRMI'), 3, 0, 'PRMI Mortgage', 0, GETUTCDATE(), 0, 0, 0)");

            // create associations between the new plan and Solcius
            Sql($@"INSERT INTO finance.OuAssociation
                   (Guid, OUID, FinancePlanDefinitionId, TenantId, DateCreated, Version, IsDeleted)
                   VALUES
                   (NEWID(), (SELECT Guid FROM OUS WHERE Name = 'Solcius' AND IsDeleted = 0), '{planID}', 0, GETUTCDATE(), 0, 0)");

            // create associations between the new plan and DataReef Solar / Solar #1
            Sql($@"INSERT INTO finance.OuAssociation
                   (Guid, OUID, FinancePlanDefinitionId, TenantId, DateCreated, Version, IsDeleted)
                   VALUES
                   (NEWID(), (SELECT Guid FROM OUS WHERE Name = 'Solar #1' AND IsDeleted = 0), '{planID}', 0, GETUTCDATE(), 0, 0)");
        }

        public override void Down()
        {
        }
    }
}
