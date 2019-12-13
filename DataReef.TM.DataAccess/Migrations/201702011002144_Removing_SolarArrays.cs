namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removing_SolarArrays : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("solar.Arrays", "InverterID", "solar.Inverters");
            DropForeignKey("solar.ArrayPanels", "SolarArrayID", "solar.Arrays");
            DropForeignKey("solar.ArraySegments", "SolarArrayID", "solar.Arrays");
            DropForeignKey("solar.Arrays", "SolarPanelID", "solar.Panels");
            DropForeignKey("solar.Arrays", "SolarSystemID", "solar.Systems");
            DropIndex("solar.Arrays", new[] { "SolarPanelID" });
            DropIndex("solar.Arrays", new[] { "SolarSystemID" });
            DropIndex("solar.Arrays", new[] { "InverterID" });
            DropIndex("solar.Arrays", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.Arrays", new[] { "TenantID" });
            DropIndex("solar.Arrays", new[] { "DateCreated" });
            DropIndex("solar.Arrays", new[] { "CreatedByID" });
            DropIndex("solar.Arrays", new[] { "Version" });
            DropIndex("solar.Arrays", new[] { "ExternalID" });
            DropIndex("solar.ArrayPanels", new[] { "SolarArrayID" });
            DropIndex("solar.ArrayPanels", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ArrayPanels", new[] { "TenantID" });
            DropIndex("solar.ArrayPanels", new[] { "DateCreated" });
            DropIndex("solar.ArrayPanels", new[] { "CreatedByID" });
            DropIndex("solar.ArrayPanels", new[] { "Version" });
            DropIndex("solar.ArrayPanels", new[] { "ExternalID" });
            DropIndex("solar.ArraySegments", new[] { "SolarArrayID" });
            DropIndex("solar.ArraySegments", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ArraySegments", new[] { "TenantID" });
            DropIndex("solar.ArraySegments", new[] { "DateCreated" });
            DropIndex("solar.ArraySegments", new[] { "CreatedByID" });
            DropIndex("solar.ArraySegments", new[] { "Version" });
            DropIndex("solar.ArraySegments", new[] { "ExternalID" });
            DropTable("solar.Arrays");
            DropTable("solar.ArrayPanels");
            DropTable("solar.ArraySegments");
        }
        
        public override void Down()
        {
            CreateTable(
                "solar.ArraySegments",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        SolarArrayID = c.Guid(nullable: false),
                        X = c.Double(nullable: false),
                        Y = c.Double(nullable: false),
                        Index = c.Int(nullable: false),
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
                "solar.ArrayPanels",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        SolarArrayID = c.Guid(nullable: false),
                        IsHidden = c.Boolean(nullable: false),
                        Row = c.Int(nullable: false),
                        Column = c.Int(nullable: false),
                        WellKnownText = c.String(),
                        X1 = c.Double(nullable: false),
                        X2 = c.Double(nullable: false),
                        Y1 = c.Double(nullable: false),
                        Y2 = c.Double(nullable: false),
                        CentroidX = c.Double(nullable: false),
                        CentroidY = c.Double(nullable: false),
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
                "solar.Arrays",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        SolarPanelID = c.Guid(),
                        SolarSystemID = c.Guid(nullable: false),
                        PanelOrientation = c.Int(nullable: false),
                        InverterID = c.Guid(),
                        Azimuth = c.Int(nullable: false),
                        RidgeLineAzimuth = c.Int(),
                        Tilt = c.Int(nullable: false),
                        Racking = c.Int(nullable: false),
                        ModuleSpacing = c.Double(nullable: false),
                        RowSpacing = c.Double(nullable: false),
                        SolarArrayRotation = c.Double(nullable: false),
                        AnchorPointX = c.Double(nullable: false),
                        AnchorPointY = c.Double(nullable: false),
                        CenterX = c.Double(nullable: false),
                        CenterY = c.Double(nullable: false),
                        CenterLatitude = c.Double(nullable: false),
                        CenterLongitude = c.Double(nullable: false),
                        IsManuallyEntered = c.Boolean(nullable: false),
                        Shading = c.Short(nullable: false),
                        PanXOffset = c.Double(nullable: false),
                        PanYOffset = c.Double(nullable: false),
                        BoundingRect = c.String(),
                        Bounds = c.String(),
                        ManuallyEnteredPanelsCount = c.Int(nullable: false),
                        GenabilitySolarProviderProfileID = c.String(),
                        FireOffsetIsEnabled = c.Boolean(nullable: false),
                        FireOffset = c.Double(nullable: false),
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
            
            CreateIndex("solar.ArraySegments", "ExternalID");
            CreateIndex("solar.ArraySegments", "Version");
            CreateIndex("solar.ArraySegments", "CreatedByID");
            CreateIndex("solar.ArraySegments", "DateCreated");
            CreateIndex("solar.ArraySegments", "TenantID");
            CreateIndex("solar.ArraySegments", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("solar.ArraySegments", "SolarArrayID");
            CreateIndex("solar.ArrayPanels", "ExternalID");
            CreateIndex("solar.ArrayPanels", "Version");
            CreateIndex("solar.ArrayPanels", "CreatedByID");
            CreateIndex("solar.ArrayPanels", "DateCreated");
            CreateIndex("solar.ArrayPanels", "TenantID");
            CreateIndex("solar.ArrayPanels", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("solar.ArrayPanels", "SolarArrayID");
            CreateIndex("solar.Arrays", "ExternalID");
            CreateIndex("solar.Arrays", "Version");
            CreateIndex("solar.Arrays", "CreatedByID");
            CreateIndex("solar.Arrays", "DateCreated");
            CreateIndex("solar.Arrays", "TenantID");
            CreateIndex("solar.Arrays", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("solar.Arrays", "InverterID");
            CreateIndex("solar.Arrays", "SolarSystemID");
            CreateIndex("solar.Arrays", "SolarPanelID");
            AddForeignKey("solar.Arrays", "SolarSystemID", "solar.Systems", "Guid");
            AddForeignKey("solar.Arrays", "SolarPanelID", "solar.Panels", "Guid");
            AddForeignKey("solar.ArraySegments", "SolarArrayID", "solar.Arrays", "Guid");
            AddForeignKey("solar.ArrayPanels", "SolarArrayID", "solar.Arrays", "Guid");
            AddForeignKey("solar.Arrays", "InverterID", "solar.Inverters", "Guid");
        }
    }
}
