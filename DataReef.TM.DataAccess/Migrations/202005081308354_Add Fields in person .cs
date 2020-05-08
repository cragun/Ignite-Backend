namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsinperson : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "LastLoginDate", c => c.DateTime());
            AddColumn("dbo.People", "LastActivityDate", c => c.DateTime());
            AddColumn("dbo.People", "ActivityName", c => c.String(maxLength: 250));
            AddColumn("dbo.PersonClockTime", "IsRemainFiveMin", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PersonClockTime", "IsRemainFiveMin");
            DropColumn("dbo.People", "ActivityName");
            DropColumn("dbo.People", "LastActivityDate");
            DropColumn("dbo.People", "LastLoginDate");
        }
    }
}
