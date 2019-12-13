namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Velocify_Add_Request_Response : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "PRMI.VelocifyRequests",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        FirstName = c.String(),
                        MiddleInitial = c.String(),
                        LastName = c.String(),
                        NameSuffix = c.String(),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipCode = c.String(),
                        PrimaryPhone = c.String(),
                        Email = c.String(),
                        TotalSolarCost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SolarSystemDescription = c.String(),
                        ReferenceID = c.Guid(nullable: false),
                        DealerName = c.String(),
                        SalesRepName = c.String(),
                        SalesRepPhone = c.String(),
                        Avm = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IncomeTaxCredit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalMortgageStartDate = c.DateTime(nullable: false),
                        OriginalMortgageOriginalBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalMortgageCurrentBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalMortgagePaymentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OriginalMortgageInterestRate = c.Decimal(nullable: false, precision: 18, scale: 2),
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
                "PRMI.VelocifyResponses",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        VelocifyRequestID = c.Guid(nullable: false),
                        ReferenceID = c.Guid(nullable: false),
                        LeadID = c.String(),
                        Result = c.String(),
                        Message = c.String(),
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
                .ForeignKey("PRMI.VelocifyRequests", t => t.VelocifyRequestID)
                .Index(t => t.VelocifyRequestID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("PRMI.VelocifyResponses", "VelocifyRequestID", "PRMI.VelocifyRequests");
            DropIndex("PRMI.VelocifyResponses", new[] { "ExternalID" });
            DropIndex("PRMI.VelocifyResponses", new[] { "Version" });
            DropIndex("PRMI.VelocifyResponses", new[] { "CreatedByID" });
            DropIndex("PRMI.VelocifyResponses", new[] { "DateCreated" });
            DropIndex("PRMI.VelocifyResponses", new[] { "TenantID" });
            DropIndex("PRMI.VelocifyResponses", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("PRMI.VelocifyResponses", new[] { "VelocifyRequestID" });
            DropIndex("PRMI.VelocifyRequests", new[] { "ExternalID" });
            DropIndex("PRMI.VelocifyRequests", new[] { "Version" });
            DropIndex("PRMI.VelocifyRequests", new[] { "CreatedByID" });
            DropIndex("PRMI.VelocifyRequests", new[] { "DateCreated" });
            DropIndex("PRMI.VelocifyRequests", new[] { "TenantID" });
            DropIndex("PRMI.VelocifyRequests", "CLUSTERED_INDEX_ON_LONG");
            DropTable("PRMI.VelocifyResponses");
            DropTable("PRMI.VelocifyRequests");
        }
    }
}
