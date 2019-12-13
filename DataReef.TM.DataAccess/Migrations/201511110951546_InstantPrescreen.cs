namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InstantPrescreen : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PrescreenInstants",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Status = c.Int(nullable: false),
                        CompletionDate = c.DateTime(),
                        ErrorString = c.String(),
                        PropertyID = c.Guid(nullable: false),
                        Reference = c.String(maxLength: 150),
                        CreditCategory = c.String(),
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
            
            AddColumn("dbo.TokenExpenses", "BatchPrescreenID", c => c.Guid());
            AddColumn("dbo.TokenExpenses", "InstantPrescreenID", c => c.Guid());
            DropColumn("dbo.TokenExpenses", "BatchID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TokenExpenses", "BatchID", c => c.Guid(nullable: false));
            DropForeignKey("dbo.PrescreenInstants", "PropertyID", "dbo.Properties");
            DropIndex("dbo.PrescreenInstants", new[] { "ExternalID" });
            DropIndex("dbo.PrescreenInstants", new[] { "Version" });
            DropIndex("dbo.PrescreenInstants", new[] { "CreatedByID" });
            DropIndex("dbo.PrescreenInstants", new[] { "DateCreated" });
            DropIndex("dbo.PrescreenInstants", new[] { "TenantID" });
            DropIndex("dbo.PrescreenInstants", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.PrescreenInstants", new[] { "PropertyID" });
            DropColumn("dbo.TokenExpenses", "InstantPrescreenID");
            DropColumn("dbo.TokenExpenses", "BatchPrescreenID");
            DropTable("dbo.PrescreenInstants");
        }
    }
}
