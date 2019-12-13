namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewSolarModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.RoofPlanes",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Azimuth = c.Int(nullable: false),
                        CenterX = c.Double(nullable: false),
                        CenterY = c.Double(nullable: false),
                        CenterLatitude = c.Double(nullable: false),
                        CenterLongitude = c.Double(nullable: false),
                        GenabilitySolarProviderProfileID = c.String(),
                        IsManuallyEntered = c.Boolean(nullable: false),
                        ManuallyEnteredPanelsCount = c.Int(nullable: false),
                        ModuleSpacing = c.Double(nullable: false),
                        Pitch = c.Double(nullable: false),
                        Racking = c.Int(nullable: false),
                        RowSpacing = c.Double(nullable: false),
                        Shading = c.Int(nullable: false),
                        InverterID = c.Guid(nullable: false),
                        SolarPanelID = c.Guid(nullable: false),
                        SolarSystemID = c.Guid(nullable: false),
                        Tilt = c.Double(nullable: false),
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
                        SolarSystem_Guid = c.Guid(),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("solar.Proposals", t => t.SolarSystemID)
                .ForeignKey("solar.Systems", t => t.SolarSystem_Guid)
                .Index(t => t.SolarSystemID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID)
                .Index(t => t.SolarSystem_Guid);
            
            CreateTable(
                "solar.RoofPlaneEdges",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        EdgeType = c.Int(nullable: false),
                        StartPointX = c.Double(nullable: false),
                        StartPointY = c.Double(nullable: false),
                        EndPointX = c.Double(nullable: false),
                        EndPointY = c.Double(nullable: false),
                        FireOffset = c.Double(nullable: false),
                        FireOffsetIsEnabled = c.Boolean(nullable: false),
                        Index = c.Int(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
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
                .Index(t => t.RoofPlaneID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "solar.RoofPlaneObstructions",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
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
                .Index(t => t.RoofPlaneID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "solar.RoofPlanePanels",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        CenterX = c.Double(nullable: false),
                        CenterY = c.Double(nullable: false),
                        Height = c.Double(nullable: false),
                        Width = c.Double(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
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
                .Index(t => t.RoofPlaneID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "solar.RoofPlanePoints",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Index = c.Int(nullable: false),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                        RoofPlaneID = c.Guid(nullable: false),
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
                .Index(t => t.RoofPlaneID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            AddColumn("solar.Panels", "Thickness", c => c.Double(nullable: false));
            AddColumn("solar.Panels", "Weight", c => c.Double(nullable: false));
            AddColumn("solar.Panels", "NumberOfCells", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("solar.RoofPlanes", "SolarSystem_Guid", "solar.Systems");
            DropForeignKey("solar.RoofPlanes", "SolarSystemID", "solar.Proposals");
            DropForeignKey("solar.RoofPlanePoints", "RoofPlaneID", "solar.RoofPlanes");
            DropForeignKey("solar.RoofPlanePanels", "RoofPlaneID", "solar.RoofPlanes");
            DropForeignKey("solar.RoofPlaneObstructions", "RoofPlaneID", "solar.RoofPlanes");
            DropForeignKey("solar.RoofPlaneEdges", "RoofPlaneID", "solar.RoofPlanes");
            DropIndex("solar.RoofPlanePoints", new[] { "ExternalID" });
            DropIndex("solar.RoofPlanePoints", new[] { "Version" });
            DropIndex("solar.RoofPlanePoints", new[] { "CreatedByID" });
            DropIndex("solar.RoofPlanePoints", new[] { "DateCreated" });
            DropIndex("solar.RoofPlanePoints", new[] { "TenantID" });
            DropIndex("solar.RoofPlanePoints", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.RoofPlanePoints", new[] { "RoofPlaneID" });
            DropIndex("solar.RoofPlanePanels", new[] { "ExternalID" });
            DropIndex("solar.RoofPlanePanels", new[] { "Version" });
            DropIndex("solar.RoofPlanePanels", new[] { "CreatedByID" });
            DropIndex("solar.RoofPlanePanels", new[] { "DateCreated" });
            DropIndex("solar.RoofPlanePanels", new[] { "TenantID" });
            DropIndex("solar.RoofPlanePanels", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.RoofPlanePanels", new[] { "RoofPlaneID" });
            DropIndex("solar.RoofPlaneObstructions", new[] { "ExternalID" });
            DropIndex("solar.RoofPlaneObstructions", new[] { "Version" });
            DropIndex("solar.RoofPlaneObstructions", new[] { "CreatedByID" });
            DropIndex("solar.RoofPlaneObstructions", new[] { "DateCreated" });
            DropIndex("solar.RoofPlaneObstructions", new[] { "TenantID" });
            DropIndex("solar.RoofPlaneObstructions", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.RoofPlaneObstructions", new[] { "RoofPlaneID" });
            DropIndex("solar.RoofPlaneEdges", new[] { "ExternalID" });
            DropIndex("solar.RoofPlaneEdges", new[] { "Version" });
            DropIndex("solar.RoofPlaneEdges", new[] { "CreatedByID" });
            DropIndex("solar.RoofPlaneEdges", new[] { "DateCreated" });
            DropIndex("solar.RoofPlaneEdges", new[] { "TenantID" });
            DropIndex("solar.RoofPlaneEdges", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.RoofPlaneEdges", new[] { "RoofPlaneID" });
            DropIndex("solar.RoofPlanes", new[] { "SolarSystem_Guid" });
            DropIndex("solar.RoofPlanes", new[] { "ExternalID" });
            DropIndex("solar.RoofPlanes", new[] { "Version" });
            DropIndex("solar.RoofPlanes", new[] { "CreatedByID" });
            DropIndex("solar.RoofPlanes", new[] { "DateCreated" });
            DropIndex("solar.RoofPlanes", new[] { "TenantID" });
            DropIndex("solar.RoofPlanes", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.RoofPlanes", new[] { "SolarSystemID" });
            DropColumn("solar.Panels", "NumberOfCells");
            DropColumn("solar.Panels", "Weight");
            DropColumn("solar.Panels", "Thickness");
            DropTable("solar.RoofPlanePoints");
            DropTable("solar.RoofPlanePanels");
            DropTable("solar.RoofPlaneObstructions");
            DropTable("solar.RoofPlaneEdges");
            DropTable("solar.RoofPlanes");
        }
    }
}
