namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonClockTime : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonClockTime",
                c => new
                {
                    Guid = c.Guid(nullable: false),
                    PersonID = c.Guid(nullable: false),
                    ClockType = c.String(),
                    ClockDiff = c.Long(nullable: false),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
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
                    ClockMin = c.Long(nullable: false),
                    ClockHours = c.Long(nullable: false)
                })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.Users", t => t.PersonID)
                .Index(t => t.PersonID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.PersonClockTime", "PersonID", "dbo.Users");
            DropIndex("dbo.PersonClockTime", new[] { "ExternalID" });
            DropIndex("dbo.PersonClockTime", new[] { "Version" });
            DropIndex("dbo.PersonClockTime", new[] { "CreatedByID" });
            DropIndex("dbo.PersonClockTime", new[] { "DateCreated" });
            DropIndex("dbo.PersonClockTime", new[] { "TenantID" });
            DropIndex("dbo.PersonClockTime", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PersonClockTime", new[] { "PersonID" });
            DropTable("dbo.PersonClockTime");
        }
    }
}
