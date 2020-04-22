namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addnewfieldinpersonclocktime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonClockTime", "ClockMin", c => c.Long(nullable: false));
            AddColumn("dbo.PersonClockTime", "ClockHours", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonClockTime", "ClockHours");
            DropColumn("dbo.PersonClockTime", "ClockMin");
        }
    }
}
