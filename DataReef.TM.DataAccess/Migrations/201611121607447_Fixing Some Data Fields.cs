namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingSomeDataFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ImagePurchase", "OUID_Guid", "dbo.Users");
            DropIndex("dbo.ImagePurchase", new[] { "OUID_Guid" });
            AddColumn("dbo.ImagePurchase", "OUID", c => c.Guid(nullable: false));
            DropColumn("dbo.ImagePurchase", "OUID_Guid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImagePurchase", "OUID_Guid", c => c.Guid());
            DropColumn("dbo.ImagePurchase", "OUID");
            CreateIndex("dbo.ImagePurchase", "OUID_Guid");
            AddForeignKey("dbo.ImagePurchase", "OUID_Guid", "dbo.Users", "Guid");
        }
    }
}
