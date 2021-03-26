namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "JobNimbusID", c => c.String());
            AddColumn("dbo.Appointments", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.Appointments", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "JobNimbusLeadID", c => c.String(maxLength: 200));
            AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.PropertyNotes", "JobNimbusID", c => c.String());
            AddColumn("dbo.PropertyNotes", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.PropertyNotes", "PropertyType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyNotes", "PropertyType");
            DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            DropColumn("dbo.PropertyNotes", "JobNimbusID");
            DropColumn("dbo.Properties", "PropertyType");
            DropColumn("dbo.Properties", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "PropertyType");
            DropColumn("dbo.Appointments", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusID");
        }
    }
}
