namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingFinancePlanDefinitionMetaDataJSONproperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.PlanDefinitions", "MetaDataJSON", c => c.String());

            Sql("UPDATE finance.PlanDefinitions SET MetaDataJSON = '{\"Data\":{\"Integrations\":{\"LoanPal\":{\"OptionID\":1}}}}' WHERE Name = 'LoanPal 10 / 3.99%' ");
            Sql("UPDATE finance.PlanDefinitions SET MetaDataJSON = '{\"Data\":{\"Integrations\":{\"LoanPal\":{\"OptionID\":2}}}}' WHERE Name = 'LoanPal 20 / 4.99%' ");
            Sql("UPDATE finance.PlanDefinitions SET MetaDataJSON = '{\"Data\":{\"Integrations\":{\"LoanPal\":{\"OptionID\":3}}}}' WHERE Name = 'LoanPal 10 / 2.99%' ");
            Sql("UPDATE finance.PlanDefinitions SET MetaDataJSON = '{\"Data\":{\"Integrations\":{\"LoanPal\":{\"OptionID\":4}}}}' WHERE Name = 'LoanPal 20 / 3.99%' ");
            Sql("UPDATE finance.PlanDefinitions SET MetaDataJSON = '{\"Data\":{\"Integrations\":{\"LoanPal\":{\"OptionID\":5}}}}' WHERE Name = 'LoanPal 20 / 5.99%' ");
        }

        public override void Down()
        {
            DropColumn("finance.PlanDefinitions", "MetaDataJSON");
        }
    }
}
