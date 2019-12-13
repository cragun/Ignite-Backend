namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinancePlanChanges1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn("solar.FinancePlans", "FinanceRequestJSON", "LoanRequestJSON");
            RenameColumn("solar.FinancePlans", "FinanceResponseJSON", "LoanResponseJSON");
        }
        
        public override void Down()
        {
            RenameColumn("solar.FinancePlans", "LoanRequestJSON", "FinanceRequestJSON");
            RenameColumn("solar.FinancePlans", "LoanResponseJSON", "FinanceResponseJSON");
        }
    }
}
