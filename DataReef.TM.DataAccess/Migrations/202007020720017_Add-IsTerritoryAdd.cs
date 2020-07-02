namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsTerritoryAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "IsTerritoryAdd", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OUs", "IsTerritoryAdd");
        }
    }
}
