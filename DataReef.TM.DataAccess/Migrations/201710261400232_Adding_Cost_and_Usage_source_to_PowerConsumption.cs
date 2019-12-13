namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Cost_and_Usage_source_to_PowerConsumption : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.PowerConsumption", "CostSource", c => c.Int());
            AddColumn("solar.PowerConsumption", "UsageSource", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("solar.PowerConsumption", "UsageSource");
            DropColumn("solar.PowerConsumption", "CostSource");
        }
    }
}
