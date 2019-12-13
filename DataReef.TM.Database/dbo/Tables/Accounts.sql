CREATE TABLE [dbo].[Accounts] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [IsDisabled]         BIT              NOT NULL,
    [OwnerID]            UNIQUEIDENTIFIER NOT NULL,
    [RootOUID]           UNIQUEIDENTIFIER NULL,
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
    CONSTRAINT [PK_dbo.Accounts] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Accounts_dbo.OUs_RootOUID] FOREIGN KEY ([RootOUID]) REFERENCES [dbo].[OUs] ([Guid]),
    CONSTRAINT [FK_dbo.Accounts_dbo.People_OwnerID] FOREIGN KEY ([OwnerID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Accounts]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OwnerID]
    ON [dbo].[Accounts]([OwnerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RootOUID]
    ON [dbo].[Accounts]([RootOUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Accounts]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Accounts]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Accounts]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Accounts]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Accounts]([ExternalID] ASC);

