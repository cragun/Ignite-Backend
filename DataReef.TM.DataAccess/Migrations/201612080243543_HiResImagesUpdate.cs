namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HiResImagesUpdate : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.HighResolutionImages", "idx_hires_coords");
            AddColumn("dbo.HighResolutionImages", "MapUnitsPerPixelX", c => c.Double(nullable: false));
            AddColumn("dbo.HighResolutionImages", "MapUnitsPerPixelY", c => c.Double(nullable: false));
            AddColumn("dbo.HighResolutionImages", "SkewX", c => c.Double(nullable: false));
            AddColumn("dbo.HighResolutionImages", "SkewY", c => c.Double(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Top", c => c.Double(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Left", c => c.Double(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Bottom", c => c.Double(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Right", c => c.Double(nullable: false));
            CreateIndex("dbo.HighResolutionImages", new[] { "Top", "Left", "Bottom", "Right" }, name: "idx_hires_coords");
        }
        
        public override void Down()
        {
            DropIndex("dbo.HighResolutionImages", "idx_hires_coords");
            AlterColumn("dbo.HighResolutionImages", "Right", c => c.Single(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Bottom", c => c.Single(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Left", c => c.Single(nullable: false));
            AlterColumn("dbo.HighResolutionImages", "Top", c => c.Single(nullable: false));
            DropColumn("dbo.HighResolutionImages", "SkewY");
            DropColumn("dbo.HighResolutionImages", "SkewX");
            DropColumn("dbo.HighResolutionImages", "MapUnitsPerPixelY");
            DropColumn("dbo.HighResolutionImages", "MapUnitsPerPixelX");
            CreateIndex("dbo.HighResolutionImages", new[] { "Top", "Left", "Bottom", "Right" }, name: "idx_hires_coords");
        }
    }
}
