namespace DataReef.TM.DataAccess.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RoofPlaneChanges : DbMigration
    {
        public override void Up()
        {
            CreateIndex("solar.RoofPlanes", "InverterID");
            CreateIndex("solar.RoofPlanes", "SolarPanelID");
            AddForeignKey("solar.RoofPlanes", "InverterID", "solar.Inverters", "Guid");
            AddForeignKey("solar.RoofPlanes", "SolarPanelID", "solar.Panels", "Guid");

            Sql(@"
INSERT INTO [solar].[RoofPlanes]
           ([Guid]
           ,[Azimuth]
           ,[CenterX]
           ,[CenterY]
           ,[CenterLatitude]
           ,[CenterLongitude]
           ,[GenabilitySolarProviderProfileID]
           ,[IsManuallyEntered]
           ,[ManuallyEnteredPanelsCount]
           ,[ModuleSpacing]
           ,[Pitch]
           ,[Racking]
           ,[RowSpacing]
           ,[Shading]
           ,[InverterID]
           ,[SolarPanelID]
           ,[Tilt]
           ,[Name]
           ,[Flags]
           ,[TenantID]
           ,[DateCreated]
           ,[DateLastModified]
           ,[CreatedByName]
           ,[CreatedByID]
           ,[LastModifiedBy]
           ,[LastModifiedByName]
           ,[Version]
           ,[IsDeleted]
           ,[ExternalID]
           ,[TagString]
           ,[SolarSystemID])
SELECT	a.Guid
           ,a.Azimuth
           ,a.CenterX
           ,a.CenterY
           ,a.CenterLatitude
           ,a.CenterLongitude
           ,a.GenabilitySolarProviderProfileID
           ,a.IsManuallyEntered
           ,a.ManuallyEnteredPanelsCount
           ,a.ModuleSpacing
           ,tan(a.Tilt * PI() / 180.0)  as Pitch
           ,a.Racking
           ,a.RowSpacing
           ,a.Shading
           ,a.InverterID
           ,a.SolarPanelID
           ,a.Tilt
           ,a.Name
           ,a.Flags
           ,a.TenantID
           ,a.DateCreated
           ,a.DateLastModified
           ,a.CreatedByName
           ,a.CreatedByID
           ,a.LastModifiedBy
           ,a.LastModifiedByName
           ,a.[Version]
           ,a.IsDeleted
           ,a.ExternalID
           ,a.TagString
           ,a.SolarSystemID
From solar.arrays a
where ismanuallyentered = 1

INSERT INTO [solar].[RoofPlanes]
           ([Guid]
           ,[Azimuth]
           ,[CenterX]
           ,[CenterY]
           ,[CenterLatitude]
           ,[CenterLongitude]
           ,[GenabilitySolarProviderProfileID]
           ,[IsManuallyEntered]
           ,[ManuallyEnteredPanelsCount]
           ,[ModuleSpacing]
           ,[Pitch]
           ,[Racking]
           ,[RowSpacing]
           ,[Shading]
           ,[InverterID]
           ,[SolarPanelID]
           ,[Tilt]
           ,[Name]
           ,[Flags]
           ,[TenantID]
           ,[DateCreated]
           ,[DateLastModified]
           ,[CreatedByName]
           ,[CreatedByID]
           ,[LastModifiedBy]
           ,[LastModifiedByName]
           ,[Version]
           ,[IsDeleted]
           ,[ExternalID]
           ,[TagString]
           ,[SolarSystemID])
SELECT	a.Guid
           ,a.Azimuth
           ,a.CenterX
           ,a.CenterY
           ,a.CenterLatitude
           ,a.CenterLongitude
           ,a.GenabilitySolarProviderProfileID
           ,1 as IsManuallyEntered
           ,(SELECT COUNT(*) FROM solar.ArrayPanels p where p.SolarArrayID = a.Guid AND p.IsHidden = 0) as ManuallyEnteredPanelsCount
           ,a.ModuleSpacing
           ,tan(a.Tilt * PI() / 180.0)  as Pitch
           ,a.Racking
           ,a.RowSpacing
           ,a.Shading
           ,a.InverterID
           ,a.SolarPanelID
           ,a.Tilt
           ,a.Name
           ,a.Flags
           ,a.TenantID
           ,a.DateCreated
           ,a.DateLastModified
           ,a.CreatedByName
           ,a.CreatedByID
           ,a.LastModifiedBy
           ,a.LastModifiedByName
           ,a.[Version]
           ,a.IsDeleted
           ,a.ExternalID
           ,a.TagString
           ,a.SolarSystemID
From solar.arrays a
where ismanuallyentered = 0
            ");
        }
        
        public override void Down()
        {
            DropForeignKey("solar.RoofPlanes", "SolarPanelID", "solar.Panels");
            DropForeignKey("solar.RoofPlanes", "InverterID", "solar.Inverters");
            DropIndex("solar.RoofPlanes", new[] { "SolarPanelID" });
            DropIndex("solar.RoofPlanes", new[] { "InverterID" });
        }
    }
}
