namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Adding_missing_financeDetails_for_Spruce : DbMigration
    {
        public override void Up()
        {
            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/2017-03-30_01_01_Update_Spruce_FinanceDetails.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
        }
    }
}
