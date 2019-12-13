namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedHighResolutionImagemodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HighResolutionImages",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Top = c.Single(nullable: false),
                        Left = c.Single(nullable: false),
                        Bottom = c.Single(nullable: false),
                        Right = c.Single(nullable: false),
                        Resolution = c.Int(nullable: false),
                        Width = c.Int(nullable: false),
                        Height = c.Int(nullable: false),
                        Source = c.String(maxLength: 100),
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Flags = c.Long(),
                        TenantID = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateLastModified = c.DateTime(),
                        CreatedByName = c.String(maxLength: 100),
                        CreatedByID = c.Guid(),
                        LastModifiedBy = c.Guid(),
                        LastModifiedByName = c.String(maxLength: 100),
                        Version = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        ExternalID = c.String(maxLength: 50),
                        TagString = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Guid)
                .Index(t => new { t.Top, t.Left, t.Bottom, t.Right }, name: "idx_hires_coords")
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            AlterColumn("dbo.ImagePurchase", "ImageType", c => c.String(maxLength: 100));
            AlterColumn("dbo.ImagePurchase", "UniqueID", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropIndex("dbo.HighResolutionImages", new[] { "ExternalID" });
            DropIndex("dbo.HighResolutionImages", new[] { "Version" });
            DropIndex("dbo.HighResolutionImages", new[] { "CreatedByID" });
            DropIndex("dbo.HighResolutionImages", new[] { "DateCreated" });
            DropIndex("dbo.HighResolutionImages", new[] { "TenantID" });
            DropIndex("dbo.HighResolutionImages", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.HighResolutionImages", "idx_hires_coords");
            AlterColumn("dbo.ImagePurchase", "UniqueID", c => c.String());
            AlterColumn("dbo.ImagePurchase", "ImageType", c => c.String());
            DropTable("dbo.HighResolutionImages");
        }
    }
}
