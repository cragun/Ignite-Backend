CREATE TABLE [dbo].[MediaItems] (
    [Guid]                 UNIQUEIDENTIFIER NOT NULL,
    [IsFolder]             BIT              NOT NULL,
    [ParentID]             UNIQUEIDENTIFIER NULL,
    [OUID]                 UNIQUEIDENTIFIER NOT NULL,
    [MimeType]             NVARCHAR (100)   NULL,
    [Url]                  NVARCHAR (255)   NULL,
    [Size]                 BIGINT           NOT NULL,
    [AuthenticationnToken] NVARCHAR (255)   NULL,
    [Id]                   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                 NVARCHAR (100)   NULL,
    [Flags]                BIGINT           NULL,
    [TenantID]             INT              NOT NULL,
    [DateCreated]          DATETIME         NOT NULL,
    [DateLastModified]     DATETIME         NULL,
    [CreatedByName]        NVARCHAR (100)   NULL,
    [CreatedByID]          UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]       UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]   NVARCHAR (100)   NULL,
    [Version]              INT              NOT NULL,
    [IsDeleted]            BIT              NOT NULL,
    [ExternalID]           NVARCHAR (50)    NULL,
    [TagString]            NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_dbo.MediaItems] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.MediaItems_dbo.MediaItems_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[MediaItems] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[MediaItems]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ParentID]
    ON [dbo].[MediaItems]([ParentID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[MediaItems]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[MediaItems]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[MediaItems]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[MediaItems]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[MediaItems]([ExternalID] ASC);

