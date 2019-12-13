CREATE TABLE [dbo].[Fields] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [DisplayName]        NVARCHAR (100)   NULL,
    [Description]        NVARCHAR (100)   NULL,
    [GroupName]          NVARCHAR (50)    NULL,
    [Value]              NVARCHAR (MAX)   NULL,
    [FormatString]       NVARCHAR (50)    NULL,
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
    [OccupantId]         UNIQUEIDENTIFIER NULL,
    [PropertyId]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.Fields] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Fields_dbo.Occupants_Occupant_Guid] FOREIGN KEY ([OccupantId]) REFERENCES [dbo].[Occupants] ([Guid]),
    CONSTRAINT [FK_dbo.Fields_dbo.Properties_PropertyBase_Guid] FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Properties] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Fields]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Fields]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Fields]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Fields]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Fields]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Fields]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OccupantId]
    ON [dbo].[Fields]([OccupantId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PropertyId]
    ON [dbo].[Fields]([PropertyId] ASC);

