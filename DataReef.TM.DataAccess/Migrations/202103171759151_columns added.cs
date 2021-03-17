namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class columnsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "JobNimbusID", c => c.String());
            AddColumn("dbo.Appointments", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.Properties", "JobNimbusLeadID", c => c.String(maxLength: 200));
            AddColumn("dbo.Inquiries", "SalesRepName", c => c.String(maxLength: 100));
            AddColumn("dbo.PropertyNotes", "JobNimbusID", c => c.String());
            AddColumn("dbo.PropertyNotes", "JobNimbusLeadID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            DropColumn("dbo.PropertyNotes", "JobNimbusID");
            DropColumn("dbo.Inquiries", "SalesRepName");
            DropColumn("dbo.Properties", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusID");
        }
    }
}
