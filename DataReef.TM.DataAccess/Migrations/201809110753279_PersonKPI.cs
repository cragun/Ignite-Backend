namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PersonKPI : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PersonKPIs",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
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
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonKPIs", "PersonID", "dbo.People");
            DropIndex("dbo.PersonKPIs", new[] { "ExternalID" });
            DropIndex("dbo.PersonKPIs", new[] { "Version" });
            DropIndex("dbo.PersonKPIs", new[] { "CreatedByID" });
            DropIndex("dbo.PersonKPIs", new[] { "DateCreated" });
            DropIndex("dbo.PersonKPIs", new[] { "TenantID" });
            DropIndex("dbo.PersonKPIs", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PersonKPIs", new[] { "PersonID" });
            DropTable("dbo.PersonKPIs");
        }
    }
}
