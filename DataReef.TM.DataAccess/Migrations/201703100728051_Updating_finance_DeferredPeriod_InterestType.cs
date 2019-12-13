namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Updating_finance_DeferredPeriod_InterestType : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE finance.FinanceDetails
                  SET InterestType = 2
                  WHERE Months = 2");
        }

        public override void Down()
        {
        }
    }
}
