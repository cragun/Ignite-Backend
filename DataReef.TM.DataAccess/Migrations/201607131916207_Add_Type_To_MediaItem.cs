namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Type_To_MediaItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MediaItems", "MediaType", c => c.Int(nullable: false));

            var sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Set_MediaType_To_MediaItems.sql");
            SqlFile(sqlMigrationPath);
        }
        
        public override void Down()
        {
            DropColumn("dbo.MediaItems", "MediaType");
        }
    }
}
