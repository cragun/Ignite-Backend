namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PropertyUsageProfileInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "GenabilityProfileID", c => c.String());
            AddColumn("dbo.Properties", "GenabilityProviderProfileID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "GenabilityProviderProfileID");
            DropColumn("dbo.Properties", "GenabilityProfileID");
        }
    }
}
