namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMinModule : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "MinModule", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUs", "MinModule");
        }
    }
}
