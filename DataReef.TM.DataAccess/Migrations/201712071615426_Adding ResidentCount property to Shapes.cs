namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingResidentCountpropertytoShapes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TerritoryShapes", "ResidentCount", c => c.Long(nullable: false));
            AddColumn("dbo.OUShapes", "ResidentCount", c => c.Long(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.OUShapes", "ResidentCount");
            DropColumn("dbo.TerritoryShapes", "ResidentCount");
        }
    }
}
