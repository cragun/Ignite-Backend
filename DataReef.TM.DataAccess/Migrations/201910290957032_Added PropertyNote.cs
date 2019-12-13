namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertyNote : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PropertyNotes",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        PropertyID = c.Guid(nullable: false),
                        Content = c.String(),
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
                .ForeignKey("dbo.Properties", t => t.PropertyID)
                .Index(t => t.PersonID)
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
            DropForeignKey("dbo.PropertyNotes", "PropertyID", "dbo.Properties");
            DropForeignKey("dbo.PropertyNotes", "PersonID", "dbo.People");
            DropIndex("dbo.PropertyNotes", new[] { "ExternalID" });
            DropIndex("dbo.PropertyNotes", new[] { "Version" });
            DropIndex("dbo.PropertyNotes", new[] { "CreatedByID" });
            DropIndex("dbo.PropertyNotes", new[] { "DateCreated" });
            DropIndex("dbo.PropertyNotes", new[] { "TenantID" });
            DropIndex("dbo.PropertyNotes", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PropertyNotes", new[] { "PropertyID" });
            DropIndex("dbo.PropertyNotes", new[] { "PersonID" });
            DropTable("dbo.PropertyNotes");
        }
    }
}
