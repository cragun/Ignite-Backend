namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFavouriteTerritory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavouriteTerritories",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        TerritoryID = c.Guid(nullable: false),
                        isFavourite = c.Boolean(nullable: false),
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
                .ForeignKey("dbo.Territories", t => t.TerritoryID)
                .ForeignKey("dbo.People", t => t.PersonID)
                .Index(t => t.PersonID)
                .Index(t => t.TerritoryID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavouriteTerritories", "PersonID", "dbo.People");
            DropForeignKey("dbo.FavouriteTerritories", "TerritoryID", "dbo.Territories");
            DropIndex("dbo.FavouriteTerritories", new[] { "ExternalID" });
            DropIndex("dbo.FavouriteTerritories", new[] { "Version" });
            DropIndex("dbo.FavouriteTerritories", new[] { "CreatedByID" });
            DropIndex("dbo.FavouriteTerritories", new[] { "DateCreated" });
            DropIndex("dbo.FavouriteTerritories", new[] { "TenantID" });
            DropIndex("dbo.FavouriteTerritories", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.FavouriteTerritories", new[] { "TerritoryID" });
            DropIndex("dbo.FavouriteTerritories", new[] { "PersonID" });
            DropTable("dbo.FavouriteTerritories");
        }
    }
}
