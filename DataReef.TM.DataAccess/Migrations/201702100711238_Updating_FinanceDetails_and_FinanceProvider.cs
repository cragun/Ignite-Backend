namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updating_FinanceDetails_and_FinanceProvider : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.FinanceDetails", "InterestIsPaid", c => c.Boolean(nullable: false));
            AddColumn("finance.Providers", "PhoneNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("finance.Providers", "PhoneNumber");
            DropColumn("finance.FinanceDetails", "InterestIsPaid");
        }
    }
}
