namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGenDocsRequestModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Spruce.GenDocsRequests",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        QuoteNumber = c.Long(nullable: false),
                        SponsorId = c.String(),
                        PartnerId = c.String(),
                        TotalCashSalesPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalesTax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CashDownPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AmountFinanced = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InstallCommencementDate = c.DateTime(nullable: false),
                        SubstantialCompletionDate = c.DateTime(nullable: false),
                        ProjectedPTODate = c.DateTime(nullable: false),
                        EmailApplicant = c.String(),
                        EmailCoapplicant = c.String(),
                        EmailAgreement = c.String(),
                        CallbackJSON = c.String(),
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
                .ForeignKey("Spruce.QuoteRequests", t => t.Guid)
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
            DropForeignKey("Spruce.GenDocsRequests", "Guid", "Spruce.QuoteRequests");
            DropIndex("Spruce.GenDocsRequests", new[] { "ExternalID" });
            DropIndex("Spruce.GenDocsRequests", new[] { "Version" });
            DropIndex("Spruce.GenDocsRequests", new[] { "CreatedByID" });
            DropIndex("Spruce.GenDocsRequests", new[] { "DateCreated" });
            DropIndex("Spruce.GenDocsRequests", new[] { "TenantID" });
            DropIndex("Spruce.GenDocsRequests", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.GenDocsRequests", new[] { "Guid" });
            DropTable("Spruce.GenDocsRequests");
        }
    }
}
