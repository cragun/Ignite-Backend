namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_commerce_orders_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "commerce.Orders",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        InitialRecords = c.Int(nullable: false),
                        Results = c.Int(nullable: false),
                        PrescreenBatchId = c.Guid(),
                        CSVFilePath = c.String(),
                        OrderType = c.Int(nullable: false),
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
            DropIndex("commerce.Orders", new[] { "ExternalID" });
            DropIndex("commerce.Orders", new[] { "Version" });
            DropIndex("commerce.Orders", new[] { "CreatedByID" });
            DropIndex("commerce.Orders", new[] { "DateCreated" });
            DropIndex("commerce.Orders", new[] { "TenantID" });
            DropIndex("commerce.Orders", "CLUSTERED_INDEX_ON_LONG");
            DropTable("commerce.Orders");
        }
    }
}
