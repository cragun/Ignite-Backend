namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Property_Add_ApplyConsumptionSlope : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Properties", "ApplyConsumptionSlope", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Properties", "ApplyConsumptionSlope");
        }
    }
}
