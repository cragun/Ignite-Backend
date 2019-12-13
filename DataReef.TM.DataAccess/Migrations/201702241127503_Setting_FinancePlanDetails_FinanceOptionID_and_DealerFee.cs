namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Setting_FinancePlanDetails_FinanceOptionID_and_DealerFee : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.PlanDefinitions", "FinanceOptionId", c => c.Int());
            AddColumn("finance.PlanDefinitions", "DealerFee", c => c.Double());

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 201,
                  DealerFee = 10.0
                  WHERE Name = 'Spruce 10 / 3.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 147,
                  DealerFee = 13.5
                  WHERE Name = 'Spruce 20 / 4.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2183,
                  DealerFee = 14.75
                  WHERE Name = 'GreenSky 12 / 3.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2184,
                  DealerFee = 10.5
                  WHERE Name = 'GreenSky 12 / 4.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2185,
                  DealerFee = 6.75
                  WHERE Name = 'GreenSky 12 / 5.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2186,
                  DealerFee = 6.25
                  WHERE Name = 'GreenSky 12 / 6.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2189,
                  DealerFee = 4.5
                  WHERE Name = 'GreenSky 12 / 9.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2049,
                  DealerFee = 12.75
                  WHERE Name = 'GreenSky 20 / 4.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2059,
                  DealerFee = 8.75
                  WHERE Name = 'GreenSky 20 / 5.99%'");

            Sql(@"UPDATE finance.PlanDefinitions
                  SET FinanceOptionId = 2069,
                  DealerFee = 6.75
                  WHERE Name = 'GreenSky 20 / 6.99%'");
        }

        public override void Down()
        {
            DropColumn("finance.PlanDefinitions", "DealerFee");
            DropColumn("finance.PlanDefinitions", "FinanceOptionId");
        }
    }
}
