namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_ProposalDocuments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.ProposalDocuments",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        DocumentID = c.String(maxLength: 250),
                        DocumentType = c.Int(nullable: false),
                        SignedURL = c.String(),
                        UnsignedURL = c.String(),
                        SignedDate = c.DateTime(),
                        ExpiryDate = c.DateTime(),
                        FinancePlanID = c.Guid(nullable: false),
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
                .Index(t => t.DocumentID, name: "IDX_DocumenID")
                .Index(t => t.FinancePlanID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);

            // call sql script to migrate existing data to new structure
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Documents_migration.sql");
            SqlFile(sqlMigrationPath);
        }
        
        public override void Down()
        {
            DropForeignKey("solar.ProposalDocuments", "FinancePlanID", "solar.FinancePlans");
            DropIndex("solar.ProposalDocuments", new[] { "ExternalID" });
            DropIndex("solar.ProposalDocuments", new[] { "Version" });
            DropIndex("solar.ProposalDocuments", new[] { "CreatedByID" });
            DropIndex("solar.ProposalDocuments", new[] { "DateCreated" });
            DropIndex("solar.ProposalDocuments", new[] { "TenantID" });
            DropIndex("solar.ProposalDocuments", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ProposalDocuments", new[] { "FinancePlanID" });
            DropIndex("solar.ProposalDocuments", "IDX_DocumenID");
            DropTable("solar.ProposalDocuments");
        }
    }
}
