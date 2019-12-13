namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGoogleEventIDandStatusonAppointment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "GoogleEventID", c => c.String(maxLength: 250));
            AddColumn("dbo.Appointments", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "Status");
            DropColumn("dbo.Appointments", "GoogleEventID");
        }
    }
}
