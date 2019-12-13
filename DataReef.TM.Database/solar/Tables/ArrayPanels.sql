CREATE TABLE [solar].[ArrayPanels] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [SolarArrayID]       UNIQUEIDENTIFIER NOT NULL,
    [SolarPanelID]       UNIQUEIDENTIFIER NOT NULL,
    [InverterID]         UNIQUEIDENTIFIER NULL,
    [IsHidden]           BIT              NOT NULL,
    [Row]                INT              NOT NULL,
    [Column]             INT              NOT NULL,
    [WellKnownText]      NVARCHAR (MAX)   NULL,
    [X1]                 FLOAT (53)       NOT NULL,
    [X2]                 FLOAT (53)       NOT NULL,
    [Y1]                 FLOAT (53)       NOT NULL,
    [Y2]                 FLOAT (53)       NOT NULL,
    [CentroidX]          FLOAT (53)       NOT NULL,
    [CentroidY]          FLOAT (53)       NOT NULL,
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
    CONSTRAINT [PK_solar.ArrayPanels] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.ArrayPanels_solar.Arrays_SolarArrayID] FOREIGN KEY ([SolarArrayID]) REFERENCES [solar].[Arrays] ([Guid]),
    CONSTRAINT [FK_solar.ArrayPanels_solar.Inverters_InverterID] FOREIGN KEY ([InverterID]) REFERENCES [solar].[Inverters] ([Guid]),
    CONSTRAINT [FK_solar.ArrayPanels_solar.Panels_SolarPanelID] FOREIGN KEY ([SolarPanelID]) REFERENCES [solar].[Panels] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[ArrayPanels]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarArrayID]
    ON [solar].[ArrayPanels]([SolarArrayID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarPanelID]
    ON [solar].[ArrayPanels]([SolarPanelID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InverterID]
    ON [solar].[ArrayPanels]([InverterID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[ArrayPanels]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[ArrayPanels]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[ArrayPanels]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[ArrayPanels]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[ArrayPanels]([ExternalID] ASC);

