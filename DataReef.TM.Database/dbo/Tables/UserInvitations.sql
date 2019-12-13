CREATE TABLE [dbo].[UserInvitations] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [FromPersonID]       UNIQUEIDENTIFIER NOT NULL,
    [EmailAddress]       NVARCHAR (100)   NULL,
    [FirstName]          NVARCHAR (50)    NULL,
    [LastName]           NVARCHAR (50)    NULL,
    [InvitationCode]     NVARCHAR (100)   NULL,
    [Status]             INT              NOT NULL,
    [DateAccepted]       DATETIME         NULL,
    [ExpirationDate]     DATETIME         NOT NULL,
    [ToPersonID]         UNIQUEIDENTIFIER NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
    [RoleID]             UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_dbo.UserInvitations] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.UserInvitations_dbo.OURoles_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[OURoles] ([Guid]),
    CONSTRAINT [FK_dbo.UserInvitations_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid]),
    CONSTRAINT [FK_dbo.UserInvitations_dbo.People_FromPersonID] FOREIGN KEY ([FromPersonID]) REFERENCES [dbo].[People] ([Guid]),
    CONSTRAINT [FK_dbo.UserInvitations_dbo.People_ToPersonID] FOREIGN KEY ([ToPersonID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[UserInvitations]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FromPersonID]
    ON [dbo].[UserInvitations]([FromPersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ToPersonID]
    ON [dbo].[UserInvitations]([ToPersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[UserInvitations]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RoleID]
    ON [dbo].[UserInvitations]([RoleID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[UserInvitations]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[UserInvitations]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[UserInvitations]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[UserInvitations]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[UserInvitations]([ExternalID] ASC);

