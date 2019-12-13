namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_KeyValue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KeyValues",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        ObjectID = c.Guid(nullable: false),
                        Key = c.String(nullable: false, maxLength: 50),
                        Value = c.String(),
                        Notes = c.String(),
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
                .Index(t => new { t.ObjectID, t.Key }, name: "idx_kv_primary")
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.KeyValues", new[] { "ExternalID" });
            DropIndex("dbo.KeyValues", new[] { "Version" });
            DropIndex("dbo.KeyValues", new[] { "CreatedByID" });
            DropIndex("dbo.KeyValues", new[] { "DateCreated" });
            DropIndex("dbo.KeyValues", new[] { "TenantID" });
            DropIndex("dbo.KeyValues", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.KeyValues", "idx_kv_primary");
            DropTable("dbo.KeyValues");
        }
    }
}
