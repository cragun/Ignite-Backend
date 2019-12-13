CREATE TABLE [dbo].[TerritoryShapes] (
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
    [TagString]          NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_dbo.TerritoryShapes] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.TerritoryShapes_dbo.Territories_TerritoryID] FOREIGN KEY ([TerritoryID]) REFERENCES [dbo].[Territories] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[TerritoryShapes]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TerritoryID]
    ON [dbo].[TerritoryShapes]([TerritoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[TerritoryShapes]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[TerritoryShapes]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[TerritoryShapes]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[TerritoryShapes]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[TerritoryShapes]([ExternalID] ASC);

