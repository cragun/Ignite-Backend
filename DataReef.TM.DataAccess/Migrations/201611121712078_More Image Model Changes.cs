namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreImageModelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImagePurchases", "GlobalID", c => c.String(maxLength: 50));
            AddColumn("dbo.ImagePurchases", "Top", c => c.Single(nullable: false));
            AddColumn("dbo.ImagePurchases", "Left", c => c.Single(nullable: false));
            AddColumn("dbo.ImagePurchases", "Bottom", c => c.Single(nullable: false));
            AddColumn("dbo.ImagePurchases", "Right", c => c.Single(nullable: false));
            DropColumn("dbo.ImagePurchases", "UniqueID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImagePurchases", "UniqueID", c => c.String(maxLength: 50));
            DropColumn("dbo.ImagePurchases", "Right");
            DropColumn("dbo.ImagePurchases", "Bottom");
            DropColumn("dbo.ImagePurchases", "Left");
            DropColumn("dbo.ImagePurchases", "Top");
            DropColumn("dbo.ImagePurchases", "GlobalID");
        }
    }
}
