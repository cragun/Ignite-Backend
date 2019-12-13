namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removing_Unused_FinanceDetail_Properties : DbMigration
    {
        public override void Up()
        {
            DropColumn("finance.FinanceDetails", "PrincipalIsPaid");
            DropColumn("finance.FinanceDetails", "InterestIsPaid");
            DropColumn("finance.FinanceDetails", "ApplyPrincipalReductionAfter");
        }
        
        public override void Down()
        {
            AddColumn("finance.FinanceDetails", "ApplyPrincipalReductionAfter", c => c.Boolean(nullable: false));
            AddColumn("finance.FinanceDetails", "InterestIsPaid", c => c.Boolean(nullable: false));
            AddColumn("finance.FinanceDetails", "PrincipalIsPaid", c => c.Boolean(nullable: false));
        }
    }
}
