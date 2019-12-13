namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Rank_to_SolarPanels_and_Inverters : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Inverters", "Rank", c => c.Int(nullable: false));
            AddColumn("solar.Panels", "Rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Panels", "Rank");
            DropColumn("solar.Inverters", "Rank");
        }
    }
}
