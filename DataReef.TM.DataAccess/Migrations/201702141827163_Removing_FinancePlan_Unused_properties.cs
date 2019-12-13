namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Removing_FinancePlan_Unused_properties : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.FinancePlans", "FinanceProvider");
            DropColumn("solar.FinancePlans", "PeriodInMonths");

            Sql(@"UPDATE OUSettings
                  SET Value = REPLACE(Value, '{""3"":', '{""77034eac-e0c2-42ee-a7ba-b3fa280ed0c5"":')
                  WHERE Name IN ('AgreementTemplates', 'ProposalTemplates', 'ContractTemplates')");

            Sql("DELETE FROM OUSettings WHERE Name = 'SupportedFinancePlans'");
        }

        public override void Down()
        {
            AddColumn("solar.FinancePlans", "PeriodInMonths", c => c.Int(nullable: false));
            AddColumn("solar.FinancePlans", "FinanceProvider", c => c.Int(nullable: false));

            Sql(@"UPDATE OUSettings
                  SET Value = REPLACE(Value, '{""77034eac-e0c2-42ee-a7ba-b3fa280ed0c5"":', '{""3"":')
                  WHERE Name IN ('AgreementTemplates', 'ProposalTemplates', 'ContractTemplates')");
        }
    }
}
