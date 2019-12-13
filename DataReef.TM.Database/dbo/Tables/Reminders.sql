CREATE TABLE [dbo].[Reminders] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [PersonID]           UNIQUEIDENTIFIER NOT NULL,
    [PropertyID]         UNIQUEIDENTIFIER NULL,
    [ReminderDate]       DATETIME         NOT NULL,
    [Description]        NVARCHAR (1000)  NULL,
    [IncludeInCalendar]  BIT              NOT NULL,
    [EmailReminder]      BIT              NOT NULL,
    [ReminderMinutes]    INT              NOT NULL,
    [Reminded]           BIT              NOT NULL,
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
    CONSTRAINT [PK_dbo.Reminders] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Reminders_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid]),
    CONSTRAINT [FK_dbo.Reminders_dbo.Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Reminders]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[Reminders]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PropertyID]
    ON [dbo].[Reminders]([PropertyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Reminders]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Reminders]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Reminders]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Reminders]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Reminders]([ExternalID] ASC);

