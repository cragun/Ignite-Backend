namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_OrderDetails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "commerce.OrderDetails",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        OrderID = c.Guid(nullable: false),
                        Json = c.String(),
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
                .ForeignKey("commerce.Orders", t => t.OrderID)
                .Index(t => t.OrderID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("commerce.OrderDetails", "OrderID", "commerce.Orders");
            DropIndex("commerce.OrderDetails", new[] { "ExternalID" });
            DropIndex("commerce.OrderDetails", new[] { "Version" });
            DropIndex("commerce.OrderDetails", new[] { "CreatedByID" });
            DropIndex("commerce.OrderDetails", new[] { "DateCreated" });
            DropIndex("commerce.OrderDetails", new[] { "TenantID" });
            DropIndex("commerce.OrderDetails", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("commerce.OrderDetails", new[] { "OrderID" });
            DropTable("commerce.OrderDetails");
        }
    }
}
