CREATE TABLE [solar].[PowerConsumption] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Year]               INT              NOT NULL,
    [Month]              INT              NOT NULL,
    [Watts]              DECIMAL (18, 2)  NOT NULL,
    [Cost]               DECIMAL (18, 2)  NOT NULL,
    [SolarSystemID]      UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_solar.PowerConsumption] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.PowerConsumption_solar.Systems_SolarSystemID] FOREIGN KEY ([SolarSystemID]) REFERENCES [solar].[Systems] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[PowerConsumption]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarSystemID]
    ON [solar].[PowerConsumption]([SolarSystemID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[PowerConsumption]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[PowerConsumption]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[PowerConsumption]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[PowerConsumption]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[PowerConsumption]([ExternalID] ASC);

