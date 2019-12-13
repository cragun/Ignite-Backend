namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PropertiesRelatedCleanup : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Counties", "ParentID", "dbo.States");
            DropForeignKey("dbo.CensusTracts", "ParentID", "dbo.Counties");
            DropForeignKey("dbo.BlockGroups", "ParentID", "dbo.CensusTracts");
            DropForeignKey("dbo.Blocks", "ParentID", "dbo.BlockGroups");
            DropForeignKey("dbo.Properties", "ParentID", "dbo.Blocks");
            DropIndex("dbo.Properties", new[] { "ParentID" });
            DropIndex("dbo.Blocks", new[] { "ParentID" });
            DropIndex("dbo.Blocks", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.Blocks", new[] { "TenantID" });
            DropIndex("dbo.Blocks", new[] { "DateCreated" });
            DropIndex("dbo.Blocks", new[] { "CreatedByID" });
            DropIndex("dbo.Blocks", new[] { "Version" });
            DropIndex("dbo.Blocks", new[] { "ExternalID" });
            DropIndex("dbo.BlockGroups", new[] { "ParentID" });
            DropIndex("dbo.BlockGroups", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.BlockGroups", new[] { "TenantID" });
            DropIndex("dbo.BlockGroups", new[] { "DateCreated" });
            DropIndex("dbo.BlockGroups", new[] { "CreatedByID" });
            DropIndex("dbo.BlockGroups", new[] { "Version" });
            DropIndex("dbo.BlockGroups", new[] { "ExternalID" });
            DropIndex("dbo.CensusTracts", new[] { "ParentID" });
            DropIndex("dbo.CensusTracts", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.CensusTracts", new[] { "TenantID" });
            DropIndex("dbo.CensusTracts", new[] { "DateCreated" });
            DropIndex("dbo.CensusTracts", new[] { "CreatedByID" });
            DropIndex("dbo.CensusTracts", new[] { "Version" });
            DropIndex("dbo.CensusTracts", new[] { "ExternalID" });
            DropIndex("dbo.Counties", new[] { "ParentID" });
            DropIndex("dbo.Counties", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.Counties", new[] { "TenantID" });
            DropIndex("dbo.Counties", new[] { "DateCreated" });
            DropIndex("dbo.Counties", new[] { "CreatedByID" });
            DropIndex("dbo.Counties", new[] { "Version" });
            DropIndex("dbo.Counties", new[] { "ExternalID" });
            DropIndex("dbo.States", "CLUSTERED_INDEX_ON_LONG");
            DropIndex("dbo.States", new[] { "TenantID" });
            DropIndex("dbo.States", new[] { "DateCreated" });
            DropIndex("dbo.States", new[] { "CreatedByID" });
            DropIndex("dbo.States", new[] { "Version" });
            DropIndex("dbo.States", new[] { "ExternalID" });
            AlterColumn("dbo.Occupants", "MiddleInitial", c => c.String(maxLength: 2));
            AlterColumn("dbo.Occupants", "LastNameSuffix", c => c.String(maxLength: 3));
            DropColumn("dbo.Properties", "AddressID");
            DropColumn("dbo.Properties", "PropertyType");
            DropColumn("dbo.Properties", "Abbreviation");
            DropColumn("dbo.Properties", "CensusID");
            DropColumn("dbo.Properties", "NWLatitude");
            DropColumn("dbo.Properties", "NWLongitude");
            DropColumn("dbo.Properties", "SELatitude");
            DropColumn("dbo.Properties", "SELongitude");
            DropColumn("dbo.Properties", "ParentID");
            DropColumn("dbo.Properties", "Centroid");
            DropColumn("dbo.Properties", "Shape");
            DropColumn("dbo.Properties", "ShapeReduced");
            DropColumn("dbo.Properties", "BoundingBox");
            DropColumn("dbo.Properties", "HousingCount");
            DropColumn("dbo.Properties", "ResidentCount");
            DropColumn("dbo.Properties", "Radius");
            DropColumn("dbo.Properties", "QuadKey1");
            DropColumn("dbo.Properties", "QuadKey2");
            DropColumn("dbo.Properties", "QuadKey3");
            DropColumn("dbo.Properties", "QuadKey4");
            DropColumn("dbo.Properties", "QuadKey5");
            DropColumn("dbo.Properties", "QuadKey6");
            DropColumn("dbo.Properties", "QuadKey7");
            DropColumn("dbo.Properties", "QuadKey8");
            DropColumn("dbo.Properties", "QuadKey9");
            DropColumn("dbo.Properties", "QuadKey10");
            DropColumn("dbo.Properties", "QuadKey11");
            DropColumn("dbo.Properties", "QuadKey12");
            DropColumn("dbo.Properties", "QuadKey13");
            DropColumn("dbo.Properties", "QuadKey14");
            DropColumn("dbo.Properties", "QuadKey15");
            DropColumn("dbo.Properties", "QuadKey16");
            DropColumn("dbo.Properties", "QuadKey17");
            DropColumn("dbo.Properties", "QuadKey18");
            DropColumn("dbo.Properties", "QuadKey19");
            DropColumn("dbo.Properties", "QuadKey20");
            DropColumn("dbo.Properties", "QuadKey21");
            DropColumn("dbo.Properties", "QuadKey22");
            DropColumn("dbo.Properties", "QuadKey23");
            DropColumn("dbo.Properties", "QuadKey24");
            DropColumn("dbo.PropertyAttributes", "Description");
            DropColumn("dbo.Fields", "Description");
            DropColumn("dbo.Fields", "GroupName");
            DropColumn("dbo.Fields", "FormatString");
            DropTable("dbo.Blocks");
            DropTable("dbo.BlockGroups");
            DropTable("dbo.CensusTracts");
            DropTable("dbo.Counties");
            DropTable("dbo.States");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.States",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Abbreviation = c.String(maxLength: 50),
                        CensusID = c.String(maxLength: 50),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                        NWLatitude = c.Single(),
                        NWLongitude = c.Single(),
                        SELatitude = c.Single(),
                        SELongitude = c.Single(),
                        ParentID = c.Guid(),
                        Centroid = c.Geography(),
                        Shape = c.Geography(),
                        ShapeReduced = c.Geography(),
                        BoundingBox = c.Geography(),
                        HousingCount = c.Int(nullable: false),
                        ResidentCount = c.Int(nullable: false),
                        Radius = c.Single(nullable: false),
                        QuadKey1 = c.String(maxLength: 1),
                        QuadKey2 = c.String(maxLength: 2),
                        QuadKey3 = c.String(maxLength: 3),
                        QuadKey4 = c.String(maxLength: 4),
                        QuadKey5 = c.String(maxLength: 5),
                        QuadKey6 = c.String(maxLength: 6),
                        QuadKey7 = c.String(maxLength: 7),
                        QuadKey8 = c.String(maxLength: 8),
                        QuadKey9 = c.String(maxLength: 9),
                        QuadKey10 = c.String(maxLength: 10),
                        QuadKey11 = c.String(maxLength: 11),
                        QuadKey12 = c.String(maxLength: 12),
                        QuadKey13 = c.String(maxLength: 13),
                        QuadKey14 = c.String(maxLength: 14),
                        QuadKey15 = c.String(maxLength: 15),
                        QuadKey16 = c.String(maxLength: 16),
                        QuadKey17 = c.String(maxLength: 17),
                        QuadKey18 = c.String(maxLength: 18),
                        QuadKey19 = c.String(maxLength: 19),
                        QuadKey20 = c.String(maxLength: 20),
                        QuadKey21 = c.String(maxLength: 21),
                        QuadKey22 = c.String(maxLength: 22),
                        QuadKey23 = c.String(maxLength: 23),
                        QuadKey24 = c.String(maxLength: 24),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.Counties",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Abbreviation = c.String(maxLength: 50),
                        CensusID = c.String(maxLength: 50),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                        NWLatitude = c.Single(),
                        NWLongitude = c.Single(),
                        SELatitude = c.Single(),
                        SELongitude = c.Single(),
                        ParentID = c.Guid(),
                        Centroid = c.Geography(),
                        Shape = c.Geography(),
                        ShapeReduced = c.Geography(),
                        BoundingBox = c.Geography(),
                        HousingCount = c.Int(nullable: false),
                        ResidentCount = c.Int(nullable: false),
                        Radius = c.Single(nullable: false),
                        QuadKey1 = c.String(maxLength: 1),
                        QuadKey2 = c.String(maxLength: 2),
                        QuadKey3 = c.String(maxLength: 3),
                        QuadKey4 = c.String(maxLength: 4),
                        QuadKey5 = c.String(maxLength: 5),
                        QuadKey6 = c.String(maxLength: 6),
                        QuadKey7 = c.String(maxLength: 7),
                        QuadKey8 = c.String(maxLength: 8),
                        QuadKey9 = c.String(maxLength: 9),
                        QuadKey10 = c.String(maxLength: 10),
                        QuadKey11 = c.String(maxLength: 11),
                        QuadKey12 = c.String(maxLength: 12),
                        QuadKey13 = c.String(maxLength: 13),
                        QuadKey14 = c.String(maxLength: 14),
                        QuadKey15 = c.String(maxLength: 15),
                        QuadKey16 = c.String(maxLength: 16),
                        QuadKey17 = c.String(maxLength: 17),
                        QuadKey18 = c.String(maxLength: 18),
                        QuadKey19 = c.String(maxLength: 19),
                        QuadKey20 = c.String(maxLength: 20),
                        QuadKey21 = c.String(maxLength: 21),
                        QuadKey22 = c.String(maxLength: 22),
                        QuadKey23 = c.String(maxLength: 23),
                        QuadKey24 = c.String(maxLength: 24),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.CensusTracts",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Abbreviation = c.String(maxLength: 50),
                        CensusID = c.String(maxLength: 50),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                        NWLatitude = c.Single(),
                        NWLongitude = c.Single(),
                        SELatitude = c.Single(),
                        SELongitude = c.Single(),
                        ParentID = c.Guid(),
                        Centroid = c.Geography(),
                        Shape = c.Geography(),
                        ShapeReduced = c.Geography(),
                        BoundingBox = c.Geography(),
                        HousingCount = c.Int(nullable: false),
                        ResidentCount = c.Int(nullable: false),
                        Radius = c.Single(nullable: false),
                        QuadKey1 = c.String(maxLength: 1),
                        QuadKey2 = c.String(maxLength: 2),
                        QuadKey3 = c.String(maxLength: 3),
                        QuadKey4 = c.String(maxLength: 4),
                        QuadKey5 = c.String(maxLength: 5),
                        QuadKey6 = c.String(maxLength: 6),
                        QuadKey7 = c.String(maxLength: 7),
                        QuadKey8 = c.String(maxLength: 8),
                        QuadKey9 = c.String(maxLength: 9),
                        QuadKey10 = c.String(maxLength: 10),
                        QuadKey11 = c.String(maxLength: 11),
                        QuadKey12 = c.String(maxLength: 12),
                        QuadKey13 = c.String(maxLength: 13),
                        QuadKey14 = c.String(maxLength: 14),
                        QuadKey15 = c.String(maxLength: 15),
                        QuadKey16 = c.String(maxLength: 16),
                        QuadKey17 = c.String(maxLength: 17),
                        QuadKey18 = c.String(maxLength: 18),
                        QuadKey19 = c.String(maxLength: 19),
                        QuadKey20 = c.String(maxLength: 20),
                        QuadKey21 = c.String(maxLength: 21),
                        QuadKey22 = c.String(maxLength: 22),
                        QuadKey23 = c.String(maxLength: 23),
                        QuadKey24 = c.String(maxLength: 24),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.BlockGroups",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Abbreviation = c.String(maxLength: 50),
                        CensusID = c.String(maxLength: 50),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                        NWLatitude = c.Single(),
                        NWLongitude = c.Single(),
                        SELatitude = c.Single(),
                        SELongitude = c.Single(),
                        ParentID = c.Guid(),
                        Centroid = c.Geography(),
                        Shape = c.Geography(),
                        ShapeReduced = c.Geography(),
                        BoundingBox = c.Geography(),
                        HousingCount = c.Int(nullable: false),
                        ResidentCount = c.Int(nullable: false),
                        Radius = c.Single(nullable: false),
                        QuadKey1 = c.String(maxLength: 1),
                        QuadKey2 = c.String(maxLength: 2),
                        QuadKey3 = c.String(maxLength: 3),
                        QuadKey4 = c.String(maxLength: 4),
                        QuadKey5 = c.String(maxLength: 5),
                        QuadKey6 = c.String(maxLength: 6),
                        QuadKey7 = c.String(maxLength: 7),
                        QuadKey8 = c.String(maxLength: 8),
                        QuadKey9 = c.String(maxLength: 9),
                        QuadKey10 = c.String(maxLength: 10),
                        QuadKey11 = c.String(maxLength: 11),
                        QuadKey12 = c.String(maxLength: 12),
                        QuadKey13 = c.String(maxLength: 13),
                        QuadKey14 = c.String(maxLength: 14),
                        QuadKey15 = c.String(maxLength: 15),
                        QuadKey16 = c.String(maxLength: 16),
                        QuadKey17 = c.String(maxLength: 17),
                        QuadKey18 = c.String(maxLength: 18),
                        QuadKey19 = c.String(maxLength: 19),
                        QuadKey20 = c.String(maxLength: 20),
                        QuadKey21 = c.String(maxLength: 21),
                        QuadKey22 = c.String(maxLength: 22),
                        QuadKey23 = c.String(maxLength: 23),
                        QuadKey24 = c.String(maxLength: 24),
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
                .PrimaryKey(t => t.Guid);
            
            CreateTable(
                "dbo.Blocks",
                c => new
                    {
                        Guid = c.Guid(nullable: false),
                        Abbreviation = c.String(maxLength: 50),
                        CensusID = c.String(maxLength: 50),
                        Latitude = c.Single(),
                        Longitude = c.Single(),
                        NWLatitude = c.Single(),
                        NWLongitude = c.Single(),
                        SELatitude = c.Single(),
                        SELongitude = c.Single(),
                        ParentID = c.Guid(),
                        Centroid = c.Geography(),
                        Shape = c.Geography(),
                        ShapeReduced = c.Geography(),
                        BoundingBox = c.Geography(),
                        HousingCount = c.Int(nullable: false),
                        ResidentCount = c.Int(nullable: false),
                        Radius = c.Single(nullable: false),
                        QuadKey1 = c.String(maxLength: 1),
                        QuadKey2 = c.String(maxLength: 2),
                        QuadKey3 = c.String(maxLength: 3),
                        QuadKey4 = c.String(maxLength: 4),
                        QuadKey5 = c.String(maxLength: 5),
                        QuadKey6 = c.String(maxLength: 6),
                        QuadKey7 = c.String(maxLength: 7),
                        QuadKey8 = c.String(maxLength: 8),
                        QuadKey9 = c.String(maxLength: 9),
                        QuadKey10 = c.String(maxLength: 10),
                        QuadKey11 = c.String(maxLength: 11),
                        QuadKey12 = c.String(maxLength: 12),
                        QuadKey13 = c.String(maxLength: 13),
                        QuadKey14 = c.String(maxLength: 14),
                        QuadKey15 = c.String(maxLength: 15),
                        QuadKey16 = c.String(maxLength: 16),
                        QuadKey17 = c.String(maxLength: 17),
                        QuadKey18 = c.String(maxLength: 18),
                        QuadKey19 = c.String(maxLength: 19),
                        QuadKey20 = c.String(maxLength: 20),
                        QuadKey21 = c.String(maxLength: 21),
                        QuadKey22 = c.String(maxLength: 22),
                        QuadKey23 = c.String(maxLength: 23),
                        QuadKey24 = c.String(maxLength: 24),
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
                .PrimaryKey(t => t.Guid);
            
            AddColumn("dbo.Fields", "FormatString", c => c.String(maxLength: 50));
            AddColumn("dbo.Fields", "GroupName", c => c.String(maxLength: 50));
            AddColumn("dbo.Fields", "Description", c => c.String(maxLength: 100));
            AddColumn("dbo.PropertyAttributes", "Description", c => c.String(maxLength: 200));
            AddColumn("dbo.Properties", "QuadKey24", c => c.String(maxLength: 24));
            AddColumn("dbo.Properties", "QuadKey23", c => c.String(maxLength: 23));
            AddColumn("dbo.Properties", "QuadKey22", c => c.String(maxLength: 22));
            AddColumn("dbo.Properties", "QuadKey21", c => c.String(maxLength: 21));
            AddColumn("dbo.Properties", "QuadKey20", c => c.String(maxLength: 20));
            AddColumn("dbo.Properties", "QuadKey19", c => c.String(maxLength: 19));
            AddColumn("dbo.Properties", "QuadKey18", c => c.String(maxLength: 18));
            AddColumn("dbo.Properties", "QuadKey17", c => c.String(maxLength: 17));
            AddColumn("dbo.Properties", "QuadKey16", c => c.String(maxLength: 16));
            AddColumn("dbo.Properties", "QuadKey15", c => c.String(maxLength: 15));
            AddColumn("dbo.Properties", "QuadKey14", c => c.String(maxLength: 14));
            AddColumn("dbo.Properties", "QuadKey13", c => c.String(maxLength: 13));
            AddColumn("dbo.Properties", "QuadKey12", c => c.String(maxLength: 12));
            AddColumn("dbo.Properties", "QuadKey11", c => c.String(maxLength: 11));
            AddColumn("dbo.Properties", "QuadKey10", c => c.String(maxLength: 10));
            AddColumn("dbo.Properties", "QuadKey9", c => c.String(maxLength: 9));
            AddColumn("dbo.Properties", "QuadKey8", c => c.String(maxLength: 8));
            AddColumn("dbo.Properties", "QuadKey7", c => c.String(maxLength: 7));
            AddColumn("dbo.Properties", "QuadKey6", c => c.String(maxLength: 6));
            AddColumn("dbo.Properties", "QuadKey5", c => c.String(maxLength: 5));
            AddColumn("dbo.Properties", "QuadKey4", c => c.String(maxLength: 4));
            AddColumn("dbo.Properties", "QuadKey3", c => c.String(maxLength: 3));
            AddColumn("dbo.Properties", "QuadKey2", c => c.String(maxLength: 2));
            AddColumn("dbo.Properties", "QuadKey1", c => c.String(maxLength: 1));
            AddColumn("dbo.Properties", "Radius", c => c.Single(nullable: false));
            AddColumn("dbo.Properties", "ResidentCount", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "HousingCount", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "BoundingBox", c => c.Geography());
            AddColumn("dbo.Properties", "ShapeReduced", c => c.Geography());
            AddColumn("dbo.Properties", "Shape", c => c.Geography());
            AddColumn("dbo.Properties", "Centroid", c => c.Geography());
            AddColumn("dbo.Properties", "ParentID", c => c.Guid());
            AddColumn("dbo.Properties", "SELongitude", c => c.Single());
            AddColumn("dbo.Properties", "SELatitude", c => c.Single());
            AddColumn("dbo.Properties", "NWLongitude", c => c.Single());
            AddColumn("dbo.Properties", "NWLatitude", c => c.Single());
            AddColumn("dbo.Properties", "CensusID", c => c.String(maxLength: 50));
            AddColumn("dbo.Properties", "Abbreviation", c => c.String(maxLength: 50));
            AddColumn("dbo.Properties", "PropertyType", c => c.Int(nullable: false));
            AddColumn("dbo.Properties", "AddressID", c => c.Guid());
            AlterColumn("dbo.Occupants", "LastNameSuffix", c => c.String());
            AlterColumn("dbo.Occupants", "MiddleInitial", c => c.String());
            CreateIndex("dbo.States", "ExternalID");
            CreateIndex("dbo.States", "Version");
            CreateIndex("dbo.States", "CreatedByID");
            CreateIndex("dbo.States", "DateCreated");
            CreateIndex("dbo.States", "TenantID");
            CreateIndex("dbo.States", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.Counties", "ExternalID");
            CreateIndex("dbo.Counties", "Version");
            CreateIndex("dbo.Counties", "CreatedByID");
            CreateIndex("dbo.Counties", "DateCreated");
            CreateIndex("dbo.Counties", "TenantID");
            CreateIndex("dbo.Counties", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.Counties", "ParentID");
            CreateIndex("dbo.CensusTracts", "ExternalID");
            CreateIndex("dbo.CensusTracts", "Version");
            CreateIndex("dbo.CensusTracts", "CreatedByID");
            CreateIndex("dbo.CensusTracts", "DateCreated");
            CreateIndex("dbo.CensusTracts", "TenantID");
            CreateIndex("dbo.CensusTracts", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.CensusTracts", "ParentID");
            CreateIndex("dbo.BlockGroups", "ExternalID");
            CreateIndex("dbo.BlockGroups", "Version");
            CreateIndex("dbo.BlockGroups", "CreatedByID");
            CreateIndex("dbo.BlockGroups", "DateCreated");
            CreateIndex("dbo.BlockGroups", "TenantID");
            CreateIndex("dbo.BlockGroups", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.BlockGroups", "ParentID");
            CreateIndex("dbo.Blocks", "ExternalID");
            CreateIndex("dbo.Blocks", "Version");
            CreateIndex("dbo.Blocks", "CreatedByID");
            CreateIndex("dbo.Blocks", "DateCreated");
            CreateIndex("dbo.Blocks", "TenantID");
            CreateIndex("dbo.Blocks", "Id", clustered: true, name: "CLUSTERED_INDEX_ON_LONG");
            CreateIndex("dbo.Blocks", "ParentID");
            CreateIndex("dbo.Properties", "ParentID");
            AddForeignKey("dbo.Properties", "ParentID", "dbo.Blocks", "Guid");
            AddForeignKey("dbo.Blocks", "ParentID", "dbo.BlockGroups", "Guid");
            AddForeignKey("dbo.BlockGroups", "ParentID", "dbo.CensusTracts", "Guid");
            AddForeignKey("dbo.CensusTracts", "ParentID", "dbo.Counties", "Guid");
            AddForeignKey("dbo.Counties", "ParentID", "dbo.States", "Guid");
        }
    }
}
