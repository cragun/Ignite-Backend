namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManualProposalTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.ManualProposal",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        TotalBill = c.Double(nullable: false),
                        TotalKWH = c.Double(nullable: false),
                        IsManual = c.Boolean(nullable: false),
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
            DropIndex("solar.ManualProposal", new[] { "ExternalID" });
            DropIndex("solar.ManualProposal", new[] { "Version" });
            DropIndex("solar.ManualProposal", new[] { "CreatedByID" });
            DropIndex("solar.ManualProposal", new[] { "DateCreated" });
            DropIndex("solar.ManualProposal", new[] { "TenantID" });
            DropIndex("solar.ManualProposal", "CLUSTERED_INDEX_ON_LONG");
            DropTable("solar.ManualProposal");
        }
    }
}
