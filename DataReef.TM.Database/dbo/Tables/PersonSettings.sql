CREATE TABLE [dbo].[PersonSettings] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [PersonID]           UNIQUEIDENTIFIER NOT NULL,
    [Value]              NVARCHAR (MAX)   NULL,
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
    CONSTRAINT [PK_dbo.PersonSettings] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.PersonSettings_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[PersonSettings]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[PersonSettings]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[PersonSettings]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[PersonSettings]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[PersonSettings]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[PersonSettings]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[PersonSettings]([ExternalID] ASC);

