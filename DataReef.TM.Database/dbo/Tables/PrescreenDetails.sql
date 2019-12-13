CREATE TABLE [dbo].[PrescreenDetails] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [BatchID]            UNIQUEIDENTIFIER NOT NULL,
    [AddressID]          UNIQUEIDENTIFIER NOT NULL,
    [Reference]          NVARCHAR (150)   NULL,
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
    [CreditCategory]     NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_dbo.PrescreenDetails] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.PrescreenDetails_dbo.PrescreenBatches_BatchID] FOREIGN KEY ([BatchID]) REFERENCES [dbo].[PrescreenBatches] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[PrescreenDetails]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BatchID]
    ON [dbo].[PrescreenDetails]([BatchID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[PrescreenDetails]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[PrescreenDetails]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[PrescreenDetails]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[PrescreenDetails]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[PrescreenDetails]([ExternalID] ASC);

