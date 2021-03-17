namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remainfields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            DropColumn("dbo.PropertyNotes", "JobNimbusID");
            DropColumn("dbo.Inquiries", "SalesRepName");
            DropColumn("dbo.Properties", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusID");
        }
        
        public override void Down()
        {
        }
    }
}
