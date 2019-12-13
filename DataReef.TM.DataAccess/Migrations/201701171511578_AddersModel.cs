namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddersModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.AdderItems",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        SolarSystemID = c.Guid(nullable: false),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RateType = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        ReducesUsage = c.Boolean(nullable: false),
                        UsageReductionAmount = c.Decimal(precision: 18, scale: 2),
                        UsageReductionType = c.Int(),
                        AddToSystemCost = c.Boolean(nullable: false),
                        IsCalculatedPerRoofPlane = c.Boolean(nullable: false),
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
                .ForeignKey("solar.Systems", t => t.SolarSystemID)
                .Index(t => t.SolarSystemID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "solar.AdderItemRoofPlanes",
                c => new
                    {
                        AdderItemID = c.Guid(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdderItemID, t.RoofPlaneID })
                .ForeignKey("solar.AdderItems", t => t.AdderItemID, cascadeDelete: true)
                .ForeignKey("solar.RoofPlanes", t => t.RoofPlaneID, cascadeDelete: true)
                .Index(t => t.AdderItemID)
                .Index(t => t.RoofPlaneID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("solar.AdderItemRoofPlanes", "RoofPlaneID", "solar.RoofPlanes");
            DropForeignKey("solar.AdderItemRoofPlanes", "AdderItemID", "solar.AdderItems");
            DropForeignKey("solar.AdderItems", "SolarSystemID", "solar.Systems");
            DropIndex("solar.AdderItemRoofPlanes", new[] { "RoofPlaneID" });
            DropIndex("solar.AdderItemRoofPlanes", new[] { "AdderItemID" });
            DropIndex("solar.AdderItems", new[] { "ExternalID" });
            DropIndex("solar.AdderItems", new[] { "Version" });
            DropIndex("solar.AdderItems", new[] { "CreatedByID" });
            DropIndex("solar.AdderItems", new[] { "DateCreated" });
            DropIndex("solar.AdderItems", new[] { "TenantID" });
            DropIndex("solar.AdderItems", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.AdderItems", new[] { "SolarSystemID" });
            DropTable("solar.AdderItemRoofPlanes");
            DropTable("solar.AdderItems");
        }
    }
}
