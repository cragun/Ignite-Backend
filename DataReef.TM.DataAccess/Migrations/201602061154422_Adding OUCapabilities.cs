namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingOUCapabilities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OUCapabilities",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        CapabilityID = c.Guid(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
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
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Capabilities", t => t.CapabilityID)
                .ForeignKey("dbo.OUs", t => t.OUID)
                .Index(t => t.OUID)
                .Index(t => t.CapabilityID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "dbo.Capabilities",
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
                .PrimaryKey(t => t.Guid)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OUCapabilities", "OUID", "dbo.OUs");
            DropForeignKey("dbo.OUCapabilities", "CapabilityID", "dbo.Capabilities");
            DropIndex("dbo.Capabilities", new[] { "ExternalID" });
            DropIndex("dbo.Capabilities", new[] { "Version" });
            DropIndex("dbo.Capabilities", new[] { "CreatedByID" });
            DropIndex("dbo.Capabilities", new[] { "DateCreated" });
            DropIndex("dbo.Capabilities", new[] { "TenantID" });
            DropIndex("dbo.Capabilities", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUCapabilities", new[] { "ExternalID" });
            DropIndex("dbo.OUCapabilities", new[] { "Version" });
            DropIndex("dbo.OUCapabilities", new[] { "CreatedByID" });
            DropIndex("dbo.OUCapabilities", new[] { "DateCreated" });
            DropIndex("dbo.OUCapabilities", new[] { "TenantID" });
            DropIndex("dbo.OUCapabilities", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUCapabilities", new[] { "CapabilityID" });
            DropIndex("dbo.OUCapabilities", new[] { "OUID" });
            DropTable("dbo.Capabilities");
            DropTable("dbo.OUCapabilities");
        }
    }
}
