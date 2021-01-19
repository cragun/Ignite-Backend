namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addattachmentsfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "SunnovaLeadID", c => c.String(maxLength: 200));
            AddColumn("dbo.PropertyNotes", "Attachments", c => c.String());
            AlterColumn("dbo.People", "StartDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "StartDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.PropertyNotes", "Attachments");
            DropColumn("dbo.Properties", "SunnovaLeadID");
        }
    }
}
