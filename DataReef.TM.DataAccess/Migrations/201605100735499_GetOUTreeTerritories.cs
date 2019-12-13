namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    
    public partial class GetOUTreeTerritories : DbMigration
    {
        public override void Up()
        {
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/GetOUTreeTerritories.sql");
            SqlFile(sqlMigrationPath);
        }
        
        public override void Down()
        {
        }
    }
}
