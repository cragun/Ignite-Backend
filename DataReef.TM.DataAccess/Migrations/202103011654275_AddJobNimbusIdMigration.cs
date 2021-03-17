namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddJobNimbusIdMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "JobNimbusID", c => c.String());
            AddColumn("dbo.Appointments", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.PropertyNotes", "JobNimbusID", c => c.String());
            AddColumn("dbo.PropertyNotes", "JobNimbusLeadID", c => c.String());
            AddColumn("dbo.QuotasCommitments", "IsCommitmentSet", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.QuotasCommitments", "IsCommitmentSet");
            DropColumn("dbo.PropertyNotes", "JobNimbusLeadID");
            DropColumn("dbo.PropertyNotes", "JobNimbusID");
            DropColumn("dbo.Appointments", "JobNimbusLeadID");
            DropColumn("dbo.Appointments", "JobNimbusID");
        }
    }
}
