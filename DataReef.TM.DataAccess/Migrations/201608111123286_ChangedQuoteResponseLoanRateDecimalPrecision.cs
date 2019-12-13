namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedQuoteResponseLoanRateDecimalPrecision : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Spruce.QuoteResponses", "LoanRate", c => c.Decimal(nullable: false, precision: 18, scale: 6));
        }
        
        public override void Down()
        {
            AlterColumn("Spruce.QuoteResponses", "LoanRate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
