namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PropertyPowerConsumption : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "power.PropertyPowerConsumptions",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Year = c.Int(nullable: false),
                        Month = c.Int(nullable: false),
                        Watts = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Cost = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PropertyID = c.Guid(nullable: false),
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
            DropForeignKey("power.PropertyPowerConsumptions", "PropertyID", "dbo.Properties");
            DropIndex("power.PropertyPowerConsumptions", new[] { "ExternalID" });
            DropIndex("power.PropertyPowerConsumptions", new[] { "Version" });
            DropIndex("power.PropertyPowerConsumptions", new[] { "CreatedByID" });
            DropIndex("power.PropertyPowerConsumptions", new[] { "DateCreated" });
            DropIndex("power.PropertyPowerConsumptions", new[] { "TenantID" });
            DropIndex("power.PropertyPowerConsumptions", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("power.PropertyPowerConsumptions", new[] { "PropertyID" });
            DropTable("power.PropertyPowerConsumptions");
        }
    }
}
