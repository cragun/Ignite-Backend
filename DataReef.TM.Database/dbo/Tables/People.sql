CREATE TABLE [dbo].[People] (
    [Guid]                    UNIQUEIDENTIFIER NOT NULL,
    [SocialSecurityEntrypted] NVARCHAR (MAX)   NULL,
    [Prefix]                  NVARCHAR (15)    NULL,
    [FirstName]               NVARCHAR (100)   NULL,
    [MiddleName]              NVARCHAR (50)    NULL,
    [LastName]                NVARCHAR (100)   NULL,
    [PreferredName]           NVARCHAR (100)   NULL,
    [Suffix]                  NVARCHAR (10)    NULL,
    [EmailAddressString]      NVARCHAR (450)   NULL,
    [Id]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (100)   NULL,
    [Flags]                   BIGINT           NULL,
    [TenantID]                INT              NOT NULL,
    [DateCreated]             DATETIME         NOT NULL,
    [DateLastModified]        DATETIME         NULL,
    [CreatedByName]           NVARCHAR (100)   NULL,
    [CreatedByID]             UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]          UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]      NVARCHAR (100)   NULL,
    [Version]                 INT              NOT NULL,
    [IsDeleted]               BIT              NOT NULL,
    [ExternalID]              NVARCHAR (50)    NULL,
    [TagString]               NVARCHAR (1000)  NULL,
    [SalesRepID]              NVARCHAR (50)    NULL,
    CONSTRAINT [PK_dbo.People] PRIMARY KEY NONCLUSTERED ([Guid] ASC)
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[People]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[People]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[People]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[People]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[People]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[People]([ExternalID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_EmailAddressString]
    ON [dbo].[People]([EmailAddressString] ASC);

