namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImagePurchase_RemoveUnusedCroppingColumns : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ImagePurchases", "CroppedX");
            DropColumn("dbo.ImagePurchases", "CroppedY");
            DropColumn("dbo.ImagePurchases", "CroppedWidth");
            DropColumn("dbo.ImagePurchases", "CroppedHeight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImagePurchases", "CroppedHeight", c => c.Int(nullable: false));
            AddColumn("dbo.ImagePurchases", "CroppedWidth", c => c.Int(nullable: false));
            AddColumn("dbo.ImagePurchases", "CroppedY", c => c.Int(nullable: false));
            AddColumn("dbo.ImagePurchases", "CroppedX", c => c.Int(nullable: false));
        }
    }
}
