namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PropertyAttachmentandPropertyAttachmentItem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PropertyAttachmentItems",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ItemID = c.String(),
                        SectionID = c.String(),
                        Status = c.Int(nullable: false),
                        ImagesJson = c.String(),
                        RejectionMessagesJson = c.String(),
                        PropertyAttachmentID = c.Guid(nullable: false),
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
                .ForeignKey("dbo.PropertyAttachments", t => t.PropertyAttachmentID)
                .Index(t => t.PropertyAttachmentID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "dbo.PropertyAttachments",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        SystemTypeID = c.String(),
                        PropertyID = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
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
                .ForeignKey("dbo.Properties", t => t.PropertyID)
                .Index(t => t.PropertyID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PropertyAttachments", "PropertyID", "dbo.Properties");
            DropForeignKey("dbo.PropertyAttachmentItems", "PropertyAttachmentID", "dbo.PropertyAttachments");
            DropIndex("dbo.PropertyAttachments", new[] { "ExternalID" });
            DropIndex("dbo.PropertyAttachments", new[] { "Version" });
            DropIndex("dbo.PropertyAttachments", new[] { "CreatedByID" });
            DropIndex("dbo.PropertyAttachments", new[] { "DateCreated" });
            DropIndex("dbo.PropertyAttachments", new[] { "TenantID" });
            DropIndex("dbo.PropertyAttachments", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PropertyAttachments", new[] { "PropertyID" });
            DropIndex("dbo.PropertyAttachmentItems", new[] { "ExternalID" });
            DropIndex("dbo.PropertyAttachmentItems", new[] { "Version" });
            DropIndex("dbo.PropertyAttachmentItems", new[] { "CreatedByID" });
            DropIndex("dbo.PropertyAttachmentItems", new[] { "DateCreated" });
            DropIndex("dbo.PropertyAttachmentItems", new[] { "TenantID" });
            DropIndex("dbo.PropertyAttachmentItems", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PropertyAttachmentItems", new[] { "PropertyAttachmentID" });
            DropTable("dbo.PropertyAttachments");
            DropTable("dbo.PropertyAttachmentItems");
        }
    }
}
