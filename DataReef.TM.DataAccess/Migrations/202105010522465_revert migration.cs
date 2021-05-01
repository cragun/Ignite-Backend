namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class revertmigration : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            //DropColumn("dbo.PropertyNotes", "JobNimbusID");
            //DropColumn("dbo.Inquiries", "SalesRepName");
            //DropColumn("dbo.Properties", "JobNimbusLeadID");
            //DropColumn("dbo.Appointments", "JobNimbusLeadID");
            //DropColumn("dbo.Appointments", "JobNimbusID");
            //DropColumn("dbo.PropertyNotes", "PropertyType");
            //DropColumn("dbo.Properties", "PropertyType");
            //DropColumn("dbo.Appointments", "PropertyType");
            //DropColumn("finance.PlanDefinitions", "TermExternalID");

            //AddColumn("dbo.Properties", "ReferenceId", c => c.String(maxLength: 200));

            //DropColumn("dbo.Properties", "NoteReferenceId");

            //DropColumn("dbo.Properties", "SunnovaContactsResponse");

            

            //DropForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties");
            //DropIndex("dbo.Notifications", new[] { "PropertyID" });
            //DropColumn("dbo.Notifications", "PropertyID");
            
        }
        
        public override void Down()
        {
            //AddColumn("dbo.Appointments", "JobNimbusID", c => c.String());
            //AddColumn("dbo.Appointments", "JobNimbusLeadID", c => c.String());
            //AddColumn("dbo.Properties", "JobNimbusLeadID", c => c.String(maxLength: 200));
            //AddColumn("dbo.Inquiries", "SalesRepName", c => c.String(maxLength: 100));
            //AddColumn("dbo.PropertyNotes", "JobNimbusID", c => c.String());
            //AddColumn("dbo.PropertyNotes", "JobNimbusLeadID", c => c.String());
            //AddColumn("dbo.Appointments", "PropertyType", c => c.Int(nullable: false));
            //AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            //AddColumn("dbo.PropertyNotes", "PropertyType", c => c.Int(nullable: false));
            //AddColumn("finance.PlanDefinitions", "TermExternalID", c => c.Int(nullable: false));

            //DropColumn("dbo.Properties", "ReferenceId");

            //AddColumn("dbo.Properties", "NoteReferenceId", c => c.String(maxLength: 150));

            //AddColumn("dbo.Properties", "SunnovaContactsResponse", c => c.String());

            //AddColumn("dbo.Notifications", "PropertyID", c => c.Guid(nullable: false));

            //CreateIndex("dbo.Notifications", "PropertyID");
            //AddForeignKey("dbo.Notifications", "PropertyID", "dbo.Properties", "Guid");
        }
    }
}
