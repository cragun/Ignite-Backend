namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ObstructionsModel_CenterUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.RoofPlaneObstructions", "CenterPointX", c => c.Single());
            AddColumn("solar.RoofPlaneObstructions", "CenterPointY", c => c.Single());
            DropColumn("solar.RoofPlaneObstructions", "CenterID");
        }
        
        public override void Down()
        {
            AddColumn("solar.RoofPlaneObstructions", "CenterID", c => c.Guid());
            DropColumn("solar.RoofPlaneObstructions", "CenterPointY");
            DropColumn("solar.RoofPlaneObstructions", "CenterPointX");
        }
    }
}
