namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ImagePurcase_Model : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImagePurchase",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        UserID = c.Guid(nullable: false),
                        Lat = c.Single(nullable: false),
                        Lon = c.Single(nullable: false),
                        Tokens = c.Int(nullable: false),
                        ImageType = c.String(),
                        PropertyID = c.Guid(nullable: false),
                        UniqueID = c.String(),
                        ImageWasCached = c.Boolean(nullable: false),
                        LocationX = c.Single(nullable: false),
                        LocationY = c.Single(nullable: false),
                        ImageDate = c.DateTime(nullable: false),
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
                        OUID_Guid = c.Guid(),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Users", t => t.OUID_Guid)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID)
                .Index(t => t.OUID_Guid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImagePurchase", "OUID_Guid", "dbo.Users");
            DropIndex("dbo.ImagePurchase", new[] { "OUID_Guid" });
            DropIndex("dbo.ImagePurchase", new[] { "ExternalID" });
            DropIndex("dbo.ImagePurchase", new[] { "Version" });
            DropIndex("dbo.ImagePurchase", new[] { "CreatedByID" });
            DropIndex("dbo.ImagePurchase", new[] { "DateCreated" });
            DropIndex("dbo.ImagePurchase", new[] { "TenantID" });
            DropIndex("dbo.ImagePurchase", "CLUSTERED_INDEX_ON_LONG");
            DropTable("dbo.ImagePurchase");
        }
    }
}
