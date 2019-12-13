namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_FinanceDetail_IsSpruced : DbMigration
    {
        public override void Up()
        {
            AddColumn("finance.FinanceDetails", "IsSpruced", c => c.Boolean(nullable: false));

            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Seed_FinanceProvider_Data_02.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            DropColumn("finance.FinanceDetails", "IsSpruced");
        }
    }
}
