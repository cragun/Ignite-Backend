namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_InterestType_and_PrincipalType_to_FinanceDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.FinanceDetails", "PrincipalType", c => c.Int(nullable: false));
            AddColumn("finance.FinanceDetails", "InterestType", c => c.Int(nullable: false));
            AddColumn("finance.FinanceDetails", "ApplyReductionAfterPeriod", c => c.Int(nullable: false));

            Sql(@"UPDATE [finance].[FinanceDetails] 
                  SET[PrincipalType] = PrincipalIsPaid,
                  [InterestType] = InterestIsPaid,
                  [ApplyReductionAfterPeriod] = ApplyPrincipalReductionAfter");

            Sql(@"INSERT INTO finance.FinanceDetails
                  (GUID, FinancePlanDefinitionID, Apr, [Order], PrincipalType,PrincipalIsPaid, ApplyPrincipalReductionAfter, ApplyReductionAfterPeriod , Months, Name, TenantID, DateCreated, Version, IsDeleted)
                  VALUES
                  (NEWID(), (SELECT [Guid] FROM finance.PlanDefinitions WHERE Name = 'Cash'), 0, 0, 2, 1, 0, 0, 1, 'Cash First Month', 0, GETUTCDATE(), 0, 0)

                  INSERT INTO finance.FinanceDetails
                  (GUID, FinancePlanDefinitionID, Apr, [Order], PrincipalType,PrincipalIsPaid, ApplyPrincipalReductionAfter,ApplyReductionAfterPeriod , Months, Name, TenantID, DateCreated, Version, IsDeleted)
                  VALUES
                  (NEWID(), (SELECT [Guid] FROM finance.PlanDefinitions WHERE Name = 'Cash'), 0, 1, 0, 0, 1, 2, 17, 'Cash Incentives', 0, GETUTCDATE(), 0, 0)");
        }

        public override void Down()
        {
            DropColumn("finance.FinanceDetails", "ApplyReductionAfterPeriod");
            DropColumn("finance.FinanceDetails", "InterestType");
            DropColumn("finance.FinanceDetails", "PrincipalType");
        }
    }
}
