namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Increasing_Precision_on_Coordinates : DbMigration
    {
        public override void Up()
        {
            // deleting the default value constraint to be able to change the type
            this.DeleteDefaultContraint("solar.RoofPlanePanels", "TopLeftLat");
            this.DeleteDefaultContraint("solar.RoofPlanePanels", "TopLeftLon");
            this.DeleteDefaultContraint("solar.RoofPlanePanels", "BottomRightLat");
            this.DeleteDefaultContraint("solar.RoofPlanePanels", "BottomRightLon");

            AlterColumn("solar.RoofPlaneObstructions", "CenterPointX", c => c.Double());
            AlterColumn("solar.RoofPlaneObstructions", "CenterPointY", c => c.Double());
            AlterColumn("solar.RoofPlanePanels", "TopLeftLat", c => c.Double(nullable: false, defaultValue: 0));
            AlterColumn("solar.RoofPlanePanels", "TopLeftLon", c => c.Double(nullable: false, defaultValue: 0));
            AlterColumn("solar.RoofPlanePanels", "BottomRightLat", c => c.Double(nullable: false, defaultValue: 0));
            AlterColumn("solar.RoofPlanePanels", "BottomRightLon", c => c.Double(nullable: false, defaultValue: 0));
            AlterColumn("solar.Proposals", "Lat", c => c.Double(nullable: false));
            AlterColumn("solar.Proposals", "Lon", c => c.Double(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("solar.Proposals", "Lon", c => c.Single(nullable: false));
            AlterColumn("solar.Proposals", "Lat", c => c.Single(nullable: false));
            AlterColumn("solar.RoofPlanePanels", "BottomRightLon", c => c.Single(nullable: false));
            AlterColumn("solar.RoofPlanePanels", "BottomRightLat", c => c.Single(nullable: false));
            AlterColumn("solar.RoofPlanePanels", "TopLeftLon", c => c.Single(nullable: false));
            AlterColumn("solar.RoofPlanePanels", "TopLeftLat", c => c.Single(nullable: false));
            AlterColumn("solar.RoofPlaneObstructions", "CenterPointY", c => c.Single());
            AlterColumn("solar.RoofPlaneObstructions", "CenterPointX", c => c.Single());
        }
    }
}
