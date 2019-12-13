namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingIsArchivedforOUandTerritory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "IsArchived", c => c.Boolean(nullable: false));
            AddColumn("dbo.Territories", "IsArchived", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Territories", "IsArchived");
            DropColumn("dbo.OUs", "IsArchived");
        }
    }
}
