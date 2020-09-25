namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addproductionfield : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "ProductionKWH", c => c.Double(nullable: false));
            AddColumn("solar.Proposals", "ProductionKWHpercentage", c => c.Double(nullable: false));
            AddColumn("solar.Proposals", "IsManual", c => c.Boolean(nullable: false));
            AddColumn("solar.Proposals", "SystemSize", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "SystemSize");
            DropColumn("solar.Proposals", "IsManual");
            DropColumn("solar.Proposals", "ProductionKWHpercentage");
            DropColumn("solar.Proposals", "ProductionKWH");
        }
    }
}
