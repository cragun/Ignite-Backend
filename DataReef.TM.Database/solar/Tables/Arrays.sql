CREATE TABLE [solar].[Arrays] (
    [Guid]                       UNIQUEIDENTIFIER NOT NULL,
    [SolarPanelID]               UNIQUEIDENTIFIER NULL,
    [PanelOrientation]           INT              NOT NULL,
    [Azimuth]                    INT              NOT NULL,
    [Tilt]                       INT              NOT NULL,
    [Racking]                    INT              NOT NULL,
    [ModuleSpacing]              FLOAT (53)       NOT NULL,
    [RowSpacing]                 FLOAT (53)       NOT NULL,
    [Id]                         BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                       NVARCHAR (100)   NULL,
    [Flags]                      BIGINT           NULL,
    [TenantID]                   INT              NOT NULL,
    [DateCreated]                DATETIME         NOT NULL,
    [DateLastModified]           DATETIME         NULL,
    [CreatedByName]              NVARCHAR (100)   NULL,
    [CreatedByID]                UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]             UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]         NVARCHAR (100)   NULL,
    [Version]                    INT              NOT NULL,
    [IsDeleted]                  BIT              NOT NULL,
    [ExternalID]                 NVARCHAR (50)    NULL,
    [TagString]                  NVARCHAR (1000)  NULL,
    [SolarSystemID]              UNIQUEIDENTIFIER NOT NULL,
    [SolarArrayRotation]         FLOAT (53)       NOT NULL,
    [AnchorPointX]               FLOAT (53)       NOT NULL,
    [AnchorPointY]               FLOAT (53)       NOT NULL,
    [CenterX]                    FLOAT (53)       NOT NULL,
    [CenterY]                    FLOAT (53)       NOT NULL,
    [IsManuallyEntered]          BIT              NOT NULL,
    [Shading]                    SMALLINT         NOT NULL,
    [PanXOffset]                 FLOAT (53)       NOT NULL,
    [PanYOffset]                 FLOAT (53)       NOT NULL,
    [BoundingRect]               NVARCHAR (MAX)   NULL,
    [Bounds]                     NVARCHAR (MAX)   NULL,
    [ManuallyEnteredPanelsCount] INT              NOT NULL,
    [InverterID]                 UNIQUEIDENTIFIER NULL,
    [RidgeLineAzimuth]           INT              NULL,
    CONSTRAINT [PK_solar.Arrays] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.Arrays_solar.Inverters_InverterID] FOREIGN KEY ([InverterID]) REFERENCES [solar].[Inverters] ([Guid]),
    CONSTRAINT [FK_solar.Arrays_solar.Panels_SolarPanelID] FOREIGN KEY ([SolarPanelID]) REFERENCES [solar].[Panels] ([Guid]),
    CONSTRAINT [FK_solar.Arrays_solar.Systems_SolarSystem_Guid] FOREIGN KEY ([SolarSystemID]) REFERENCES [solar].[Systems] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[Arrays]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarPanelID]
    ON [solar].[Arrays]([SolarPanelID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[Arrays]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[Arrays]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[Arrays]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[Arrays]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[Arrays]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarSystemID]
    ON [solar].[Arrays]([SolarSystemID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_InverterID]
    ON [solar].[Arrays]([InverterID] ASC);

