namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSolarSystemIsPPAPricingB : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Systems", "IsPPAPricingB", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Systems", "IsPPAPricingB");
        }
    }
}
