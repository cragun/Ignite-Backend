namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExternalCredentials : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExternalCredentials",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        UserID = c.Guid(nullable: false),
                        RootOrganizationName = c.String(maxLength: 400),
                        Username = c.String(),
                        EncryptedPassword = c.String(),
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
                .ForeignKey("dbo.Users", t => t.UserID)
                .Index(t => new { t.UserID, t.RootOrganizationName }, name: "IdxUserOU")
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExternalCredentials", "UserID", "dbo.Users");
            DropIndex("dbo.ExternalCredentials", new[] { "ExternalID" });
            DropIndex("dbo.ExternalCredentials", new[] { "Version" });
            DropIndex("dbo.ExternalCredentials", new[] { "CreatedByID" });
            DropIndex("dbo.ExternalCredentials", new[] { "DateCreated" });
            DropIndex("dbo.ExternalCredentials", new[] { "TenantID" });
            DropIndex("dbo.ExternalCredentials", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.ExternalCredentials", "IdxUserOU");
            DropTable("dbo.ExternalCredentials");
        }
    }
}
