namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrescreenZipAreas4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaPurchases", "OUID", c => c.Guid(nullable: false));
            CreateIndex("dbo.AreaPurchases", "OUID");
            AddForeignKey("dbo.AreaPurchases", "OUID", "dbo.OUs", "Guid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaPurchases", "OUID", "dbo.OUs");
            DropIndex("dbo.AreaPurchases", new[] { "OUID" });
            DropColumn("dbo.AreaPurchases", "OUID");
        }
    }
}
