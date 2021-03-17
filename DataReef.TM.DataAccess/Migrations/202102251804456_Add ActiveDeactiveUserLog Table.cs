namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveDeactiveUserLogTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActiveDeactiveUserLogs",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Username = c.String(),
                        OldState = c.String(),
                        NewState = c.String(),
                        Changer = c.String(),
                        Reason = c.String(),
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
            DropIndex("dbo.ActiveDeactiveUserLogs", new[] { "ExternalID" });
            DropIndex("dbo.ActiveDeactiveUserLogs", new[] { "Version" });
            DropIndex("dbo.ActiveDeactiveUserLogs", new[] { "CreatedByID" });
            DropIndex("dbo.ActiveDeactiveUserLogs", new[] { "DateCreated" });
            DropIndex("dbo.ActiveDeactiveUserLogs", new[] { "TenantID" });
            DropIndex("dbo.ActiveDeactiveUserLogs", "CLUSTERED_INDEX_ON_LONG");
            DropTable("dbo.ActiveDeactiveUserLogs");
        }
    }
}
