namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Delete_IsHiResImg : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.People", "IsHiResImg");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "IsHiResImg", c => c.Boolean(nullable: false));
        }
    }
}
