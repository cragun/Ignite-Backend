namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShapesVersioning : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "ShapesVersion", c => c.Int(nullable: false));
            AddColumn("dbo.Territories", "ShapesVersion", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Territories", "ShapesVersion");
            DropColumn("dbo.OUs", "ShapesVersion");
        }
    }
}
