namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingAppointments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                {
                    Guid = c.Guid(nullable: false),
                    PropertyID = c.Guid(nullable: false),
                    StartDate = c.DateTime(nullable: false),
                    EndDate = c.DateTime(),
                    Latitude = c.Double(),
                    Longitude = c.Double(),
                    Address = c.String(maxLength: 250),
                    Details = c.String(),
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
                .ForeignKey("dbo.Properties", t => t.PropertyID)
                .Index(t => t.PropertyID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "PropertyID", "dbo.Properties");
            DropIndex("dbo.Appointments", new[] { "ExternalID" });
            DropIndex("dbo.Appointments", new[] { "Version" });
            DropIndex("dbo.Appointments", new[] { "CreatedByID" });
            DropIndex("dbo.Appointments", new[] { "DateCreated" });
            DropIndex("dbo.Appointments", new[] { "TenantID" });
            DropIndex("dbo.Appointments", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.Appointments", new[] { "PropertyID" });
            DropTable("dbo.Appointments");
        }
    }
}
