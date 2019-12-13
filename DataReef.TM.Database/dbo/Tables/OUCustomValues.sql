CREATE TABLE [dbo].[OUCustomValues] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [CustomFieldID]      UNIQUEIDENTIFIER NOT NULL,
    [Value]              NVARCHAR (200)   NOT NULL,
    [OwnerID]            UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
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
    [IsDefault]          BIT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.OUCustomValues] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUCustomValues_dbo.OUCustomFields_CustomFieldID] FOREIGN KEY ([CustomFieldID]) REFERENCES [dbo].[OUCustomFields] ([Guid]),
    CONSTRAINT [FK_dbo.OUCustomValues_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUCustomValues]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CustomFieldID]
    ON [dbo].[OUCustomValues]([CustomFieldID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OUCustomValues]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUCustomValues]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUCustomValues]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUCustomValues]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUCustomValues]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUCustomValues]([ExternalID] ASC);

