namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGenabilityInfoToSolarSystem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "GenabilityTotalCost", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "GenabilityTotalConsumption", c => c.Double(nullable: false));
            AddColumn("solar.Systems", "GenabilityTieredAverageUtilityCost", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "GenabilityTieredAverageUtilityCost");
            DropColumn("solar.Systems", "GenabilityTotalConsumption");
            DropColumn("solar.Systems", "GenabilityTotalCost");
        }
    }
}
