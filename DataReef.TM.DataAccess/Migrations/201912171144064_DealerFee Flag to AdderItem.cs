namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DealerFeeFlagtoAdderItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "ApplyDealerFee", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("solar.AdderItems", "ApplyDealerFee");
        }
    }
}
