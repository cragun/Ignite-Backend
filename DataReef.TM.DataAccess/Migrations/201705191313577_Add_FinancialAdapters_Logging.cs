namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FinancialAdapters_Logging : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "FI.AdapterRequests",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        AdapterName = c.String(),
                        Request = c.String(),
                        Response = c.String(),
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
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropIndex("FI.AdapterRequests", new[] { "ExternalID" });
            DropIndex("FI.AdapterRequests", new[] { "Version" });
            DropIndex("FI.AdapterRequests", new[] { "CreatedByID" });
            DropIndex("FI.AdapterRequests", new[] { "DateCreated" });
            DropIndex("FI.AdapterRequests", new[] { "TenantID" });
            DropIndex("FI.AdapterRequests", "CLUSTERED_INDEX_ON_LONG");
            DropTable("FI.AdapterRequests");
        }
    }
}
