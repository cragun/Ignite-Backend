namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MakingActivityTypeAnEnum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUs", "ActivityTypes", c => c.Int(nullable: false));
            AddColumn("dbo.Inquiries", "ActivityType", c => c.Int(nullable: false));

            Sql("UPDATE Inquiries SET ActivityType = 1 WHERE ActivityTypeID = '00000000-1000-0000-0000-000000000000'");
            Sql("UPDATE Inquiries SET ActivityType = 2 WHERE ActivityTypeID = '00000000-2000-0000-0000-000000000000'");
            Sql("UPDATE Inquiries SET ActivityType = 4 WHERE ActivityTypeID = '00000000-3000-0000-0000-000000000000'");
            Sql("UPDATE Inquiries SET ActivityType = 8 WHERE ActivityTypeID = '00000000-4000-0000-0000-000000000000'");

            Sql("UPDATE OUs SET ActivityTypes = 1 WHERE RootOrganizationID = '2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'");
            Sql("UPDATE OUs SET ActivityTypes = 2 WHERE RootOrganizationID = 'F3A25F2C-AD03-4C68-B37A-7C105D861B14'");
            Sql("UPDATE OUs SET ActivityTypes = 4 WHERE GUID = '18B48634-CA2F-42F5-94B4-DC3A44D7FF3E'");

            DropForeignKey("dbo.OUActivityTypes", "OUID", "dbo.OUs");
            DropForeignKey("dbo.OUActivityTypes", "ActivityTypeID", "dbo.ActivityTypes");
            DropForeignKey("dbo.Inquiries", "ActivityTypeID", "dbo.ActivityTypes");
            DropIndex("dbo.Inquiries", new[] { "ActivityTypeID" });
            DropIndex("dbo.ActivityTypes", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.ActivityTypes", new[] { "TenantID" });
            DropIndex("dbo.ActivityTypes", new[] { "DateCreated" });
            DropIndex("dbo.ActivityTypes", new[] { "CreatedByID" });
            DropIndex("dbo.ActivityTypes", new[] { "Version" });
            DropIndex("dbo.ActivityTypes", new[] { "ExternalID" });
            DropIndex("dbo.OUActivityTypes", new[] { "OUID" });
            DropIndex("dbo.OUActivityTypes", new[] { "ActivityTypeID" });
            DropIndex("dbo.OUActivityTypes", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUActivityTypes", new[] { "TenantID" });
            DropIndex("dbo.OUActivityTypes", new[] { "DateCreated" });
            DropIndex("dbo.OUActivityTypes", new[] { "CreatedByID" });
            DropIndex("dbo.OUActivityTypes", new[] { "Version" });
            DropIndex("dbo.OUActivityTypes", new[] { "ExternalID" });
            DropColumn("dbo.Inquiries", "ActivityTypeID");
            DropTable("dbo.OUActivityTypes");
            DropTable("dbo.ActivityTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OUActivityTypes",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        ActivityTypeID = c.Guid(nullable: false),
                        IsInheritable = c.Boolean(nullable: false),
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
                "dbo.ActivityTypes",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
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
            
            AddColumn("dbo.Inquiries", "ActivityTypeID", c => c.Guid(nullable: false));
            DropColumn("dbo.Inquiries", "ActivityType");
            DropColumn("dbo.OUs", "ActivityTypes");
            CreateIndex("dbo.OUActivityTypes", "ExternalID");
            CreateIndex("dbo.OUActivityTypes", "Version");
            CreateIndex("dbo.OUActivityTypes", "CreatedByID");
            CreateIndex("dbo.OUActivityTypes", "DateCreated");
            CreateIndex("dbo.OUActivityTypes", "TenantID");
            CreateIndex("dbo.OUActivityTypes", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.OUActivityTypes", "ActivityTypeID");
            CreateIndex("dbo.OUActivityTypes", "OUID");
            CreateIndex("dbo.ActivityTypes", "ExternalID");
            CreateIndex("dbo.ActivityTypes", "Version");
            CreateIndex("dbo.ActivityTypes", "CreatedByID");
            CreateIndex("dbo.ActivityTypes", "DateCreated");
            CreateIndex("dbo.ActivityTypes", "TenantID");
            CreateIndex("dbo.ActivityTypes", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.Inquiries", "ActivityTypeID");
            AddForeignKey("dbo.Inquiries", "ActivityTypeID", "dbo.ActivityTypes", "Guid");
            AddForeignKey("dbo.OUActivityTypes", "ActivityTypeID", "dbo.ActivityTypes", "Guid");
            AddForeignKey("dbo.OUActivityTypes", "OUID", "dbo.OUs", "Guid");
        }
    }
}
