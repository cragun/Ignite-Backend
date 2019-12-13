CREATE TABLE [dbo].[OUShapes] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
    [ShapeID]            UNIQUEIDENTIFIER NOT NULL,
    [ShapeTypeID]        NVARCHAR (50)    NULL,
    [WellKnownText]      NVARCHAR (MAX)   NULL,
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
    [CentroidLat]        REAL             DEFAULT ((0)) NOT NULL,
    [CentroidLon]        REAL             DEFAULT ((0)) NOT NULL,
    [Radius]             REAL             DEFAULT ((0)) NOT NULL,
    [ParentID]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.OUShapes] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUShapes_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUShapes]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OUShapes]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUShapes]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUShapes]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUShapes]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUShapes]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUShapes]([ExternalID] ASC);


GO

