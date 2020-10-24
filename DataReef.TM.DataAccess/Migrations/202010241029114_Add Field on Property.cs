namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldonProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "SunnovaLeadID", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "SunnovaLeadID");
        }
    }
}
