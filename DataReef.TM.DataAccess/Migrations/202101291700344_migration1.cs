namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "ModifiedTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "ModifiedTime");
        }
    }
}
