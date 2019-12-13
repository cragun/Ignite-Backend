namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HardCreditCheck : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Spruce.Applicants",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        NamePrefix = c.String(),
                        NameSuffix = c.String(),
                        FirstName = c.String(maxLength: 24),
                        MiddleName = c.String(maxLength: 15),
                        LastName = c.String(maxLength: 24),
                        VerifyLicense = c.Boolean(nullable: false),
                        BirthDate = c.DateTime(nullable: false),
                        SSN = c.String(maxLength: 9),
                        InstallationAddress = c.String(maxLength: 35),
                        InstallationCity = c.String(maxLength: 25),
                        InstallationState = c.String(maxLength: 2),
                        InstallationZipCode = c.String(maxLength: 5),
                        IsMailingDifferentInstall = c.Boolean(nullable: false),
                        MailingAddressLine1 = c.String(maxLength: 35),
                        MailingAddressLine2 = c.String(maxLength: 35),
                        MailingCity = c.String(maxLength: 25),
                        MailingState = c.String(maxLength: 2),
                        MailingZipCode = c.String(maxLength: 5),
                        Email = c.String(),
                        HomePhone = c.String(),
                        CellPhone = c.String(),
                        HasCoApplicant = c.Boolean(nullable: false),
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
            
            CreateTable(
                "Spruce.QuoteRequests",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        LegionPropertyID = c.Guid(nullable: false),
                        AppEmploymentID = c.Guid(nullable: false),
                        CoAppEmploymentID = c.Guid(nullable: false),
                        AppIncomeDebtID = c.Guid(nullable: false),
                        CoAppIncomeDebtID = c.Guid(nullable: false),
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
                .ForeignKey("Spruce.Employments", t => t.AppEmploymentID)
                .ForeignKey("Spruce.IncomeDebts", t => t.AppIncomeDebtID)
                .ForeignKey("Spruce.Employments", t => t.CoAppEmploymentID)
                .ForeignKey("Spruce.IncomeDebts", t => t.CoAppIncomeDebtID)
                .ForeignKey("dbo.Properties", t => t.LegionPropertyID)
                .Index(t => t.LegionPropertyID)
                .Index(t => t.AppEmploymentID)
                .Index(t => t.CoAppEmploymentID)
                .Index(t => t.AppIncomeDebtID)
                .Index(t => t.CoAppIncomeDebtID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            CreateTable(
                "Spruce.Employments",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        EmploymentStatus = c.String(),
                        EmployedSince = c.DateTime(nullable: false),
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
                "Spruce.IncomeDebts",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        AnnualIncome = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                "Spruce.CoApplicants",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        NamePrefix = c.String(),
                        NameSuffix = c.String(),
                        FirstName = c.String(maxLength: 24),
                        MiddleName = c.String(maxLength: 15),
                        LastName = c.String(maxLength: 24),
                        VerifyLicense = c.Boolean(nullable: false),
                        BirthDate = c.DateTime(nullable: false),
                        SSN = c.String(maxLength: 9),
                        IsCoAppDifferentMailing = c.Boolean(nullable: false),
                        MailingAddressLine1 = c.String(maxLength: 35),
                        MailingAddressLine2 = c.String(maxLength: 35),
                        MailingCity = c.String(maxLength: 25),
                        MailingState = c.String(maxLength: 2),
                        MailingZipCode = c.String(maxLength: 5),
                        Email = c.String(),
                        HomePhone = c.String(),
                        CellPhone = c.String(),
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
            
            CreateTable(
                "Spruce.Properties",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        MonthlyMortgagePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TitleHolder = c.String(),
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
            
            CreateTable(
                "Spruce.QuoteResponses",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        QuoteNumber = c.Long(nullable: false),
                        Decision = c.String(),
                        CreditResponse = c.String(),
                        DecisionDateTime = c.DateTime(nullable: false),
                        AmountFinanced = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LoanRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Term = c.Int(nullable: false),
                        IntroRatePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IntroTerm = c.Int(nullable: false),
                        MonthlyPayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        GotoTerm = c.Int(nullable: false),
                        StipulationText = c.String(),
                        MaxApproved = c.Decimal(nullable: false, precision: 18, scale: 2),
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
            
            CreateTable(
                "Spruce.Inits",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ContractorId = c.String(),
                        Product = c.String(),
                        Partner = c.String(),
                        Program = c.String(),
                        ContractorQuoteId1 = c.String(),
                        ContractorQuoteId2 = c.String(),
                        RateType = c.String(),
                        FinOptId = c.String(),
                        TermInMonths = c.String(),
                        CashSalesPrice = c.String(),
                        DownPayment = c.String(),
                        AmountFinanced = c.String(),
                        PrefAgreementTypeId = c.String(),
                        DeliveryMethodId = c.String(),
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
            DropForeignKey("Spruce.Inits", "Guid", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.QuoteResponses", "Guid", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.Properties", "Guid", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.QuoteRequests", "LegionPropertyID", "dbo.Properties");
            DropForeignKey("Spruce.CoApplicants", "Guid", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.QuoteRequests", "CoAppIncomeDebtID", "Spruce.IncomeDebts");
            DropForeignKey("Spruce.QuoteRequests", "CoAppEmploymentID", "Spruce.Employments");
            DropForeignKey("Spruce.Applicants", "Guid", "Spruce.QuoteRequests");
            DropForeignKey("Spruce.QuoteRequests", "AppIncomeDebtID", "Spruce.IncomeDebts");
            DropForeignKey("Spruce.QuoteRequests", "AppEmploymentID", "Spruce.Employments");
            DropIndex("Spruce.Inits", new[] { "ExternalID" });
            DropIndex("Spruce.Inits", new[] { "Version" });
            DropIndex("Spruce.Inits", new[] { "CreatedByID" });
            DropIndex("Spruce.Inits", new[] { "DateCreated" });
            DropIndex("Spruce.Inits", new[] { "TenantID" });
            DropIndex("Spruce.Inits", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.Inits", new[] { "Guid" });
            DropIndex("Spruce.QuoteResponses", new[] { "ExternalID" });
            DropIndex("Spruce.QuoteResponses", new[] { "Version" });
            DropIndex("Spruce.QuoteResponses", new[] { "CreatedByID" });
            DropIndex("Spruce.QuoteResponses", new[] { "DateCreated" });
            DropIndex("Spruce.QuoteResponses", new[] { "TenantID" });
            DropIndex("Spruce.QuoteResponses", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.QuoteResponses", new[] { "Guid" });
            DropIndex("Spruce.Properties", new[] { "ExternalID" });
            DropIndex("Spruce.Properties", new[] { "Version" });
            DropIndex("Spruce.Properties", new[] { "CreatedByID" });
            DropIndex("Spruce.Properties", new[] { "DateCreated" });
            DropIndex("Spruce.Properties", new[] { "TenantID" });
            DropIndex("Spruce.Properties", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.Properties", new[] { "Guid" });
            DropIndex("Spruce.CoApplicants", new[] { "ExternalID" });
            DropIndex("Spruce.CoApplicants", new[] { "Version" });
            DropIndex("Spruce.CoApplicants", new[] { "CreatedByID" });
            DropIndex("Spruce.CoApplicants", new[] { "DateCreated" });
            DropIndex("Spruce.CoApplicants", new[] { "TenantID" });
            DropIndex("Spruce.CoApplicants", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.CoApplicants", new[] { "Guid" });
            DropIndex("Spruce.IncomeDebts", new[] { "ExternalID" });
            DropIndex("Spruce.IncomeDebts", new[] { "Version" });
            DropIndex("Spruce.IncomeDebts", new[] { "CreatedByID" });
            DropIndex("Spruce.IncomeDebts", new[] { "DateCreated" });
            DropIndex("Spruce.IncomeDebts", new[] { "TenantID" });
            DropIndex("Spruce.IncomeDebts", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.Employments", new[] { "ExternalID" });
            DropIndex("Spruce.Employments", new[] { "Version" });
            DropIndex("Spruce.Employments", new[] { "CreatedByID" });
            DropIndex("Spruce.Employments", new[] { "DateCreated" });
            DropIndex("Spruce.Employments", new[] { "TenantID" });
            DropIndex("Spruce.Employments", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.QuoteRequests", new[] { "ExternalID" });
            DropIndex("Spruce.QuoteRequests", new[] { "Version" });
            DropIndex("Spruce.QuoteRequests", new[] { "CreatedByID" });
            DropIndex("Spruce.QuoteRequests", new[] { "DateCreated" });
            DropIndex("Spruce.QuoteRequests", new[] { "TenantID" });
            DropIndex("Spruce.QuoteRequests", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.QuoteRequests", new[] { "CoAppIncomeDebtID" });
            DropIndex("Spruce.QuoteRequests", new[] { "AppIncomeDebtID" });
            DropIndex("Spruce.QuoteRequests", new[] { "CoAppEmploymentID" });
            DropIndex("Spruce.QuoteRequests", new[] { "AppEmploymentID" });
            DropIndex("Spruce.QuoteRequests", new[] { "LegionPropertyID" });
            DropIndex("Spruce.Applicants", new[] { "ExternalID" });
            DropIndex("Spruce.Applicants", new[] { "Version" });
            DropIndex("Spruce.Applicants", new[] { "CreatedByID" });
            DropIndex("Spruce.Applicants", new[] { "DateCreated" });
            DropIndex("Spruce.Applicants", new[] { "TenantID" });
            DropIndex("Spruce.Applicants", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("Spruce.Applicants", new[] { "Guid" });
            DropTable("Spruce.Inits");
            DropTable("Spruce.QuoteResponses");
            DropTable("Spruce.Properties");
            DropTable("Spruce.CoApplicants");
            DropTable("Spruce.IncomeDebts");
            DropTable("Spruce.Employments");
            DropTable("Spruce.QuoteRequests");
            DropTable("Spruce.Applicants");
        }
    }
}
