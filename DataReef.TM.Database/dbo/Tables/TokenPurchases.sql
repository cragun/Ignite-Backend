CREATE TABLE [dbo].[TokenPurchases] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Amount]             INT              NOT NULL,
    [Price]              DECIMAL (18, 2)  NOT NULL,
    [Reference]          NVARCHAR (100)   NULL,
    [LedgerID]           UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_dbo.TokenPurchases] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.TokenPurchases_dbo.TokenLedgers_LedgerID] FOREIGN KEY ([LedgerID]) REFERENCES [dbo].[TokenLedgers] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[TokenPurchases]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LedgerID]
    ON [dbo].[TokenPurchases]([LedgerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[TokenPurchases]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[TokenPurchases]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[TokenPurchases]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[TokenPurchases]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[TokenPurchases]([ExternalID] ASC);

