namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedElectricityAndSolarProfilesToProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "GenabilityElectricityProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilityElectricityProviderProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilitySolarProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilitySolarProviderProfileID", c => c.String());
            DropColumn("dbo.Properties", "GenabilityProfileID");
            DropColumn("dbo.Properties", "GenabilityProviderProfileID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "GenabilityProviderProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilityProfileID", c => c.String());
            DropColumn("dbo.Properties", "GenabilitySolarProviderProfileID");
            DropColumn("dbo.Properties", "GenabilitySolarProfileID");
            DropColumn("dbo.Properties", "GenabilityElectricityProviderProfileID");
            DropColumn("dbo.Properties", "GenabilityElectricityProfileID");
        }
    }
}
