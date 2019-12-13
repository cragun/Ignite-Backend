CREATE TABLE [dbo].[TokenTransfers] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Amount]             INT              NOT NULL,
    [Notes]              NVARCHAR (200)   NULL,
    [FromLedgerID]       UNIQUEIDENTIFIER NOT NULL,
    [ToLedgerID]         UNIQUEIDENTIFIER NOT NULL,
    [IsVoid]             BIT              NOT NULL,
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
    CONSTRAINT [PK_dbo.TokenTransfers] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.TokenTransfers_dbo.TokenLedgers_FromLedgerID] FOREIGN KEY ([FromLedgerID]) REFERENCES [dbo].[TokenLedgers] ([Guid]),
    CONSTRAINT [FK_dbo.TokenTransfers_dbo.TokenLedgers_ToLedgerID] FOREIGN KEY ([ToLedgerID]) REFERENCES [dbo].[TokenLedgers] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[TokenTransfers]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FromLedgerID]
    ON [dbo].[TokenTransfers]([FromLedgerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ToLedgerID]
    ON [dbo].[TokenTransfers]([ToLedgerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[TokenTransfers]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[TokenTransfers]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[TokenTransfers]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[TokenTransfers]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[TokenTransfers]([ExternalID] ASC);

