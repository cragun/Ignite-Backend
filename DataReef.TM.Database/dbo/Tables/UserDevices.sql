CREATE TABLE [dbo].[UserDevices] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [UserID]             UNIQUEIDENTIFIER NOT NULL,
    [DeviceID]           UNIQUEIDENTIFIER NOT NULL,
    [IsDisabled]         BIT              NOT NULL,
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
    CONSTRAINT [PK_dbo.UserDevices] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.UserDevices_dbo.Devices_DeviceID] FOREIGN KEY ([DeviceID]) REFERENCES [dbo].[Devices] ([Guid]),
    CONSTRAINT [FK_dbo.UserDevices_dbo.Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [dbo].[Users] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[UserDevices]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_UserID]
    ON [dbo].[UserDevices]([UserID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DeviceID]
    ON [dbo].[UserDevices]([DeviceID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[UserDevices]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[UserDevices]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[UserDevices]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[UserDevices]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[UserDevices]([ExternalID] ASC);

