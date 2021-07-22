namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_IsHiResImg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "IsHiResImg", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "IsHiResImg");
        }
    }
}
