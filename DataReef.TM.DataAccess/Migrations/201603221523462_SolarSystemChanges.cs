namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarSystemChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("solar.ArrayPanels", "InverterID", "solar.Inverters");
            DropForeignKey("solar.ArrayPanels", "SolarPanelID", "solar.Panels");
            DropIndex("solar.ArrayPanels", new[] { "SolarPanelID" });
            DropIndex("solar.ArrayPanels", new[] { "InverterID" });
            DropColumn("solar.Systems", "DefaultInverterID");
            DropColumn("solar.Systems", "DefaultSolarPanelID");
            DropColumn("solar.ArrayPanels", "SolarPanelID");
            DropColumn("solar.ArrayPanels", "InverterID");
        }
        
        public override void Down()
        {
            AddColumn("solar.ArrayPanels", "InverterID", c => c.Guid());
            AddColumn("solar.ArrayPanels", "SolarPanelID", c => c.Guid(nullable: false));
            AddColumn("solar.Systems", "DefaultSolarPanelID", c => c.Guid(nullable: false));
            AddColumn("solar.Systems", "DefaultInverterID", c => c.Guid(nullable: false));
            CreateIndex("solar.ArrayPanels", "InverterID");
            CreateIndex("solar.ArrayPanels", "SolarPanelID");
            AddForeignKey("solar.ArrayPanels", "SolarPanelID", "solar.Panels", "Guid");
            AddForeignKey("solar.ArrayPanels", "InverterID", "solar.Inverters", "Guid");
        }
    }
}
