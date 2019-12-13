namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_OUTreeUp_SQL_function : DbMigration
    {
        public override void Up()
        {
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/OUTreeUP_SQL_Function.sql");
            SqlFile(sqlMigrationPath);
        }
        
        public override void Down()
        {
            Sql("DROP FUNCTION [dbo].[OUTreeUP]");
        }
    }
}
