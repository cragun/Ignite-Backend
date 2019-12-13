namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class FixFinancePlanSolarSystemRelationship : DbMigration
    {
        public override void Up()
        {
            DropIndex("solar.FinancePlans", new[] { "SolarSystem_Guid" });
            DropColumn("solar.FinancePlans", "SolarSystemID");
            RenameColumn(table: "solar.FinancePlans", name: "SolarSystem_Guid", newName: "SolarSystemID");
            AlterColumn("solar.FinancePlans", "SolarSystemID", c => c.Guid(nullable: false));
            CreateIndex("solar.FinancePlans", "SolarSystemID");
        }
        
        public override void Down()
        {
            DropIndex("solar.FinancePlans", new[] { "SolarSystemID" });
            AlterColumn("solar.FinancePlans", "SolarSystemID", c => c.Guid());
            RenameColumn(table: "solar.FinancePlans", name: "SolarSystemID", newName: "SolarSystem_Guid");
            AddColumn("solar.FinancePlans", "SolarSystemID", c => c.Guid(nullable: false));
            CreateIndex("solar.FinancePlans", "SolarSystem_Guid");
        }
    }
}
