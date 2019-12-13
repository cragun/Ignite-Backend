CREATE TABLE [dbo].[ExecutedAgreementParts] (
    [Guid]                UNIQUEIDENTIFIER NOT NULL,
    [ExecutedAgreementID] UNIQUEIDENTIFIER NOT NULL,
    [Uri]                 NVARCHAR (255)   NULL,
    [PersonID]            UNIQUEIDENTIFIER NOT NULL,
    [SignerID]            UNIQUEIDENTIFIER NULL,
    [Id]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (100)   NULL,
    [Flags]               BIGINT           NULL,
    [TenantID]            INT              NOT NULL,
    [DateCreated]         DATETIME         NOT NULL,
    [DateLastModified]    DATETIME         NULL,
    [CreatedByName]       NVARCHAR (100)   NULL,
    [CreatedByID]         UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]      UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]  NVARCHAR (100)   NULL,
    [Version]             INT              NOT NULL,
    [IsDeleted]           BIT              NOT NULL,
    [ExternalID]          NVARCHAR (50)    NULL,
    [TagString]           NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_dbo.ExecutedAgreementParts] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.ExecutedAgreementParts_dbo.ExecutedAgreements_ExecutedAgreementID] FOREIGN KEY ([ExecutedAgreementID]) REFERENCES [dbo].[ExecutedAgreements] ([Guid]),
    CONSTRAINT [FK_dbo.ExecutedAgreementParts_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid]),
    CONSTRAINT [FK_dbo.ExecutedAgreementParts_dbo.People_SignerID] FOREIGN KEY ([SignerID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[ExecutedAgreementParts]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExecutedAgreementID]
    ON [dbo].[ExecutedAgreementParts]([ExecutedAgreementID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[ExecutedAgreementParts]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SignerID]
    ON [dbo].[ExecutedAgreementParts]([SignerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[ExecutedAgreementParts]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[ExecutedAgreementParts]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[ExecutedAgreementParts]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[ExecutedAgreementParts]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[ExecutedAgreementParts]([ExternalID] ASC);

