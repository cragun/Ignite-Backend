namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removing_OUSettingDefinition_and_OUSettingValue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OUSettingDefinitions", "OUID", "dbo.OUs");
            DropForeignKey("dbo.OUSettingValues", "SettingID", "dbo.OUSettingDefinitions");
            DropForeignKey("dbo.OUSettingValues", "OUID", "dbo.OUs");
            DropIndex("dbo.OUSettingDefinitions", new[] { "OUID" });
            DropIndex("dbo.OUSettingDefinitions", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUSettingDefinitions", new[] { "TenantID" });
            DropIndex("dbo.OUSettingDefinitions", new[] { "DateCreated" });
            DropIndex("dbo.OUSettingDefinitions", new[] { "CreatedByID" });
            DropIndex("dbo.OUSettingDefinitions", new[] { "Version" });
            DropIndex("dbo.OUSettingDefinitions", new[] { "ExternalID" });
            DropIndex("dbo.OUSettingValues", new[] { "SettingID" });
            DropIndex("dbo.OUSettingValues", new[] { "OUID" });
            DropIndex("dbo.OUSettingValues", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUSettingValues", new[] { "TenantID" });
            DropIndex("dbo.OUSettingValues", new[] { "DateCreated" });
            DropIndex("dbo.OUSettingValues", new[] { "CreatedByID" });
            DropIndex("dbo.OUSettingValues", new[] { "Version" });
            DropIndex("dbo.OUSettingValues", new[] { "ExternalID" });
            DropTable("dbo.OUSettingDefinitions");
            DropTable("dbo.OUSettingValues");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OUSettingValues",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        SettingID = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        Value = c.String(maxLength: 100),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.OUSettingDefinitions",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(),
                        IsActive = c.Boolean(nullable: false),
                        Description = c.String(maxLength: 1000),
                        DisplayName = c.String(maxLength: 50),
                        Name = c.String(maxLength: 50),
                        FormatString = c.String(maxLength: 50),
                        EditorType = c.Int(nullable: false),
                        ListData = c.String(),
                        Group = c.String(maxLength: 100),
                        SortOrder = c.Int(nullable: false),
                        MaxLength = c.Int(nullable: false),
                        Id = c.Long(nullable: false, identity: true),
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
                .PrimaryKey(t => t.Guid);
            
            CreateIndex("dbo.OUSettingValues", "ExternalID");
            CreateIndex("dbo.OUSettingValues", "Version");
            CreateIndex("dbo.OUSettingValues", "CreatedByID");
            CreateIndex("dbo.OUSettingValues", "DateCreated");
            CreateIndex("dbo.OUSettingValues", "TenantID");
            CreateIndex("dbo.OUSettingValues", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.OUSettingValues", "OUID");
            CreateIndex("dbo.OUSettingValues", "SettingID");
            CreateIndex("dbo.OUSettingDefinitions", "ExternalID");
            CreateIndex("dbo.OUSettingDefinitions", "Version");
            CreateIndex("dbo.OUSettingDefinitions", "CreatedByID");
            CreateIndex("dbo.OUSettingDefinitions", "DateCreated");
            CreateIndex("dbo.OUSettingDefinitions", "TenantID");
            CreateIndex("dbo.OUSettingDefinitions", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.OUSettingDefinitions", "OUID");
            AddForeignKey("dbo.OUSettingValues", "OUID", "dbo.OUs", "Guid");
            AddForeignKey("dbo.OUSettingValues", "SettingID", "dbo.OUSettingDefinitions", "Guid");
            AddForeignKey("dbo.OUSettingDefinitions", "OUID", "dbo.OUs", "Guid");
        }
    }
}
