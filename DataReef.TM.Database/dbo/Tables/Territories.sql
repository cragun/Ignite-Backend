CREATE TABLE [dbo].[Territories] (
    [Guid]               UNIQUEIDENTIFIER  NOT NULL,
    [Status]             INT               NOT NULL,
    [DataStatus]         INT               NOT NULL,
    [Shape]              [sys].[geography] NULL,
    [WellKnownText]      NVARCHAR (MAX)    NULL,
    [OUID]               UNIQUEIDENTIFIER  NOT NULL,
    [ShapeID]            UNIQUEIDENTIFIER  NULL,
    [Id]                 BIGINT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100)    NULL,
    [Flags]              BIGINT            NULL,
    [TenantID]           INT               NOT NULL,
    [DateCreated]        DATETIME          NOT NULL,
    [DateLastModified]   DATETIME          NULL,
    [CreatedByName]      NVARCHAR (100)    NULL,
    [CreatedByID]        UNIQUEIDENTIFIER  NULL,
    [LastModifiedBy]     UNIQUEIDENTIFIER  NULL,
    [LastModifiedByName] NVARCHAR (100)    NULL,
    [Version]            INT               NOT NULL,
    [IsDeleted]          BIT               NOT NULL,
    [ExternalID]         NVARCHAR (50)     NULL,
    [TagString]          NVARCHAR (1000)   NULL,
    [PropertyCount]      INT               DEFAULT ((0)) NOT NULL,
    [CentroidLat]        REAL              DEFAULT ((0)) NOT NULL,
    [CentroidLon]        REAL              DEFAULT ((0)) NOT NULL,
    [Radius]             REAL              DEFAULT ((0)) NOT NULL,
    [ShapesVersion]      INT               NULL,
    CONSTRAINT [PK_dbo.Territories] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Territories_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);






GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Territories]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[Territories]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Territories]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Territories]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Territories]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Territories]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Territories]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Territories_8_469576711__K6_1_9]
    ON [dbo].[Territories]([OUID] ASC)
    INCLUDE([Guid], [Id]);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Territories_8_469576711__K1_K9_K6]
    ON [dbo].[Territories]([Guid] ASC, [Id] ASC, [OUID] ASC);


GO
CREATE trigger [dbo].[tgTerritoryUpdateLayerShapes]
on [dbo].[Territories] 
AFTER INSERT,UPDATE,DELETE 
AS

begin
 


declare @OUID UniqueIdentifier
select @OUID = OUID from Inserted

-- sunedison only
if (@OUID not in (select guid from dbo.outree('2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'))) return

delete from geospatial..layershapes where ID in (select l.ID from geospatial..layershapes l with (nolock) where l.layerid='shape-territories-se')

delete from geospatial..tiledirectory where layername='shape-territories-se'


insert into geospatial..layershapes (layerid,title,subtitle,geoid,color,reference,lat1,lat2,lon1,lon2,shape)
select  'shape-territories-se',TerritoryName,OUName,null as GeoID,'0000FF' as Color,TerritoryID as Reference,
geometry::STGeomFromWKB(Shape.STAsBinary(), Shape.STSrid).STEnvelope().STPointN(3).STY as Lat1,
geometry::STGeomFromWKB(Shape.STAsBinary(), Shape.STSrid).STEnvelope().STPointN(1).STY as Lat2,
geometry::STGeomFromWKB(Shape.STAsBinary(), Shape.STSrid).STEnvelope().STPointN(1).STX as Lon1,
geometry::STGeomFromWKB(Shape.STAsBinary(), Shape.STSrid).STEnvelope().STPointN(3).STX as Lon2,
Shape

 from
(
select TerritoryID,TerritoryName,OUName, OUID,case when Shape.EnvelopeAngle() >= 90 THEN Shape.ReorientObject() else Shape end as Shape
from
(
select t.Guid as TerritoryID,t.Name as TerritoryName,ou.Name as OUName,OUID, geography::STGeomFromText(t.WellKnownText, 4326).MakeValid() as Shape
from Territories t
inner join ous ou on t.ouid=ou.guid
where ouid in (select guid from dbo.outree('2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'))
and t.isdeleted=0
and ou.name not like 'demo%'

)
as data
) as data

 

 end


GO
CREATE trigger [dbo].[tg_Territories_UpdateCentroid]
on [dbo].[Territories]
AFTER INSERT
as
begin

declare @wkt varchar(max)
declare @Guid UniqueIdentifier

select @wkt = WellKnownText, @Guid = [GUID] from Inserted

IF(@wkt is not null)
BEGIN

	-- GEOMETRY is more accurate when determining the centroid
	DECLARE @geom GEOMETRY = GEOMETRY::STGeomFromText(@wkt, 4326)
	DECLARE @centerGeom GEOMETRY = @geom.STCentroid()
	DECLARE @envelope GEOMETRY = @geom.STEnvelope()
	DECLARE @TopLeft GEOMETRY = @envelope.STPointN(1)
	-- get the geometry point to calculate the radius
	DECLARE @centerGeo GEOGRAPHY = geography::STGeomFromText(@centerGeom.ToString(), 4326)
	DECLARE @CenterTOP GEOGRAPHY = 'POINT(' + CAST(@centerGeom.STX AS VARCHAR(MAX)) + ' ' + CAST(@TopLeft.STY AS VARCHAR(MAX)) +')'
	DECLARE @Radius FLOAT = @centerGeo.STDistance(@CenterTOP)

	UPDATE 
		Territories 
	SET 
		CentroidLon = @centerGeom.STX,
		CentroidLat = @centerGeom.STY,
		Radius = @Radius
	WHERE 
		[GUID] = @Guid

END

END

GO
