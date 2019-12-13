namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updating_proc_TerritoryAnalytics_storedProceduresql : DbMigration
    {
        public override void Up()
        {
            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/2017-03-23_01_01_Update_proc_TerritoryAnalytics.sql");
            SqlFile(sqlMigrationPath);
        }
        
        public override void Down()
        {
            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/2017-03-23_01_02_Revert_proc_TerritoryAnalytics.sql");
            SqlFile(sqlMigrationPath);
        }
    }
}
