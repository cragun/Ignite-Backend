CREATE TABLE [solar].[Tariffs] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [TariffID]           NVARCHAR (MAX)   NULL,
    [TariffName]         NVARCHAR (MAX)   NULL,
    [PricePerKWH]        REAL             NOT NULL,
    [UtilityID]          NVARCHAR (MAX)   NULL,
    [UtilityName]        NVARCHAR (MAX)   NULL,
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
    [MasterTariffID]     NVARCHAR (MAX)   NULL,
    [TariffCode]         NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_solar.Tariffs] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.Tariffs_solar.Proposals_Guid] FOREIGN KEY ([Guid]) REFERENCES [solar].[Proposals] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[Tariffs]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Guid]
    ON [solar].[Tariffs]([Guid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[Tariffs]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[Tariffs]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[Tariffs]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[Tariffs]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[Tariffs]([ExternalID] ASC);

