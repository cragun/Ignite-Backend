namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class checkmigratewithlivedb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "SalesRepName", c => c.String(maxLength: 100));
            AddForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties");
            DropColumn("dbo.Inquiries", "SalesRepName");
        }
    }
}
