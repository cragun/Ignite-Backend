namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MovedUsageProfileIDsToProposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "GenabilityElectricityProviderProfileID", c => c.String());
            AddColumn("solar.Proposals", "GenabilitySolarProviderProfileID", c => c.String());
            DropColumn("dbo.Properties", "GenabilityElectricityProfileID");
            DropColumn("dbo.Properties", "GenabilityElectricityProviderProfileID");
            DropColumn("dbo.Properties", "GenabilitySolarProfileID");
            DropColumn("dbo.Properties", "GenabilitySolarProviderProfileID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "GenabilitySolarProviderProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilitySolarProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilityElectricityProviderProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilityElectricityProfileID", c => c.String());
            DropColumn("solar.Proposals", "GenabilitySolarProviderProfileID");
            DropColumn("solar.Proposals", "GenabilityElectricityProviderProfileID");
        }
    }
}
