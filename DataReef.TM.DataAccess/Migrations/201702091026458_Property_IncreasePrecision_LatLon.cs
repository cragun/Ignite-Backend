namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Property_IncreasePrecision_LatLon : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Properties", "idx_geo");
            this.DeleteDefaultContraint("dbo.ImagePurchases", "Top");
            this.DeleteDefaultContraint("dbo.ImagePurchases", "Left");
            this.DeleteDefaultContraint("dbo.ImagePurchases", "Bottom");
            this.DeleteDefaultContraint("dbo.ImagePurchases", "Right");

            AlterColumn("dbo.Properties", "Latitude", c => c.Double());
            AlterColumn("dbo.Properties", "Longitude", c => c.Double());
            AlterColumn("dbo.ImagePurchases", "Lat", c => c.Double(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Lon", c => c.Double(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Top", c => c.Double(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Left", c => c.Double(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Bottom", c => c.Double(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Right", c => c.Double(nullable: false));

            Sql(@"CREATE NONCLUSTERED INDEX [idx_geo] ON [dbo].[Properties]
                (
	                [TerritoryID] ASC,
	                [Latitude] ASC,
	                [Longitude] ASC,
	                [Id] ASC
                )
                INCLUDE ( 	[Guid]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }

        public override void Down()
        {
            DropIndex("dbo.Properties", "idx_geo");

            AlterColumn("dbo.ImagePurchases", "Right", c => c.Single(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Bottom", c => c.Single(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Left", c => c.Single(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Top", c => c.Single(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Lon", c => c.Single(nullable: false));
            AlterColumn("dbo.ImagePurchases", "Lat", c => c.Single(nullable: false));
            AlterColumn("dbo.Properties", "Longitude", c => c.Single());
            AlterColumn("dbo.Properties", "Latitude", c => c.Single());

            Sql(@"CREATE NONCLUSTERED INDEX [idx_geo] ON [dbo].[Properties]
                (
	                [TerritoryID] ASC,
	                [Latitude] ASC,
	                [Longitude] ASC,
	                [Id] ASC
                )
                INCLUDE ( 	[Guid]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }
    }
}
