namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ActivityTypeChanges : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.OUCapabilities", newName: "OUActivityTypes");
            RenameTable(name: "dbo.Capabilities", newName: "ActivityTypes");
            DropIndex("dbo.OUActivityTypes", new[] { "CapabilityID" });
            RenameColumn(table: "dbo.OUActivityTypes", name: "CapabilityID", newName: "ActivityTypeID");
            AlterColumn("dbo.OUActivityTypes", "ActivityTypeID", c => c.Guid(nullable: false));

            // Populate ActivityTypes
            Sql("INSERT INTO ActivityTypes(Guid, Name, TenantID, DateCreated, Version, IsDeleted) VALUES ('00000000-0000-0000-0000-000000000000', 'None', 0, getutcdate(), 0, 0)");
            Sql("INSERT INTO ActivityTypes(Guid, Name, TenantID, DateCreated, Version, IsDeleted) VALUES ('00000000-1000-0000-0000-000000000000', 'SolarPanels', 0, getutcdate(), 0, 0)");
            Sql("INSERT INTO ActivityTypes(Guid, Name, TenantID, DateCreated, Version, IsDeleted) VALUES ('00000000-2000-0000-0000-000000000000', 'TV', 0, getutcdate(), 0, 0)");
            Sql("INSERT INTO ActivityTypes(Guid, Name, TenantID, DateCreated, Version, IsDeleted) VALUES ('00000000-3000-0000-0000-000000000000', 'Survey', 0, getutcdate(), 0, 0)");
            Sql("INSERT INTO ActivityTypes(Guid, Name, TenantID, DateCreated, Version, IsDeleted) VALUES ('00000000-4000-0000-0000-000000000000', 'HomeSystems', 0, getutcdate(), 0, 0)");

            AddColumn("dbo.Inquiries", "ActivityTypeID", c => c.Guid(nullable: false));
            // SunEdison
            // Sale 100 becomes Pending 90, SaleClosed 110 becomes Completed 100 (only SunEdison had the SaleClosed status)
            Sql("UPDATE Inquiries SET Status = 90 WHERE Status = 100 AND OUID IN (SELECT Guid from dbo.OUTree('2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'))");
            Sql("UPDATE Inquiries SET Status = 100 WHERE Status = 110 AND OUID IN (SELECT Guid from dbo.OUTree('2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'))");
            Sql("UPDATE Inquiries SET ActivityTypeID = '00000000-1000-0000-0000-000000000000' WHERE OUID IN (SELECT Guid from dbo.OUTree('2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'))");
            //ClearSattelite
            Sql("UPDATE Inquiries SET ActivityTypeID = '00000000-2000-0000-0000-000000000000' WHERE OUID IN (SELECT Guid from dbo.OUTree('F3A25F2C-AD03-4C68-B37A-7C105D861B14'))");
            //Smart72
            Sql("UPDATE Inquiries SET ActivityTypeID = '00000000-3000-0000-0000-000000000000' WHERE OUID = '18B48634-CA2F-42F5-94B4-DC3A44D7FF3E'");

            //All
            //AppointmentSet 10 is the same As CallBack 2
            Sql("UPDATE Inquiries SET Status = 2 WHERE Status = 10");
            //Inquiries with Survey 7 status will have the Survey activity type and Completed 100 status
            Sql("UPDATE Inquiries SET  ActivityTypeID = '00000000-3000-0000-0000-000000000000', Status = 100 WHERE Status = 7");

            CreateIndex("dbo.Inquiries", "ActivityTypeID");
            CreateIndex("dbo.OUActivityTypes", "ActivityTypeID");
            AddForeignKey("dbo.Inquiries", "ActivityTypeID", "dbo.ActivityTypes", "Guid");
            DropColumn("dbo.OUActivityTypes", "StartDate");
            DropColumn("dbo.OUActivityTypes", "EndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OUActivityTypes", "EndDate", c => c.DateTime());
            AddColumn("dbo.OUActivityTypes", "StartDate", c => c.DateTime());
            DropForeignKey("dbo.Inquiries", "ActivityTypeID", "dbo.ActivityTypes");
            DropIndex("dbo.OUActivityTypes", new[] { "ActivityTypeID" });
            DropIndex("dbo.Inquiries", new[] { "ActivityTypeID" });
            AlterColumn("dbo.OUActivityTypes", "ActivityTypeID", c => c.Guid());
            DropColumn("dbo.Inquiries", "ActivityTypeID");
            RenameColumn(table: "dbo.OUActivityTypes", name: "ActivityTypeID", newName: "CapabilityID");
            CreateIndex("dbo.OUActivityTypes", "CapabilityID");
            RenameTable(name: "dbo.ActivityTypes", newName: "Capabilities");
            RenameTable(name: "dbo.OUActivityTypes", newName: "OUCapabilities");
        }
    }
}
