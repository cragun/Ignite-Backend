CREATE TABLE [dbo].[PrescreenInstants] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Status]             INT              NOT NULL,
    [CompletionDate]     DATETIME         NULL,
    [ErrorString]        NVARCHAR (MAX)   NULL,
    [PropertyID]         UNIQUEIDENTIFIER NOT NULL,
    [Reference]          NVARCHAR (150)   NULL,
    [CreditCategory]     NVARCHAR (MAX)   NULL,
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
    CONSTRAINT [PK_dbo.PrescreenInstants] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.PrescreenInstants_dbo.Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[PrescreenInstants]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PropertyID]
    ON [dbo].[PrescreenInstants]([PropertyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[PrescreenInstants]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[PrescreenInstants]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[PrescreenInstants]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[PrescreenInstants]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[PrescreenInstants]([ExternalID] ASC);

