namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Updating_Cash_FinanceDetails : DbMigration
    {
        public override void Up()
        {
            // Removing the cash detail for 1 month. Updating the remaining detail to be 18 months and order 0.
            Sql(@"DELETE 
                FROM finance.FinanceDetails
                WHERE FinancePlanDefinitionID IN (SELECT Guid FROM finance.PlanDefinitions WHERE Name = 'Cash')
                AND [Months] = 1

                UPDATE finance.FinanceDetails
                set Months = 18, [Order] = 0
                WHERE FinancePlanDefinitionID IN (SELECT Guid FROM finance.PlanDefinitions WHERE Name = 'Cash')");
        }

        public override void Down()
        {
        }
    }
}
