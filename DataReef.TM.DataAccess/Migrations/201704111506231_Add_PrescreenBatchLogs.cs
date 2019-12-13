namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PrescreenBatchLogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrescreenBatchLogs",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PrescreenBatchID = c.Guid(nullable: false),
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
                .ForeignKey("dbo.PrescreenBatches", t => t.PrescreenBatchID)
                .Index(t => t.PrescreenBatchID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PrescreenBatchLogs", "PrescreenBatchID", "dbo.PrescreenBatches");
            DropIndex("dbo.PrescreenBatchLogs", new[] { "ExternalID" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "Version" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "CreatedByID" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "DateCreated" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "TenantID" });
            DropIndex("dbo.PrescreenBatchLogs", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PrescreenBatchLogs", new[] { "PrescreenBatchID" });
            DropTable("dbo.PrescreenBatchLogs");
        }
    }
}
