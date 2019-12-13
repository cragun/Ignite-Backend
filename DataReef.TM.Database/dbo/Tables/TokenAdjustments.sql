CREATE TABLE [dbo].[TokenAdjustments] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [LedgerID]           UNIQUEIDENTIFIER NOT NULL,
    [Amount]             INT              NOT NULL,
    [Notes]              NVARCHAR (200)   NULL,
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
    CONSTRAINT [PK_dbo.TokenAdjustments] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.TokenAdjustments_dbo.TokenLedgers_LedgerID] FOREIGN KEY ([LedgerID]) REFERENCES [dbo].[TokenLedgers] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[TokenAdjustments]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_LedgerID]
    ON [dbo].[TokenAdjustments]([LedgerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[TokenAdjustments]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[TokenAdjustments]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[TokenAdjustments]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[TokenAdjustments]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[TokenAdjustments]([ExternalID] ASC);

