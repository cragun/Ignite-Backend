namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveExtraFieldfrominquiries : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Inquiries", "StartDate");
            DropColumn("dbo.Inquiries", "EndDate");
            DropColumn("dbo.Inquiries", "ClockDiff");
            DropColumn("dbo.Inquiries", "ClockType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Inquiries", "ClockType", c => c.String(maxLength: 50));
            AddColumn("dbo.Inquiries", "ClockDiff", c => c.Long(nullable: false));
            AddColumn("dbo.Inquiries", "EndDate", c => c.DateTime());
            AddColumn("dbo.Inquiries", "StartDate", c => c.DateTime());
        }
    }
}
