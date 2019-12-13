namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrescreenZipAreas1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.AreaPurchases", "PersonID");
            AddForeignKey("dbo.AreaPurchases", "PersonID", "dbo.People", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaPurchases", "PersonID", "dbo.People");
            DropIndex("dbo.AreaPurchases", new[] { "PersonID" });
        }
    }
}
