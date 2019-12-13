namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FinancePlanChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.FinancePlans", "SolarProviderType", c => c.Int(nullable: false));

            Sql("UPDATE solar.FinancePlans SET SolarProviderType = 1 WHERE SolarProviderType = 0 OR SolarProviderType IS NULL");

            RenameColumn("solar.FinancePlans", "SunEdisonPricingQuoteID", "PricingQuoteID");
            RenameColumn("solar.FinancePlans", "EnvelopeID", "ContractID");
        }
        
        public override void Down()
        {
            RenameColumn("solar.FinancePlans", "PricingQuoteID", "SunEdisonPricingQuoteID");
            RenameColumn("solar.FinancePlans", "ContractID", "EnvelopeID");
            DropColumn("solar.FinancePlans", "SolarProviderType");
        }
    }
}
