namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddtwoFieldinsolarFinanceplan : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.FinancePlans", "SunlightReqJson", c => c.String());
            AddColumn("solar.FinancePlans", "SunlightResponseJson", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.FinancePlans", "SunlightResponseJson");
            DropColumn("solar.FinancePlans", "SunlightReqJson");
        }
    }
}
