CREATE TABLE [dbo].[Attachments] (
    [Guid]                UNIQUEIDENTIFIER NOT NULL,
    [MimeType]            NVARCHAR (50)    NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    [SortOrder]           INT              NULL,
    [OwnerID]             UNIQUEIDENTIFIER NOT NULL,
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
    [Person_Guid]         UNIQUEIDENTIFIER NULL,
    [Identification_Guid] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.Attachments] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Attachments_dbo.Identifications_Identification_Guid] FOREIGN KEY ([Identification_Guid]) REFERENCES [dbo].[Identifications] ([Guid]),
    CONSTRAINT [FK_dbo.Attachments_dbo.People_Person_Guid] FOREIGN KEY ([Person_Guid]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Attachments]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Attachments]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Attachments]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Attachments]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Attachments]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Attachments]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Person_Guid]
    ON [dbo].[Attachments]([Person_Guid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Identification_Guid]
    ON [dbo].[Attachments]([Identification_Guid] ASC);

