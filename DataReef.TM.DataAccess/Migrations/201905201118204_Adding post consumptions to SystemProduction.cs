namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingpostconsumptionstoSystemProduction : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.SystemsProduction", "PostAddersConsumption", c => c.Single());
            AddColumn("solar.SystemsProduction", "PostSolarConsumption", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("solar.SystemsProduction", "PostSolarConsumption");
            DropColumn("solar.SystemsProduction", "PostAddersConsumption");
        }
    }
}
