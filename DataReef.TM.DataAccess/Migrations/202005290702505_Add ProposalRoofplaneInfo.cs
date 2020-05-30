namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProposalRoofplaneInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.ProposalRoofPlaneInfo",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ArrayName = c.String(),
                        Size = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TargetOffset = c.Int(nullable: false),
                        ArrayOffset = c.Int(nullable: false),
                        Tilt = c.Int(nullable: false),
                        Azimuth = c.Int(nullable: false),
                        ProfileName = c.String(),
                        ProviderProfileId = c.String(),
                        Losses = c.Double(nullable: false),
                        Shading = c.Double(nullable: false),
                        InverterEfficiency = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PanelsEfficiency = c.Double(nullable: false),
                        ProposalId = c.Guid(nullable: false),
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
                .ForeignKey("solar.Proposals", t => t.ProposalId)
                .Index(t => t.ProposalId)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            AddColumn("dbo.PropertyNotes", "ParentID", c => c.Guid(nullable: false));
            AddColumn("dbo.PropertyNotes", "ContentType", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("solar.ProposalRoofPlaneInfo", "ProposalId", "solar.Proposals");
            DropIndex("solar.ProposalRoofPlaneInfo", new[] { "ExternalID" });
            DropIndex("solar.ProposalRoofPlaneInfo", new[] { "Version" });
            DropIndex("solar.ProposalRoofPlaneInfo", new[] { "CreatedByID" });
            DropIndex("solar.ProposalRoofPlaneInfo", new[] { "DateCreated" });
            DropIndex("solar.ProposalRoofPlaneInfo", new[] { "TenantID" });
            DropIndex("solar.ProposalRoofPlaneInfo", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ProposalRoofPlaneInfo", new[] { "ProposalId" });
            DropColumn("dbo.PropertyNotes", "ContentType");
            DropColumn("dbo.PropertyNotes", "ParentID");
            DropTable("solar.ProposalRoofPlaneInfo");
        }
    }
}
