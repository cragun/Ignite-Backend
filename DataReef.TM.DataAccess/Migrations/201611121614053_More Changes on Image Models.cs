namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoreChangesonImageModels : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ImagePurchase", newName: "ImagePurchases");
            AddColumn("dbo.ImagePurchases", "ImageID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImagePurchases", "ImageID");
            RenameTable(name: "dbo.ImagePurchases", newName: "ImagePurchase");
        }
    }
}
