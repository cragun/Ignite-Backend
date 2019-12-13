CREATE TABLE [dbo].[LoginAttempts] (
    [Guid]                   UNIQUEIDENTIFIER NOT NULL,
    [Result]                 INT              NOT NULL,
    [UserName]               NVARCHAR (50)    NULL,
    [IPAddress]              NVARCHAR (50)    NULL,
    [DeviceID]               NVARCHAR (50)    NULL,
    [OSVersion]              NVARCHAR (50)    NULL,
    [OSName]                 NVARCHAR (50)    NULL,
    [DeviceName]             NVARCHAR (50)    NULL,
    [ApplicationName]        NVARCHAR (50)    NULL,
    [FailureReason]          NVARCHAR (50)    NULL,
    [PasswordChangeRequired] BIT              NOT NULL,
    [PersonID]               UNIQUEIDENTIFIER NULL,
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
    CONSTRAINT [PK_dbo.LoginAttempts] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.LoginAttempts_dbo.Users_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Users] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[LoginAttempts]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[LoginAttempts]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[LoginAttempts]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[LoginAttempts]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[LoginAttempts]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[LoginAttempts]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[LoginAttempts]([ExternalID] ASC);

