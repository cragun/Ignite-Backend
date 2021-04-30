namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addremainfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "JobNimbusID", c => c.String());
            AddColumn("dbo.Appointments", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.Appointments", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "SunnovaContactsResponse", c => c.String());
            AddColumn("dbo.Properties", "JobNimbusLeadID", c => c.String(maxLength: 200));
            AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "NoteReferenceId", c => c.String(maxLength: 150));
            AddColumn("dbo.PropertyNotes", "JobNimbusID", c => c.String());
            AddColumn("dbo.PropertyNotes", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.PropertyNotes", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Notifications", "PropertyID", c => c.Guid(nullable: false));
            CreateIndex("dbo.Notifications", "PropertyID");
            AddForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties");
            DropIndex("dbo.Notifications", new[] { "PropertyID" });
            DropColumn("dbo.Notifications", "PropertyID");
            DropColumn("dbo.PropertyNotes", "PropertyType");
            DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            DropColumn("dbo.PropertyNotes", "JobNimbusID");
            DropColumn("dbo.Properties", "NoteReferenceId");
            DropColumn("dbo.Properties", "PropertyType");
            DropColumn("dbo.Properties", "JobNimbusLeadID");
            DropColumn("dbo.Properties", "SunnovaContactsResponse");
            DropColumn("dbo.Appointments", "PropertyType");
            DropColumn("dbo.Appointments", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusID");
        }
    }
}
