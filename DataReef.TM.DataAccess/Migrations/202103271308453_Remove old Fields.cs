namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveoldFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Appointments", "JobNimbusID");
            DropColumn("dbo.Appointments", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "PropertyType");
            DropColumn("dbo.Properties", "JobNimbusLeadID");
            DropColumn("dbo.Properties", "PropertyType");
            DropColumn("dbo.PropertyNotes", "JobNimbusID");
            DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            DropColumn("dbo.PropertyNotes", "PropertyType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PropertyNotes", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.PropertyNotes", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.PropertyNotes", "JobNimbusID", c => c.String());
            AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "JobNimbusLeadID", c => c.String(maxLength: 200));
            AddColumn("dbo.Appointments", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Appointments", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.Appointments", "JobNimbusID", c => c.String());
        }
    }
}
