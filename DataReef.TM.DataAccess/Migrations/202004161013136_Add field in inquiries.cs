namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addfieldininquiries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inquiries", "StartDate", c => c.DateTime());
            AddColumn("dbo.Inquiries", "EndDate", c => c.DateTime());
            AddColumn("dbo.Inquiries", "ClockDiff", c => c.Long(nullable: false));
            AddColumn("dbo.Inquiries", "ClockType", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inquiries", "ClockType");
            DropColumn("dbo.Inquiries", "ClockDiff");
            DropColumn("dbo.Inquiries", "EndDate");
            DropColumn("dbo.Inquiries", "StartDate");
        }
    }
}
