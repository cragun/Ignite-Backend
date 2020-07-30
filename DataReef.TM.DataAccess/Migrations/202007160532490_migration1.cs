namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration1 : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Appointments", "AppointmentType", c => c.Int(nullable: false));
            //AddColumn("dbo.Users", "StartDate", c => c.DateTime());
            //AddColumn("FI.AdapterRequests", "Prefix", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("FI.AdapterRequests", "Prefix");
            //DropColumn("dbo.Users", "StartDate");
            //DropColumn("dbo.Appointments", "AppointmentType");
        }
    }
}
