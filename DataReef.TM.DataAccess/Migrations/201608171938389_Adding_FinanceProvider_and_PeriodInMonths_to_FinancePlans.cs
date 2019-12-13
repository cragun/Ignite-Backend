namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_FinanceProvider_and_PeriodInMonths_to_FinancePlans : DbMigration
    {
        public override void Up()
        {
            AddColumn("solar.FinancePlans", "FinanceProvider", c => c.Int(nullable: false));
            AddColumn("solar.FinancePlans", "PeriodInMonths", c => c.Int(nullable: false));
            AddColumn("solar.FinancePlans", "RequestJSON", c => c.String());
            AddColumn("solar.FinancePlans", "ResponseJSON", c => c.String());

            // Migrating data            
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/FinancePlans_Adding_FinanceProvider_and_PeriodInMonths.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            // Rolling back migrated data
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/FinancePlans_Adding_FinanceProvider_and_PeriodInMonths_Rollback.sql");
            SqlFile(sqlMigrationPath);

            DropColumn("solar.FinancePlans", "ResponseJSON");
            DropColumn("solar.FinancePlans", "RequestJSON");
            DropColumn("solar.FinancePlans", "PeriodInMonths");
            DropColumn("solar.FinancePlans", "FinanceProvider");
        }
    }
}
