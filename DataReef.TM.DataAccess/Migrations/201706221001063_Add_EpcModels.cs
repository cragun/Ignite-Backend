namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_EpcModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "EPC.EpcStatuses",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PropertyID = c.Guid(nullable: false),
                        Status = c.String(),
                        PersonID = c.Guid(nullable: false),
                        StatusProgress = c.Int(nullable: false),
                        CompletionDate = c.DateTime(),
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
                .ForeignKey("dbo.People", t => t.PersonID)
                .ForeignKey("dbo.Properties", t => t.PropertyID)
                .Index(t => t.PropertyID)
                .Index(t => t.PersonID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "EPC.ActionItems",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PropertyID = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        Description = c.String(),
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
                .ForeignKey("dbo.People", t => t.PersonID)
                .ForeignKey("dbo.Properties", t => t.PropertyID)
                .Index(t => t.PropertyID)
                .Index(t => t.PersonID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("EPC.ActionItems", "PropertyID", "dbo.Properties");
            DropForeignKey("EPC.ActionItems", "PersonID", "dbo.People");
            DropForeignKey("EPC.EpcStatuses", "PropertyID", "dbo.Properties");
            DropForeignKey("EPC.EpcStatuses", "PersonID", "dbo.People");
            DropIndex("EPC.ActionItems", new[] { "ExternalID" });
            DropIndex("EPC.ActionItems", new[] { "Version" });
            DropIndex("EPC.ActionItems", new[] { "CreatedByID" });
            DropIndex("EPC.ActionItems", new[] { "DateCreated" });
            DropIndex("EPC.ActionItems", new[] { "TenantID" });
            DropIndex("EPC.ActionItems", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("EPC.ActionItems", new[] { "PersonID" });
            DropIndex("EPC.ActionItems", new[] { "PropertyID" });
            DropIndex("EPC.EpcStatuses", new[] { "ExternalID" });
            DropIndex("EPC.EpcStatuses", new[] { "Version" });
            DropIndex("EPC.EpcStatuses", new[] { "CreatedByID" });
            DropIndex("EPC.EpcStatuses", new[] { "DateCreated" });
            DropIndex("EPC.EpcStatuses", new[] { "TenantID" });
            DropIndex("EPC.EpcStatuses", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("EPC.EpcStatuses", new[] { "PersonID" });
            DropIndex("EPC.EpcStatuses", new[] { "PropertyID" });
            DropTable("EPC.ActionItems");
            DropTable("EPC.EpcStatuses");
        }
    }
}
