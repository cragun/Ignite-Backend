namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinanceDefinitions_PropertyRatings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "finance.OUAssociation",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        FinancePlanDefinitionID = c.Guid(nullable: false),
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
                .ForeignKey("finance.PlanDefinitions", t => t.FinancePlanDefinitionID)
                .ForeignKey("dbo.OUs", t => t.OUID)
                .Index(t => t.OUID)
                .Index(t => t.FinancePlanDefinitionID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "finance.PlanDefinitions",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ProviderID = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        IsDisabled = c.Boolean(nullable: false),
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
                .ForeignKey("finance.Providers", t => t.ProviderID)
                .Index(t => t.ProviderID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "finance.FinanceDetails",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        FinancePlanDefinitionID = c.Guid(nullable: false),
                        Apr = c.Single(nullable: false),
                        Order = c.Int(nullable: false),
                        PrincipalIsPaid = c.Boolean(nullable: false),
                        ApplyPrincipalReductionAfter = c.Boolean(nullable: false),
                        Months = c.Int(nullable: false),
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
                .ForeignKey("finance.PlanDefinitions", t => t.FinancePlanDefinitionID)
                .Index(t => t.FinancePlanDefinitionID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "finance.Providers",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ImageUrl = c.String(),
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
            
            CreateTable(
                "dbo.PropertyRatings",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        RecordLocator = c.String(),
                        DisplayType = c.String(maxLength: 50),
                        Value = c.String(maxLength: 150),
                        OwnerID = c.String(maxLength: 50),
                        AttributeKey = c.String(maxLength: 50),
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
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("finance.OUAssociation", "OUID", "dbo.OUs");
            DropForeignKey("finance.PlanDefinitions", "ProviderID", "finance.Providers");
            DropForeignKey("finance.FinanceDetails", "FinancePlanDefinitionID", "finance.PlanDefinitions");
            DropForeignKey("finance.OUAssociation", "FinancePlanDefinitionID", "finance.PlanDefinitions");
            DropIndex("dbo.PropertyRatings", new[] { "ExternalID" });
            DropIndex("dbo.PropertyRatings", new[] { "Version" });
            DropIndex("dbo.PropertyRatings", new[] { "CreatedByID" });
            DropIndex("dbo.PropertyRatings", new[] { "DateCreated" });
            DropIndex("dbo.PropertyRatings", new[] { "TenantID" });
            DropIndex("dbo.PropertyRatings", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("finance.Providers", new[] { "ExternalID" });
            DropIndex("finance.Providers", new[] { "Version" });
            DropIndex("finance.Providers", new[] { "CreatedByID" });
            DropIndex("finance.Providers", new[] { "DateCreated" });
            DropIndex("finance.Providers", new[] { "TenantID" });
            DropIndex("finance.Providers", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("finance.FinanceDetails", new[] { "ExternalID" });
            DropIndex("finance.FinanceDetails", new[] { "Version" });
            DropIndex("finance.FinanceDetails", new[] { "CreatedByID" });
            DropIndex("finance.FinanceDetails", new[] { "DateCreated" });
            DropIndex("finance.FinanceDetails", new[] { "TenantID" });
            DropIndex("finance.FinanceDetails", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("finance.FinanceDetails", new[] { "FinancePlanDefinitionID" });
            DropIndex("finance.PlanDefinitions", new[] { "ExternalID" });
            DropIndex("finance.PlanDefinitions", new[] { "Version" });
            DropIndex("finance.PlanDefinitions", new[] { "CreatedByID" });
            DropIndex("finance.PlanDefinitions", new[] { "DateCreated" });
            DropIndex("finance.PlanDefinitions", new[] { "TenantID" });
            DropIndex("finance.PlanDefinitions", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("finance.PlanDefinitions", new[] { "ProviderID" });
            DropIndex("finance.OUAssociation", new[] { "ExternalID" });
            DropIndex("finance.OUAssociation", new[] { "Version" });
            DropIndex("finance.OUAssociation", new[] { "CreatedByID" });
            DropIndex("finance.OUAssociation", new[] { "DateCreated" });
            DropIndex("finance.OUAssociation", new[] { "TenantID" });
            DropIndex("finance.OUAssociation", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("finance.OUAssociation", new[] { "FinancePlanDefinitionID" });
            DropIndex("finance.OUAssociation", new[] { "OUID" });
            DropTable("dbo.PropertyRatings");
            DropTable("finance.Providers");
            DropTable("finance.FinanceDetails");
            DropTable("finance.PlanDefinitions");
            DropTable("finance.OUAssociation");
        }
    }
}
