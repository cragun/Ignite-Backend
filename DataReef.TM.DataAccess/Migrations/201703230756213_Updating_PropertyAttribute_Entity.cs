namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Updating_PropertyAttribute_Entity : DbMigration
    {
        public override void Up()
        {
            Sql("ALTER TABLE PropertyAttributes NOCHECK CONSTRAINT ALL");

            AddColumn("dbo.PropertyAttributes", "ExpirationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PropertyAttributes", "TerritoryID", c => c.Guid());
            AddColumn("dbo.PropertyAttributes", "UserID", c => c.Guid());

            Sql(@"UPDATE dbo.PropertyAttributes
                  SET ExpirationDate = DATEADD(minute, ExpiryMinutes, DateCreated),
                      TerritoryID = (SELECT Guid FROM Territories WHERE Guid = OwnerID),
                      UserID = CreatedByID");

            DropColumn("dbo.PropertyAttributes", "OwnerID");

            Sql("ALTER TABLE PropertyAttributes WITH CHECK CHECK CONSTRAINT ALL");
        }

        public override void Down()
        {
            AddColumn("dbo.PropertyAttributes", "OwnerID", c => c.String(maxLength: 50));

            Sql(@"UPDATE dbo.PropertyAttributes
                  SET OwnerID = TerritoryID");

            DropColumn("dbo.PropertyAttributes", "UserID");
            DropColumn("dbo.PropertyAttributes", "TerritoryID");
            DropColumn("dbo.PropertyAttributes", "ExpirationDate");
        }
    }
}
