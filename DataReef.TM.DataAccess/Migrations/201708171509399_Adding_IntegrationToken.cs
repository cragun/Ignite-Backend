namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_IntegrationToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IntegrationTokens",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
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
                .ForeignKey("dbo.People", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IntegrationTokens", "UserId", "dbo.People");
            DropIndex("dbo.IntegrationTokens", new[] { "ExternalID" });
            DropIndex("dbo.IntegrationTokens", new[] { "Version" });
            DropIndex("dbo.IntegrationTokens", new[] { "CreatedByID" });
            DropIndex("dbo.IntegrationTokens", new[] { "DateCreated" });
            DropIndex("dbo.IntegrationTokens", new[] { "TenantID" });
            DropIndex("dbo.IntegrationTokens", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.IntegrationTokens", new[] { "UserId" });
            DropTable("dbo.IntegrationTokens");
        }
    }
}
