CREATE TABLE [dbo].[OUMediaItems] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
    [MediaID]            UNIQUEIDENTIFIER NOT NULL,
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
    [ParentId]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.OUMediaItems] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUMediaItems_dbo.MediaItems_MediaID] FOREIGN KEY ([MediaID]) REFERENCES [dbo].[MediaItems] ([Guid]),
    CONSTRAINT [FK_dbo.OUMediaItems_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUMediaItems]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OUMediaItems]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MediaID]
    ON [dbo].[OUMediaItems]([MediaID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUMediaItems]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUMediaItems]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUMediaItems]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUMediaItems]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUMediaItems]([ExternalID] ASC);

