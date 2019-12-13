namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdderItem_HistoryModelUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "AllowsQuantitySelection", c => c.Boolean(nullable: false));
            AddColumn("solar.AdderItems", "CanBePaidForByRep", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("solar.AdderItems", "CanBePaidForByRep");
            DropColumn("solar.AdderItems", "AllowsQuantitySelection");
        }
    }
}
