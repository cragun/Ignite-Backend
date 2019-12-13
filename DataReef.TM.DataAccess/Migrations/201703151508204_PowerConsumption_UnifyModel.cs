namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PowerConsumption_UnifyModel : DbMigration
    {
        public override void Up()
        {
            DropIndex("solar.PowerConsumption", new[] { "SolarSystemID" });
            AddColumn("solar.PowerConsumption", "PropertyID", c => c.Guid());
            AddColumn("solar.PowerConsumption", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("solar.PowerConsumption", "SolarSystemID", c => c.Guid());
            CreateIndex("solar.PowerConsumption", "PropertyID");
            CreateIndex("solar.PowerConsumption", "SolarSystemID");
            AddForeignKey("solar.PowerConsumption", "PropertyID", "dbo.Properties", "Guid");
            Sql("update solar.PowerConsumption set Discriminator='SolarSystemPowerConsumption'");
        }
        
        public override void Down()
        {
            DropForeignKey("solar.PowerConsumption", "PropertyID", "dbo.Properties");
            DropIndex("solar.PowerConsumption", new[] { "SolarSystemID" });
            DropIndex("solar.PowerConsumption", new[] { "PropertyID" });
            AlterColumn("solar.PowerConsumption", "SolarSystemID", c => c.Guid(nullable: false));
            DropColumn("solar.PowerConsumption", "Discriminator");
            DropColumn("solar.PowerConsumption", "PropertyID");
            CreateIndex("solar.PowerConsumption", "SolarSystemID");
        }
    }
}
