namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdvancedFinancePlanFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.FinancePlans", "AdvancedLoanRequestJSON", c => c.String());
            AddColumn("solar.FinancePlans", "AdvancedLoanResponseJSON", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("solar.FinancePlans", "AdvancedLoanResponseJSON");
            DropColumn("solar.FinancePlans", "AdvancedLoanRequestJSON");
        }
    }
}
