CREATE TABLE [dbo].[OUSettingDefinitions] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NULL,
    [IsActive]           BIT              NOT NULL,
    [Description]        NVARCHAR (1000)  NULL,
    [DisplayName]        NVARCHAR (50)    NULL,
    [Name]               NVARCHAR (50)    NULL,
    [FormatString]       NVARCHAR (50)    NULL,
    [EditorType]         INT              NOT NULL,
    [ListData]           NVARCHAR (MAX)   NULL,
    [Group]              NVARCHAR (100)   NULL,
    [SortOrder]          INT              NOT NULL,
    [MaxLength]          INT              NOT NULL,
    [Id]                 BIGINT           IDENTITY (1, 1) NOT NULL,
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
    CONSTRAINT [PK_dbo.OUSettingDefinitions] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUSettingDefinitions_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUSettingDefinitions]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OUSettingDefinitions]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUSettingDefinitions]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUSettingDefinitions]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUSettingDefinitions]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUSettingDefinitions]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUSettingDefinitions]([ExternalID] ASC);

