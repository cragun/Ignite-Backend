namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FavouriteOus",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        PersonID = c.Guid(nullable: false),
                        OUID = c.Guid(nullable: false),
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
                .ForeignKey("dbo.OUs", t => t.OUID)
                .ForeignKey("dbo.People", t => t.PersonID)
                .Index(t => t.PersonID)
                .Index(t => t.OUID)
                .Index(t => t.Id, clustered: true, name: "CLUSTERED_INDEX_ON_LONG")
                .Index(t => t.TenantID)
                .Index(t => t.DateCreated)
                .Index(t => t.CreatedByID)
                .Index(t => t.Version)
                .Index(t => t.ExternalID);
            
            AddColumn("finance.PlanDefinitions", "LenderFee", c => c.Double());
            AddColumn("finance.PlanDefinitions", "PPW", c => c.Double());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FavouriteOus", "PersonID", "dbo.People");
            DropForeignKey("dbo.FavouriteOus", "OUID", "dbo.OUs");
            DropIndex("dbo.FavouriteOus", new[] { "ExternalID" });
            DropIndex("dbo.FavouriteOus", new[] { "Version" });
            DropIndex("dbo.FavouriteOus", new[] { "CreatedByID" });
            DropIndex("dbo.FavouriteOus", new[] { "DateCreated" });
            DropIndex("dbo.FavouriteOus", new[] { "TenantID" });
            DropIndex("dbo.FavouriteOus", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.FavouriteOus", new[] { "OUID" });
            DropIndex("dbo.FavouriteOus", new[] { "PersonID" });
            DropColumn("finance.PlanDefinitions", "PPW");
            DropColumn("finance.PlanDefinitions", "LenderFee");
            DropTable("dbo.FavouriteOus");
        }
    }
}
