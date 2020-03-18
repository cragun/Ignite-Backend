namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveOnlineAppointmentParam : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Properties", "onlineAppointment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Properties", "onlineAppointment", c => c.Boolean(nullable: false));
        }
    }
}
