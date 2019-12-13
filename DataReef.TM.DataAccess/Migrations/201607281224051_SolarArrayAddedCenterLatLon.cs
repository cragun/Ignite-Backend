namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SolarArrayAddedCenterLatLon : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Arrays", "CenterLatitude", c => c.Double(nullable: false));
            AddColumn("solar.Arrays", "CenterLongitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Arrays", "CenterLongitude");
            DropColumn("solar.Arrays", "CenterLatitude");
        }
    }
}
