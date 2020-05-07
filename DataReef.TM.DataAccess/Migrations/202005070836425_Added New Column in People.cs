namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNewColumninPeople : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "LastLoginTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.People", "LastActivityTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.People", "LastActivityName", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "LastActivityName");
            DropColumn("dbo.People", "LastActivityTime");
            DropColumn("dbo.People", "LastLoginTime");
        }
    }
}
