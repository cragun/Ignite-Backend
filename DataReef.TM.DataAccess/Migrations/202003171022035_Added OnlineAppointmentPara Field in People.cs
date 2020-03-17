namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOnlineAppointmentParaFieldinPeople : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "OnlineAppointmentPara", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "OnlineAppointmentPara");
        }
    }
}
