CREATE TABLE [dbo].[AccountAssociations] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [PersonID]           UNIQUEIDENTIFIER NOT NULL,
    [AccountID]          UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_dbo.AccountAssociations] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.AccountAssociations_dbo.Accounts_AccountID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts] ([Guid]),
    CONSTRAINT [FK_dbo.AccountAssociations_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[AccountAssociations]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[AccountAssociations]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountID]
    ON [dbo].[AccountAssociations]([AccountID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[AccountAssociations]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[AccountAssociations]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[AccountAssociations]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[AccountAssociations]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[AccountAssociations]([ExternalID] ASC);

