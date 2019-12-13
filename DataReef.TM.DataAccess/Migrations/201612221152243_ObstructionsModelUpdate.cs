namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ObstructionsModelUpdate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.ObstructionPoints",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        RoofPlaneObstructionID = c.Guid(nullable: false),
                        Index = c.Int(nullable: false),
                        PointX = c.Single(nullable: false),
                        PointY = c.Single(nullable: false),
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
                .ForeignKey("solar.RoofPlaneObstructions", t => t.RoofPlaneObstructionID)
                .Index(t => t.RoofPlaneObstructionID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            AddColumn("solar.RoofPlaneObstructions", "ObstructionType", c => c.Int(nullable: false));
            AddColumn("solar.RoofPlaneObstructions", "Radius", c => c.Single());
            AddColumn("solar.RoofPlaneObstructions", "CenterID", c => c.Guid());
        }
        
        public override void Down()
        {
            DropForeignKey("solar.ObstructionPoints", "RoofPlaneObstructionID", "solar.RoofPlaneObstructions");
            DropIndex("solar.ObstructionPoints", new[] { "ExternalID" });
            DropIndex("solar.ObstructionPoints", new[] { "Version" });
            DropIndex("solar.ObstructionPoints", new[] { "CreatedByID" });
            DropIndex("solar.ObstructionPoints", new[] { "DateCreated" });
            DropIndex("solar.ObstructionPoints", new[] { "TenantID" });
            DropIndex("solar.ObstructionPoints", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ObstructionPoints", new[] { "RoofPlaneObstructionID" });
            DropColumn("solar.RoofPlaneObstructions", "CenterID");
            DropColumn("solar.RoofPlaneObstructions", "Radius");
            DropColumn("solar.RoofPlaneObstructions", "ObstructionType");
            DropTable("solar.ObstructionPoints");
        }
    }
}
