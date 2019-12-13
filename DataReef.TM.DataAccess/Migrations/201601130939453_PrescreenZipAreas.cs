namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrescreenZipAreas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AreaPurchases",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        AreaID = c.Guid(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        ErrorString = c.String(),
                        NumberOfTokens = c.Int(nullable: false),
                        TokenPriceInDollars = c.Single(nullable: false),
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
                .ForeignKey("dbo.ZipAreas", t => t.AreaID)
                .Index(t => t.AreaID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "dbo.ZipAreas",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        OUID = c.Guid(nullable: false),
                        PropertyCount = c.Int(nullable: false),
                        ActiveStartDate = c.DateTime(nullable: false),
                        LastPurchaseDate = c.DateTime(nullable: false),
                        ExpiryMinutes = c.Int(nullable: false),
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
                "dbo.Zip5Shape",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ShapeID = c.Guid(nullable: false),
                        WellKnownText = c.String(),
                        CentroidLat = c.Single(nullable: false),
                        CentroidLon = c.Single(nullable: false),
                        Radius = c.Single(nullable: false),
                        ResidentCount = c.Int(nullable: false),
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
                .ForeignKey("dbo.ZipAreas", t => t.Guid)
                .Index(t => t.Guid)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Zip5Shape", "Guid", "dbo.ZipAreas");
            DropForeignKey("dbo.AreaPurchases", "AreaID", "dbo.ZipAreas");
            DropForeignKey("dbo.ZipAreas", "OUID", "dbo.OUs");
            DropIndex("dbo.Zip5Shape", new[] { "ExternalID" });
            DropIndex("dbo.Zip5Shape", new[] { "Version" });
            DropIndex("dbo.Zip5Shape", new[] { "CreatedByID" });
            DropIndex("dbo.Zip5Shape", new[] { "DateCreated" });
            DropIndex("dbo.Zip5Shape", new[] { "TenantID" });
            DropIndex("dbo.Zip5Shape", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.Zip5Shape", new[] { "Guid" });
            DropIndex("dbo.ZipAreas", new[] { "ExternalID" });
            DropIndex("dbo.ZipAreas", new[] { "Version" });
            DropIndex("dbo.ZipAreas", new[] { "CreatedByID" });
            DropIndex("dbo.ZipAreas", new[] { "DateCreated" });
            DropIndex("dbo.ZipAreas", new[] { "TenantID" });
            DropIndex("dbo.ZipAreas", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.ZipAreas", new[] { "OUID" });
            DropIndex("dbo.AreaPurchases", new[] { "ExternalID" });
            DropIndex("dbo.AreaPurchases", new[] { "Version" });
            DropIndex("dbo.AreaPurchases", new[] { "CreatedByID" });
            DropIndex("dbo.AreaPurchases", new[] { "DateCreated" });
            DropIndex("dbo.AreaPurchases", new[] { "TenantID" });
            DropIndex("dbo.AreaPurchases", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.AreaPurchases", new[] { "AreaID" });
            DropTable("dbo.Zip5Shape");
            DropTable("dbo.ZipAreas");
            DropTable("dbo.AreaPurchases");
        }
    }
}
