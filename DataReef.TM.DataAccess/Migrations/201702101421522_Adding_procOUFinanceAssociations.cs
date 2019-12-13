namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_procOUFinanceAssociations : DbMigration
    {
        public override void Up()
        {
            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/proc_OUFinanceAssociations.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            Sql("DROP PROC proc_OUFinanceAssociations");
        }
    }
}
