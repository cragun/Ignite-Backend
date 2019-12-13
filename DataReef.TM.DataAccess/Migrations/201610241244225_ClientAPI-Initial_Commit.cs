namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientAPIInitial_Commit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApiKeys",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        IsDisabled = c.Boolean(nullable: false),
                        AccessKey = c.String(maxLength: 20),
                        SecretKeyHash = c.String(maxLength: 50),
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
                .ForeignKey("dbo.OUs", t => t.OUID)
                .Index(t => t.OUID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "dbo.ApiTokens",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        IsDisabled = c.Boolean(nullable: false),
                        ApiKeyID = c.Guid(nullable: false),
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
                .ForeignKey("dbo.ApiKeys", t => t.ApiKeyID)
                .Index(t => t.ApiKeyID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "dbo.WebHooks",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        IsDisabled = c.Boolean(nullable: false),
                        Url = c.String(maxLength: 1000),
                        EventFlags = c.Int(nullable: false),
                        PrivateKey = c.String(),
                        NotificationEmailAddress = c.String(maxLength: 150),
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
                        OU_Guid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.OUs", t => t.OU_Guid)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID)
                .Index(t => t.OU_Guid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WebHooks", "OU_Guid", "dbo.OUs");
            DropForeignKey("dbo.ApiKeys", "OUID", "dbo.OUs");
            DropForeignKey("dbo.ApiTokens", "ApiKeyID", "dbo.ApiKeys");
            DropIndex("dbo.WebHooks", new[] { "OU_Guid" });
            DropIndex("dbo.WebHooks", new[] { "ExternalID" });
            DropIndex("dbo.WebHooks", new[] { "Version" });
            DropIndex("dbo.WebHooks", new[] { "CreatedByID" });
            DropIndex("dbo.WebHooks", new[] { "DateCreated" });
            DropIndex("dbo.WebHooks", new[] { "TenantID" });
            DropIndex("dbo.WebHooks", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.ApiTokens", new[] { "ExternalID" });
            DropIndex("dbo.ApiTokens", new[] { "Version" });
            DropIndex("dbo.ApiTokens", new[] { "CreatedByID" });
            DropIndex("dbo.ApiTokens", new[] { "DateCreated" });
            DropIndex("dbo.ApiTokens", new[] { "TenantID" });
            DropIndex("dbo.ApiTokens", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.ApiTokens", new[] { "ApiKeyID" });
            DropIndex("dbo.ApiKeys", new[] { "ExternalID" });
            DropIndex("dbo.ApiKeys", new[] { "Version" });
            DropIndex("dbo.ApiKeys", new[] { "CreatedByID" });
            DropIndex("dbo.ApiKeys", new[] { "DateCreated" });
            DropIndex("dbo.ApiKeys", new[] { "TenantID" });
            DropIndex("dbo.ApiKeys", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.ApiKeys", new[] { "OUID" });
            DropTable("dbo.WebHooks");
            DropTable("dbo.ApiTokens");
            DropTable("dbo.ApiKeys");
        }
    }
}
