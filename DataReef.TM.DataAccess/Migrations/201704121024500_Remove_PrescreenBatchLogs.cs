namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_PrescreenBatchLogs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PrescreenBatchLogs", "PrescreenBatchID", "dbo.PrescreenBatches");
            DropIndex("dbo.PrescreenBatchLogs", new[] { "PrescreenBatchID" });
            DropIndex("dbo.PrescreenBatchLogs", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PrescreenBatchLogs", new[] { "TenantID" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "DateCreated" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "CreatedByID" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "Version" });
            DropIndex("dbo.PrescreenBatchLogs", new[] { "ExternalID" });
            DropTable("dbo.PrescreenBatchLogs");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Guid);
            
            CreateIndex("dbo.PrescreenBatchLogs", "ExternalID");
            CreateIndex("dbo.PrescreenBatchLogs", "Version");
            CreateIndex("dbo.PrescreenBatchLogs", "CreatedByID");
            CreateIndex("dbo.PrescreenBatchLogs", "DateCreated");
            CreateIndex("dbo.PrescreenBatchLogs", "TenantID");
            CreateIndex("dbo.PrescreenBatchLogs", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.PrescreenBatchLogs", "PrescreenBatchID");
            AddForeignKey("dbo.PrescreenBatchLogs", "PrescreenBatchID", "dbo.PrescreenBatches", "Guid");
        }
    }
}
