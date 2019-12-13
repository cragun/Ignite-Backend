CREATE TABLE [dbo].[Credentials] (
    [Guid]                   UNIQUEIDENTIFIER NOT NULL,
    [UserID]                 UNIQUEIDENTIFIER NOT NULL,
    [PersonID]               UNIQUEIDENTIFIER NOT NULL,
    [UserName]               NVARCHAR (50)    NULL,
    [PasswordHashed]         NVARCHAR (100)   NULL,
    [Salt]                   NVARCHAR (MAX)   NULL,
    [ExpirationDate]         DATETIME         NULL,
    [RequiresPasswordChange] BIT              NOT NULL,
    [Id]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                   NVARCHAR (100)   NULL,
    [Flags]                  BIGINT           NULL,
    [TenantID]               INT              NOT NULL,
    [DateCreated]            DATETIME         NOT NULL,
    [DateLastModified]       DATETIME         NULL,
    [CreatedByName]          NVARCHAR (100)   NULL,
    [CreatedByID]            UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]         UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]     NVARCHAR (100)   NULL,
    [Version]                INT              NOT NULL,
    [IsDeleted]              BIT              NOT NULL,
    [ExternalID]             NVARCHAR (50)    NULL,
    [TagString]              NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_dbo.Credentials] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Credentials_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Credentials]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserID]
    ON [dbo].[Credentials]([UserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Credentials]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Credentials]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Credentials]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Credentials]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Credentials]([ExternalID] ASC);

