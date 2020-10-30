namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuotacommitstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuotasCommitments",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        RoleID = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        dispositions = c.String(),
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
                .ForeignKey("dbo.OURoles", t => t.RoleID)
                .Index(t => t.RoleID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            AddColumn("dbo.People", "SBLastActivityDate", c => c.DateTime());
            AddColumn("dbo.People", "SBActivityName", c => c.String(maxLength: 250));
            AddColumn("dbo.Appointments", "ESID", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuotasCommitments", "RoleID", "dbo.OURoles");
            DropIndex("dbo.QuotasCommitments", new[] { "ExternalID" });
            DropIndex("dbo.QuotasCommitments", new[] { "Version" });
            DropIndex("dbo.QuotasCommitments", new[] { "CreatedByID" });
            DropIndex("dbo.QuotasCommitments", new[] { "DateCreated" });
            DropIndex("dbo.QuotasCommitments", new[] { "TenantID" });
            DropIndex("dbo.QuotasCommitments", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.QuotasCommitments", new[] { "RoleID" });
            DropColumn("dbo.Appointments", "ESID");
            DropColumn("dbo.People", "SBActivityName");
            DropColumn("dbo.People", "SBLastActivityDate");
            DropTable("dbo.QuotasCommitments");
        }
    }
}
