namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    
    public partial class RemoveOUMediaItems : DbMigration
    {
        public override void Up()
        {
            string sqlMigrationPath = String.Format("{0}/{1}/{2}", AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/RemoveOUMediaItems.sql");
            Sql(File.ReadAllText(sqlMigrationPath));

            DropForeignKey("dbo.OUMediaItems", "MediaID", "dbo.MediaItems");
            DropForeignKey("dbo.Authentications", "UserID", "dbo.Users");
            DropIndex("dbo.OUMediaItems", new[] { "OUID" });
            DropIndex("dbo.OUMediaItems", new[] { "MediaID" });
            DropIndex("dbo.OUMediaItems", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUMediaItems", new[] { "TenantID" });
            DropIndex("dbo.OUMediaItems", new[] { "DateCreated" });
            DropIndex("dbo.OUMediaItems", new[] { "CreatedByID" });
            DropIndex("dbo.OUMediaItems", new[] { "Version" });
            DropIndex("dbo.OUMediaItems", new[] { "ExternalID" });
            CreateIndex("dbo.MediaItems", "OUID");
            DropTable("dbo.OUMediaItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.OUMediaItems",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
                        MediaID = c.Guid(nullable: false),
                        ParentId = c.Guid(),
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
            
            DropIndex("dbo.MediaItems", new[] { "OUID" });
            CreateIndex("dbo.OUMediaItems", "ExternalID");
            CreateIndex("dbo.OUMediaItems", "Version");
            CreateIndex("dbo.OUMediaItems", "CreatedByID");
            CreateIndex("dbo.OUMediaItems", "DateCreated");
            CreateIndex("dbo.OUMediaItems", "TenantID");
            CreateIndex("dbo.OUMediaItems", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.OUMediaItems", "MediaID");
            CreateIndex("dbo.OUMediaItems", "OUID");
            AddForeignKey("dbo.Authentications", "UserID", "dbo.Users", "Guid");
            AddForeignKey("dbo.OUMediaItems", "MediaID", "dbo.MediaItems", "Guid");
        }
    }
}
