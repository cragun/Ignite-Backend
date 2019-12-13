namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedthePropertySurveyentity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PropertySurveys",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        PropertyID = c.Guid(nullable: false),
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
                        Person_Guid = c.Guid(),
                        Property_Guid = c.Guid(),
                    })
                .PrimaryKey(t => t.Guid)
                .ForeignKey("dbo.People", t => t.Person_Guid)
                .ForeignKey("dbo.Properties", t => t.Property_Guid)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID)
                .Index(t => t.Person_Guid)
                .Index(t => t.Property_Guid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PropertySurveys", "Property_Guid", "dbo.Properties");
            DropForeignKey("dbo.PropertySurveys", "Person_Guid", "dbo.People");
            DropIndex("dbo.PropertySurveys", new[] { "Property_Guid" });
            DropIndex("dbo.PropertySurveys", new[] { "Person_Guid" });
            DropIndex("dbo.PropertySurveys", new[] { "ExternalID" });
            DropIndex("dbo.PropertySurveys", new[] { "Version" });
            DropIndex("dbo.PropertySurveys", new[] { "CreatedByID" });
            DropIndex("dbo.PropertySurveys", new[] { "DateCreated" });
            DropIndex("dbo.PropertySurveys", new[] { "TenantID" });
            DropIndex("dbo.PropertySurveys", "CLUSTERED_INDEX_ON_LONG");
            DropTable("dbo.PropertySurveys");
        }
    }
}
