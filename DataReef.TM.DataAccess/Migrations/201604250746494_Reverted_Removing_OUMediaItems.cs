namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Reverted_Removing_OUMediaItems : DbMigration
    {
        public override void Up()
        {
            Sql("DELETE FROM [dbo].[MediaItems] \r\n GO");

            DropIndex("dbo.MediaItems", new[] { "OUID" });
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
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.MediaItems", t => t.MediaID)
                .Index(t => t.OUID)
                .Index(t => t.MediaID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            //TODO: add new data
            string sqlMigrationPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../..", "Database/SQL/Migrations/Revert_RemoveOUMediaItems.sql");
            SqlFile(sqlMigrationPath);
        }

        public override void Down()
        {
            DropForeignKey("dbo.OUMediaItems", "MediaID", "dbo.MediaItems");
            DropIndex("dbo.OUMediaItems", new[] { "ExternalID" });
            DropIndex("dbo.OUMediaItems", new[] { "Version" });
            DropIndex("dbo.OUMediaItems", new[] { "CreatedByID" });
            DropIndex("dbo.OUMediaItems", new[] { "DateCreated" });
            DropIndex("dbo.OUMediaItems", new[] { "TenantID" });
            DropIndex("dbo.OUMediaItems", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.OUMediaItems", new[] { "MediaID" });
            DropIndex("dbo.OUMediaItems", new[] { "OUID" });
            DropTable("dbo.OUMediaItems");
            CreateIndex("dbo.MediaItems", "OUID");
        }
    }
}
