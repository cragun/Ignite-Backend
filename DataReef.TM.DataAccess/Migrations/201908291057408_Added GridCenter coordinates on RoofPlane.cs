namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGridCentercoordinatesonRoofPlane : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.RoofPlanes", "GridCenterX", c => c.Double(nullable: false));
            AddColumn("solar.RoofPlanes", "GridCenterY", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.RoofPlanes", "GridCenterY");
            DropColumn("solar.RoofPlanes", "GridCenterX");
        }
    }
}
