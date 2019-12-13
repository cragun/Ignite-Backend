CREATE TABLE [solar].[FinancePlans] (
    [Guid]                    UNIQUEIDENTIFIER NOT NULL,
    [Id]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (100)   NULL,
    [Flags]                   BIGINT           NULL,
    [TenantID]                INT              NOT NULL,
    [DateCreated]             DATETIME         NOT NULL,
    [DateLastModified]        DATETIME         NULL,
    [CreatedByName]           NVARCHAR (100)   NULL,
    [CreatedByID]             UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]          UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]      NVARCHAR (100)   NULL,
    [Version]                 INT              NOT NULL,
    [IsDeleted]               BIT              NOT NULL,
    [ExternalID]              NVARCHAR (50)    NULL,
    [TagString]               NVARCHAR (1000)  NULL,
    [SolarSystemID]           UNIQUEIDENTIFIER NOT NULL,
    [FinancePlanType]         INT              DEFAULT ((0)) NOT NULL,
    [LoanRequestJSON]      NVARCHAR (MAX)   NULL,
    [LoanResponseJSON]     NVARCHAR (MAX)   NULL,
    [PPARequestJSON]          NVARCHAR (MAX)   NULL,
    [PPAResponseJSON]         NVARCHAR (MAX)   NULL,
    [SunEdisonPricingQuoteID] NVARCHAR (MAX)   NULL,
    [EnvelopeID]              NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_solar.FinancePlans] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.FinancePlans_solar.Systems_SolarSystem_Guid] FOREIGN KEY ([SolarSystemID]) REFERENCES [solar].[Systems] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[FinancePlans]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[FinancePlans]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[FinancePlans]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[FinancePlans]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[FinancePlans]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[FinancePlans]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarSystemID]
    ON [solar].[FinancePlans]([SolarSystemID] ASC);

