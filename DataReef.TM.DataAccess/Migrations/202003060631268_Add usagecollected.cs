namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addusagecollected : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "UsageCollected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "UsageCollected");
        }
    }
}
