namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdderItem_AddRoofPlaneDetails : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("solar.AdderItemRoofPlanes", "AdderItemID", "solar.AdderItems");
            DropForeignKey("solar.AdderItemRoofPlanes", "RoofPlaneID", "solar.RoofPlanes");
            DropIndex("solar.AdderItemRoofPlanes", new[] { "AdderItemID" });
            DropIndex("solar.AdderItemRoofPlanes", new[] { "RoofPlaneID" });
            CreateTable(
                "solar.RoofPlaneDetails",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        AdderItemID = c.Guid(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
                        Quantity = c.Int(nullable: false),
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
                .ForeignKey("solar.RoofPlanes", t => t.RoofPlaneID)
                .ForeignKey("solar.AdderItems", t => t.AdderItemID)
                .Index(t => t.AdderItemID)
                .Index(t => t.RoofPlaneID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            DropTable("solar.AdderItemRoofPlanes");
        }
        
        public override void Down()
        {
            CreateTable(
                "solar.AdderItemRoofPlanes",
                c => new
                    {
                        AdderItemID = c.Guid(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdderItemID, t.RoofPlaneID });
            
            DropForeignKey("solar.RoofPlaneDetails", "AdderItemID", "solar.AdderItems");
            DropForeignKey("solar.RoofPlaneDetails", "RoofPlaneID", "solar.RoofPlanes");
            DropIndex("solar.RoofPlaneDetails", new[] { "ExternalID" });
            DropIndex("solar.RoofPlaneDetails", new[] { "Version" });
            DropIndex("solar.RoofPlaneDetails", new[] { "CreatedByID" });
            DropIndex("solar.RoofPlaneDetails", new[] { "DateCreated" });
            DropIndex("solar.RoofPlaneDetails", new[] { "TenantID" });
            DropIndex("solar.RoofPlaneDetails", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.RoofPlaneDetails", new[] { "RoofPlaneID" });
            DropIndex("solar.RoofPlaneDetails", new[] { "AdderItemID" });
            DropTable("solar.RoofPlaneDetails");
            CreateIndex("solar.AdderItemRoofPlanes", "RoofPlaneID");
            CreateIndex("solar.AdderItemRoofPlanes", "AdderItemID");
            AddForeignKey("solar.AdderItemRoofPlanes", "RoofPlaneID", "solar.RoofPlanes", "Guid", cascadeDelete: true);
            AddForeignKey("solar.AdderItemRoofPlanes", "AdderItemID", "solar.AdderItems", "Guid", cascadeDelete: true);
        }
    }
}
