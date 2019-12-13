CREATE TABLE [dbo].[Zip5Shape] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [ShapeID]            UNIQUEIDENTIFIER NOT NULL,
    [WellKnownText]      NVARCHAR (MAX)   NULL,
    [CentroidLat]        REAL             NOT NULL,
    [CentroidLon]        REAL             NOT NULL,
    [Radius]             REAL             NOT NULL,
    [ResidentCount]      INT              NOT NULL,
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
    CONSTRAINT [PK_dbo.Zip5Shape] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Zip5Shape_dbo.ZipAreas_Guid] FOREIGN KEY ([Guid]) REFERENCES [dbo].[ZipAreas] ([Guid])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Zip5Shape]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Zip5Shape]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Zip5Shape]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Zip5Shape]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Zip5Shape]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Guid]
    ON [dbo].[Zip5Shape]([Guid] ASC);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Zip5Shape]([Id] ASC);

