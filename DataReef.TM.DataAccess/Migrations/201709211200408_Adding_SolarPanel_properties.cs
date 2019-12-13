namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_SolarPanel_properties : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "IsSolarThermal", c => c.Boolean(nullable: false));
            AddColumn("solar.Panels", "SolarType", c => c.String(maxLength: 50));
            AddColumn("solar.Panels", "PanelColor", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("solar.Panels", "PanelColor");
            DropColumn("solar.Panels", "SolarType");
            DropColumn("solar.AdderItems", "IsSolarThermal");
        }
    }
}
