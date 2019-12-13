CREATE TABLE [dbo].[OULayers] (
    [Guid]                UNIQUEIDENTIFIER NOT NULL,
    [OUID]                UNIQUEIDENTIFIER NOT NULL,
    [LayerID]             UNIQUEIDENTIFIER NOT NULL,
    [Color]               NVARCHAR (10)    NULL,
    [IsFavorite]          BIT              NOT NULL,
    [IsVisibleDownStream] BIT              NOT NULL,
    [IsVisibleUpStream]   BIT              NOT NULL,
    [Id]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (100)   NULL,
    [Flags]               BIGINT           NULL,
    [TenantID]            INT              NOT NULL,
    [DateCreated]         DATETIME         NOT NULL,
    [DateLastModified]    DATETIME         NULL,
    [CreatedByName]       NVARCHAR (100)   NULL,
    [CreatedByID]         UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]      UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]  NVARCHAR (100)   NULL,
    [Version]             INT              NOT NULL,
    [IsDeleted]           BIT              NOT NULL,
    [ExternalID]          NVARCHAR (50)    NULL,
    [TagString]           NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_dbo.OULayers] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OULayers_dbo.Layers_LayerID] FOREIGN KEY ([LayerID]) REFERENCES [dbo].[Layers] ([Guid]),
    CONSTRAINT [FK_dbo.OULayers_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OULayers]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OULayers]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LayerID]
    ON [dbo].[OULayers]([LayerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OULayers]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OULayers]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OULayers]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OULayers]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OULayers]([ExternalID] ASC);

