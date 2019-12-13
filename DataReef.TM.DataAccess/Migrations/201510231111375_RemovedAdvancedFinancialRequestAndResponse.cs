namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedAdvancedFinancialRequestAndResponse : DbMigration
    {
        public override void Up()
        {
            DropColumn("solar.FinancePlans", "AdvancedLoanRequestJSON");
            DropColumn("solar.FinancePlans", "AdvancedLoanResponseJSON");
        }
        
        public override void Down()
        {
            AddColumn("solar.FinancePlans", "AdvancedLoanResponseJSON", c => c.String());
            AddColumn("solar.FinancePlans", "AdvancedLoanRequestJSON", c => c.String());
        }
    }
}
