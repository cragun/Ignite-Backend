CREATE TABLE [dbo].[AgreementParts] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Description]        NVARCHAR (MAX)   NULL,
    [AgreementID]        UNIQUEIDENTIFIER NOT NULL,
    [LocationString]     NVARCHAR (50)    NULL,
    [SizeString]         NVARCHAR (50)    NULL,
    [PageNumber]         INT              NOT NULL,
    [IsRequired]         BIT              NOT NULL,
    [OptionGroupID]      NVARCHAR (50)    NULL,
    [Type]               INT              NOT NULL,
    [SortOrder]          INT              NOT NULL,
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
    CONSTRAINT [PK_dbo.AgreementParts] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.AgreementParts_dbo.Agreements_AgreementID] FOREIGN KEY ([AgreementID]) REFERENCES [dbo].[Agreements] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[AgreementParts]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgreementID]
    ON [dbo].[AgreementParts]([AgreementID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[AgreementParts]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[AgreementParts]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[AgreementParts]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[AgreementParts]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[AgreementParts]([ExternalID] ASC);

