namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinanceTypeToAdder : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.AdderItems", "FinancingFee", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("solar.AdderItems", "FinancingFee");
        }
    }
}
