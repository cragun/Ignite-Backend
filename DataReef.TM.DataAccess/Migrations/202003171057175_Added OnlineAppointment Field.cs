namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOnlineAppointmentField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "onlineAppointment", c => c.Boolean(nullable: false));
            AddColumn("dbo.People", "OnlineAppointmentPara", c => c.String(maxLength: 10000));
        }
        
        public override void Down()
        {           
            DropColumn("dbo.Properties", "onlineAppointment");
            DropColumn("dbo.People", "OnlineAppointmentPara");
        }
    }
}
