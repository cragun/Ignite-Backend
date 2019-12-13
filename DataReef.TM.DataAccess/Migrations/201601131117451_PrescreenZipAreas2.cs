namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrescreenZipAreas2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AreaPurchases", "CompletionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AreaPurchases", "CompletionDate", c => c.DateTime(nullable: false));
        }
    }
}
