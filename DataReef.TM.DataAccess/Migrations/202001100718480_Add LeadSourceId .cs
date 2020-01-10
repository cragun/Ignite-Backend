namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLeadSourceId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "LeadSourceId", c => c.Int());
            DropColumn("dbo.Properties", "LeadSource");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "LeadSource", c => c.String(maxLength: 200));
            DropColumn("dbo.Properties", "LeadSourceId");
        }
    }
}
