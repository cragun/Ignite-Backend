namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RoofPlaneChanges1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("solar.RoofPlaneEdges", "StartPointX", "StartPointLatitude");
            RenameColumn("solar.RoofPlaneEdges", "StartPointY", "StartPointLongitude");
            RenameColumn("solar.RoofPlaneEdges", "EndPointX", "EndPointLatitude");
            RenameColumn("solar.RoofPlaneEdges", "EndPointY", "EndPointLongitude");
            RenameColumn("solar.RoofPlanePoints", "X", "Latitude");
            RenameColumn("solar.RoofPlanePoints", "Y", "Longitude");
        }
        
        public override void Down()
        {
            RenameColumn("solar.RoofPlaneEdges", "StartPointLatitude", "StartPointX");
            RenameColumn("solar.RoofPlaneEdges", "StartPointLongitude", "StartPointY");
            RenameColumn("solar.RoofPlaneEdges", "EndPointLatitude", "EndPointX");
            RenameColumn("solar.RoofPlaneEdges", "EndPointLongitude", "EndPointY");
            RenameColumn("solar.RoofPlanePoints", "Latitude", "X");
            RenameColumn("solar.RoofPlanePoints", "Longitude", "Y");
        }
    }
}
