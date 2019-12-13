CREATE TABLE [dbo].[ExecutedAgreements] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [AgreementID]        UNIQUEIDENTIFIER NOT NULL,
    [AccountID]          UNIQUEIDENTIFIER NOT NULL,
    [PersonID]           UNIQUEIDENTIFIER NOT NULL,
    [SignerID]           UNIQUEIDENTIFIER NULL,
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
    CONSTRAINT [PK_dbo.ExecutedAgreements] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.ExecutedAgreements_dbo.Agreements_AgreementID] FOREIGN KEY ([AgreementID]) REFERENCES [dbo].[Agreements] ([Guid]),
    CONSTRAINT [FK_dbo.ExecutedAgreements_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid]),
    CONSTRAINT [FK_dbo.ExecutedAgreements_dbo.People_SignerID] FOREIGN KEY ([SignerID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[ExecutedAgreements]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgreementID]
    ON [dbo].[ExecutedAgreements]([AgreementID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[ExecutedAgreements]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SignerID]
    ON [dbo].[ExecutedAgreements]([SignerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[ExecutedAgreements]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[ExecutedAgreements]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[ExecutedAgreements]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[ExecutedAgreements]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[ExecutedAgreements]([ExternalID] ASC);

