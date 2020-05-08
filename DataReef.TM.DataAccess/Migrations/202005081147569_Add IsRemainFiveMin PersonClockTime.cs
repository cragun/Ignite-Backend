namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsRemainFiveMinPersonClockTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PersonClockTime", "IsRemainFiveMin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonClockTime", "IsRemainFiveMin");
        }
    }
}
