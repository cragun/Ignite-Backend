namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarArrayFloatToDouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("solar.Arrays", "ModuleSpacing", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "RowSpacing", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "SolarArrayRotation", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "AnchorPointX", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "AnchorPointY", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "CenterX", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "CenterY", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "PanXOffset", c => c.Double(nullable: false));
            AlterColumn("solar.Arrays", "PanYOffset", c => c.Double(nullable: false));
            AlterColumn("solar.ArrayPanels", "X1", c => c.Double(nullable: false));
            AlterColumn("solar.ArrayPanels", "X2", c => c.Double(nullable: false));
            AlterColumn("solar.ArrayPanels", "Y1", c => c.Double(nullable: false));
            AlterColumn("solar.ArrayPanels", "Y2", c => c.Double(nullable: false));
            AlterColumn("solar.ArrayPanels", "CentroidX", c => c.Double(nullable: false));
            AlterColumn("solar.ArrayPanels", "CentroidY", c => c.Double(nullable: false));
            AlterColumn("solar.ArraySegments", "X", c => c.Double(nullable: false));
            AlterColumn("solar.ArraySegments", "Y", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("solar.ArraySegments", "Y", c => c.Single(nullable: false));
            AlterColumn("solar.ArraySegments", "X", c => c.Single(nullable: false));
            AlterColumn("solar.ArrayPanels", "CentroidY", c => c.Single(nullable: false));
            AlterColumn("solar.ArrayPanels", "CentroidX", c => c.Single(nullable: false));
            AlterColumn("solar.ArrayPanels", "Y2", c => c.Single(nullable: false));
            AlterColumn("solar.ArrayPanels", "Y1", c => c.Single(nullable: false));
            AlterColumn("solar.ArrayPanels", "X2", c => c.Single(nullable: false));
            AlterColumn("solar.ArrayPanels", "X1", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "PanYOffset", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "PanXOffset", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "CenterY", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "CenterX", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "AnchorPointY", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "AnchorPointX", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "SolarArrayRotation", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "RowSpacing", c => c.Single(nullable: false));
            AlterColumn("solar.Arrays", "ModuleSpacing", c => c.Single(nullable: false));
        }
    }
}
