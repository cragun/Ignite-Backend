namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOnlineAppointmentParaFieldinPeople : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Appointments", "IsOnline");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Appointments", "IsOnline", c => c.Boolean(nullable: false));
        }
    }
}
