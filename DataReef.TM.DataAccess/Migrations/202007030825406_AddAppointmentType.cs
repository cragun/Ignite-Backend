namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAppointmentType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "AppointmentType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "AppointmentType");
        }
    }
}
