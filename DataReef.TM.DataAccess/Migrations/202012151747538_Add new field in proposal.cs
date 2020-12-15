namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addnewfieldinproposal : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.Proposals", "TotalBill", c => c.Double(nullable: false));
            AddColumn("solar.Proposals", "TotalKWH", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.Proposals", "TotalKWH");
            DropColumn("solar.Proposals", "TotalBill");
        }
    }
}
