namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_ProposalMediaItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "solar.ProposalMediaItems",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ProposalID = c.Guid(nullable: false),
                        Url = c.String(maxLength: 2048),
                        ThumbUrl = c.String(maxLength: 2048),
                        Notes = c.String(),
                        MimeType = c.String(maxLength: 128),
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
                .ForeignKey("solar.Proposals", t => t.ProposalID)
                .Index(t => t.ProposalID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("solar.ProposalMediaItems", "ProposalID", "solar.Proposals");
            DropIndex("solar.ProposalMediaItems", new[] { "ExternalID" });
            DropIndex("solar.ProposalMediaItems", new[] { "Version" });
            DropIndex("solar.ProposalMediaItems", new[] { "CreatedByID" });
            DropIndex("solar.ProposalMediaItems", new[] { "DateCreated" });
            DropIndex("solar.ProposalMediaItems", new[] { "TenantID" });
            DropIndex("solar.ProposalMediaItems", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("solar.ProposalMediaItems", new[] { "ProposalID" });
            DropTable("solar.ProposalMediaItems");
        }
    }
}
