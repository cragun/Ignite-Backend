namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLayerFolderAndLayerDefinition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LayerDefinitions",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        FolderID = c.Guid(nullable: false),
                        DescriptionString = c.String(maxLength: 200),
                        LayerKey = c.String(maxLength: 50),
                        IsLayerActive = c.Boolean(nullable: false),
                        VisualizationType = c.Int(nullable: false),
                        DefaultColor = c.String(maxLength: 10),
                        LastCompileDate = c.DateTime(nullable: false),
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
                .ForeignKey("dbo.LayerFolders", t => t.FolderID)
                .Index(t => t.FolderID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "dbo.LayerFolders",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        DescriptionString = c.String(maxLength: 200),
                        ParentID = c.Guid(),
                        ImageName = c.String(maxLength: 100),
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
                .ForeignKey("dbo.LayerFolders", t => t.ParentID)
                .Index(t => t.ParentID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LayerDefinitions", "FolderID", "dbo.LayerFolders");
            DropForeignKey("dbo.LayerFolders", "ParentID", "dbo.LayerFolders");
            DropIndex("dbo.LayerFolders", new[] { "ExternalID" });
            DropIndex("dbo.LayerFolders", new[] { "Version" });
            DropIndex("dbo.LayerFolders", new[] { "CreatedByID" });
            DropIndex("dbo.LayerFolders", new[] { "DateCreated" });
            DropIndex("dbo.LayerFolders", new[] { "TenantID" });
            DropIndex("dbo.LayerFolders", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.LayerFolders", new[] { "ParentID" });
            DropIndex("dbo.LayerDefinitions", new[] { "ExternalID" });
            DropIndex("dbo.LayerDefinitions", new[] { "Version" });
            DropIndex("dbo.LayerDefinitions", new[] { "CreatedByID" });
            DropIndex("dbo.LayerDefinitions", new[] { "DateCreated" });
            DropIndex("dbo.LayerDefinitions", new[] { "TenantID" });
            DropIndex("dbo.LayerDefinitions", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.LayerDefinitions", new[] { "FolderID" });
            DropTable("dbo.LayerFolders");
            DropTable("dbo.LayerDefinitions");
        }
    }
}
