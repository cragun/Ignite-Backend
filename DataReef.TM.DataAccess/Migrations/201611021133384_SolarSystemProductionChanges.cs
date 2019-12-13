namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class SolarSystemProductionChanges : DbMigration
    {
        public override void Up()
        {
            RenameColumn("solar.SystemsProduction", "TarrifID", "TariffID");
        }
        
        public override void Down()
        {
            RenameColumn("solar.SystemsProduction", "TariffID", "TarrifID");
        }
    }
}
