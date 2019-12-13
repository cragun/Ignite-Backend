namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoofPlanePanel_AddGeoCoordinates : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.RoofPlanePanels", "TopLeftLat", c => c.Single(nullable: false));
            AddColumn("solar.RoofPlanePanels", "TopLeftLon", c => c.Single(nullable: false));
            AddColumn("solar.RoofPlanePanels", "BottomRightLat", c => c.Single(nullable: false));
            AddColumn("solar.RoofPlanePanels", "BottomRightLon", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.RoofPlanePanels", "BottomRightLon");
            DropColumn("solar.RoofPlanePanels", "BottomRightLat");
            DropColumn("solar.RoofPlanePanels", "TopLeftLon");
            DropColumn("solar.RoofPlanePanels", "TopLeftLat");
        }
    }
}
