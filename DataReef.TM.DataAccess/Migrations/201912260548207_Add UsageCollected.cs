namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsageCollected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "UsageCollected", c => c.Boolean(nullable: false));
            AddColumn("solar.ProposalsData", "UsageCollected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.ProposalsData", "UsageCollected");
            DropColumn("dbo.Properties", "UsageCollected");
        }

    }
}
