CREATE TABLE [dbo].[NukeMeTerritoryShapeIssue1] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [TerritoryID]        UNIQUEIDENTIFIER NOT NULL,
    [ShapeID]            UNIQUEIDENTIFIER NOT NULL,
    [ShapeTypeID]        NVARCHAR (50)    NULL,
    [WellKnownText]      NVARCHAR (MAX)   NULL,
    [CentroidLat]        REAL             NOT NULL,
    [CentroidLon]        REAL             NOT NULL,
    [Radius]             REAL             NOT NULL,
    [ParentID]           UNIQUEIDENTIFIER NULL,
    [Id]                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100)   NULL,
    [Flags]              BIGINT           NULL,
    [TenantID]           INT              NOT NULL,
    [DateCreated]        DATETIME         NOT NULL,
    [DateLastModified]   DATETIME         NULL,
    [CreatedByName]      NVARCHAR (100)   NULL,
    [CreatedByID]        UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]     UNIQUEIDENTIFIER NULL,
    [LastModifiedByName] NVARCHAR (100)   NULL,
    [Version]            INT              NOT NULL,
    [IsDeleted]          BIT              NOT NULL,
    [ExternalID]         NVARCHAR (50)    NULL,
    [TagString]          NVARCHAR (1000)  NULL
);

