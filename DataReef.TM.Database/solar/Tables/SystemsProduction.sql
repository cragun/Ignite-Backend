CREATE TABLE [solar].[SystemsProduction] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Consumption]        REAL             NOT NULL,
    [PreSolarCost]       REAL             NOT NULL,
    [PostSolarCost]      REAL             NOT NULL,
    [Production]         REAL             NOT NULL,
    [TarrifID]           NVARCHAR (MAX)   NULL,
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
    CONSTRAINT [PK_solar.SystemsProduction] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.SystemsProduction_solar.Systems_Guid] FOREIGN KEY ([Guid]) REFERENCES [solar].[Systems] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[SystemsProduction]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Guid]
    ON [solar].[SystemsProduction]([Guid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[SystemsProduction]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[SystemsProduction]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[SystemsProduction]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[SystemsProduction]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[SystemsProduction]([ExternalID] ASC);

