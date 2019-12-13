namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_solar_ProposalData_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.ProposalsData",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        FinancePlanID = c.Guid(nullable: false),
                        ProposalTemplateID = c.Guid(nullable: false),
                        ProposalDate = c.DateTime(nullable: false),
                        ContractorID = c.String(),
                        UserInputDataJSON = c.String(),
                        UserInputDataLinksJSON = c.String(),
                        DocumentDataJSON = c.String(),
                        DocumentDataLinksJSON = c.String(),
                        SalesRepID = c.Guid(nullable: false),
                        SignatureDate = c.DateTime(),
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
                .ForeignKey("solar.FinancePlans", t => t.FinancePlanID)
                .Index(t => t.FinancePlanID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("solar.ProposalsData", "FinancePlanID", "solar.FinancePlans");
            DropIndex("solar.ProposalsData", new[] { "ExternalID" });
            DropIndex("solar.ProposalsData", new[] { "Version" });
            DropIndex("solar.ProposalsData", new[] { "CreatedByID" });
            DropIndex("solar.ProposalsData", new[] { "DateCreated" });
            DropIndex("solar.ProposalsData", new[] { "TenantID" });
            DropIndex("solar.ProposalsData", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ProposalsData", new[] { "FinancePlanID" });
            DropTable("solar.ProposalsData");
        }
    }
}
