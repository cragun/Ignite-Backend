namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTimeZoneonAppointment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Appointments", "TimeZone", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Appointments", "TimeZone");
        }
    }
}
