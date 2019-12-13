namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCroppingData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImagePurchases", "CroppedX", c => c.Int(nullable: false));
            AddColumn("dbo.ImagePurchases", "CroppedY", c => c.Int(nullable: false));
            AddColumn("dbo.ImagePurchases", "CroppedWidth", c => c.Int(nullable: false));
            AddColumn("dbo.ImagePurchases", "CroppedHeight", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImagePurchases", "CroppedHeight");
            DropColumn("dbo.ImagePurchases", "CroppedWidth");
            DropColumn("dbo.ImagePurchases", "CroppedY");
            DropColumn("dbo.ImagePurchases", "CroppedX");
        }
    }
}
