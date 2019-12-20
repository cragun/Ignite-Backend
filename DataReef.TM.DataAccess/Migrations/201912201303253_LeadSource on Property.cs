namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LeadSourceonProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "LeadSource", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "LeadSource");
        }
    }
}
