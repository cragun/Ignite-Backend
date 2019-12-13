namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class GenabilitySolarProviderProfileID : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Arrays", "GenabilitySolarProviderProfileID", c => c.String());
            DropColumn("solar.Proposals", "GenabilitySolarProviderProfileID");
            DropColumn("solar.Proposals", "SalesRepAvgUtilityCost");

            Sql(@"update solar.FinancePlans set PPARequestJSON = STUFF(PPARequestJSON, PATINDEX('%""SolarProviderProfileID"":""%', PPARequestJSON) + 71, 1, '""]') where PPARequestJSON like '%""SolarProviderProfileID"":""%'");
            Sql(@"update solar.FinancePlans set PPARequestJSON = REPLACE(PPARequestJSON, '""SolarProviderProfileID"":""', '""SolarProviderProfileIDs"":[""') where PPARequestJSON like '%""SolarProviderProfileID"":""%'");
            Sql(@"update solar.FinancePlans set PPARequestJSON = REPLACE(PPARequestJSON, '""SolarProviderProfileID"":', '""SolarProviderProfileIDs"":') where PPARequestJSON like '%""SolarProviderProfileID"":%'");
        }
        
        public override void Down()
        {
            AddColumn("solar.Proposals", "SalesRepAvgUtilityCost", c => c.Single(nullable: false));
            AddColumn("solar.Proposals", "GenabilitySolarProviderProfileID", c => c.String());
            DropColumn("solar.Arrays", "GenabilitySolarProviderProfileID");
        }
    }
}
