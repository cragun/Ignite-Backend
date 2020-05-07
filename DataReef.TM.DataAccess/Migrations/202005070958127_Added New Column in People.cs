namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNewColumninPeople : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "LastLoginDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.People", "LastActivityDate", c => c.DateTime());
            AddColumn("dbo.People", "ActivityName", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "ActivityName");
            DropColumn("dbo.People", "LastActivityDate");
            DropColumn("dbo.People", "LastLoginDate");
        }
    }
}
