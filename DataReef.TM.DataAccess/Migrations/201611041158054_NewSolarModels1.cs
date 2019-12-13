namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class NewSolarModels1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("solar.RoofPlanes", new[] { "SolarSystemID" });
            DropIndex("solar.RoofPlanes", new[] { "SolarSystem_Guid" });
            Sql("ALTER TABLE [solar].[RoofPlanes] DROP CONSTRAINT [FK_solar.RoofPlanes_solar.Proposals_SolarSystemID]");
            DropColumn("solar.RoofPlanes", "SolarSystemID");
            RenameColumn(table: "solar.RoofPlanes", name: "SolarSystem_Guid", newName: "SolarSystemID");
            AlterColumn("solar.RoofPlanes", "SolarSystemID", c => c.Guid(nullable: false));
            CreateIndex("solar.RoofPlanes", "SolarSystemID");
        }
        
        public override void Down()
        {
            DropIndex("solar.RoofPlanes", new[] { "SolarSystemID" });
            AlterColumn("solar.RoofPlanes", "SolarSystemID", c => c.Guid());
            RenameColumn(table: "solar.RoofPlanes", name: "SolarSystemID", newName: "SolarSystem_Guid");
            AddColumn("solar.RoofPlanes", "SolarSystemID", c => c.Guid(nullable: false));
            CreateIndex("solar.RoofPlanes", "SolarSystem_Guid");
            CreateIndex("solar.RoofPlanes", "SolarSystemID");
        }
    }
}
