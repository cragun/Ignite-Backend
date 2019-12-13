namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrescreenZipAreas5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ZipAreas", "ActiveStartDate", c => c.DateTime());
            AlterColumn("dbo.ZipAreas", "LastPurchaseDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ZipAreas", "LastPurchaseDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ZipAreas", "ActiveStartDate", c => c.DateTime(nullable: false));
        }
    }
}
