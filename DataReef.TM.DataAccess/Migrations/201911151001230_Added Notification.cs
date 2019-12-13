namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedNotification : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        Description = c.String(),
                        Content = c.String(),
                        SeenAt = c.DateTime(),
                        Value = c.String(),
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
                .ForeignKey("dbo.People", t => t.PersonID)
                .Index(t => t.PersonID)
                .Index(t => t.NotificationType)
                .Index(t => t.SeenAt)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "PersonID", "dbo.People");
            DropIndex("dbo.Notifications", new[] { "ExternalID" });
            DropIndex("dbo.Notifications", new[] { "Version" });
            DropIndex("dbo.Notifications", new[] { "CreatedByID" });
            DropIndex("dbo.Notifications", new[] { "DateCreated" });
            DropIndex("dbo.Notifications", new[] { "TenantID" });
            DropIndex("dbo.Notifications", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.Notifications", new[] { "SeenAt" });
            DropIndex("dbo.Notifications", new[] { "NotificationType" });
            DropIndex("dbo.Notifications", new[] { "PersonID" });
            DropTable("dbo.Notifications");
        }
    }
}
