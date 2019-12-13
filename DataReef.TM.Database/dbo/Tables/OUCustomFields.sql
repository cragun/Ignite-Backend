CREATE TABLE [dbo].[OUCustomFields] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
    [DisplayName]        NVARCHAR (50)    NOT NULL,
    [IsRequired]         BIT              NOT NULL,
    [HasValueList]       BIT              NOT NULL,
    [ValueListType]      INT              NOT NULL,
    [DataType]           INT              NOT NULL,
    [DataLength]         INT              NOT NULL,
    [FormatString]       NVARCHAR (50)    NULL,
    [ClassName]          NVARCHAR (100)   NULL,
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
    CONSTRAINT [PK_dbo.OUCustomFields] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUCustomFields_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUCustomFields]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OUCustomFields]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUCustomFields]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUCustomFields]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUCustomFields]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUCustomFields]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUCustomFields]([ExternalID] ASC);

