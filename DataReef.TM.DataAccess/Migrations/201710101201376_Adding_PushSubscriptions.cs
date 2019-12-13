namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_PushSubscriptions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "push.Subscriptions",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        DeviceId = c.Guid(nullable: false),
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
                .ForeignKey("dbo.Devices", t => t.DeviceId)
                .Index(t => t.DeviceId)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("push.Subscriptions", "DeviceId", "dbo.Devices");
            DropIndex("push.Subscriptions", new[] { "ExternalID" });
            DropIndex("push.Subscriptions", new[] { "Version" });
            DropIndex("push.Subscriptions", new[] { "CreatedByID" });
            DropIndex("push.Subscriptions", new[] { "DateCreated" });
            DropIndex("push.Subscriptions", new[] { "TenantID" });
            DropIndex("push.Subscriptions", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("push.Subscriptions", new[] { "DeviceId" });
            DropTable("push.Subscriptions");
        }
    }
}
