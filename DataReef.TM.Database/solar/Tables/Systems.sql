CREATE TABLE [solar].[Systems] (
    [Guid]                               UNIQUEIDENTIFIER NOT NULL,
    [DefaultInverterID]                  UNIQUEIDENTIFIER NOT NULL,
    [DefaultSolarPanelID]                UNIQUEIDENTIFIER NOT NULL,
    [SystemSize]                         INT              NOT NULL,
    [PanelCount]                         INT              NOT NULL,
    [AnnualOutputDegradation]            REAL             NOT NULL,
    [Id]                                 BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                               NVARCHAR (100)   NULL,
    [Flags]                              BIGINT           NULL,
    [TenantID]                           INT              NOT NULL,
    [DateCreated]                        DATETIME         NOT NULL,
    [DateLastModified]                   DATETIME         NULL,
    [CreatedByName]                      NVARCHAR (100)   NULL,
    [CreatedByID]                        UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]                     UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]                 NVARCHAR (100)   NULL,
    [Version]                            INT              NOT NULL,
    [IsDeleted]                          BIT              NOT NULL,
    [ExternalID]                         NVARCHAR (50)    NULL,
    [TagString]                          NVARCHAR (1000)  NULL,
    [MonthlySlopeIndex]                  INT              DEFAULT ((0)) NOT NULL,
    [AverageSlopeIndex]                  INT              DEFAULT ((0)) NOT NULL,
    [GenabilityTotalCost]                FLOAT (53)       DEFAULT ((0)) NOT NULL,
    [GenabilityTotalConsumption]         FLOAT (53)       DEFAULT ((0)) NOT NULL,
    [GenabilityTieredAverageUtilityCost] FLOAT (53)       DEFAULT ((0)) NOT NULL,
    [LoanPricePerWatt]                   FLOAT (53)       NULL,
    [LoanDownPayment]                    FLOAT (53)       NULL,
    [FederalInvestmentTaxCredit]         FLOAT (53)       NULL,
    [LocalPBI]                           FLOAT (53)       NULL,
    [LocalPBITerm]                       INT              NULL,
    [LocalSREC]                          FLOAT (53)       NULL,
    [LocalSRECTerm]                      INT              NULL,
    [UpfrontRebate]                      FLOAT (53)       NULL,
    [PpaIsSelected]                      BIT              NULL,
    [LoanIsSelected]                     BIT              NULL,
    [Mosaic12YearLoanIsSelected]         BIT              NULL,
    [Mosaic20YearLoanIsSelected]         BIT              NULL,
    [GreenSky12YearLoanIsSelected]       BIT              NULL,
    [GreenSky20YearLoanIsSelected]       BIT              NULL,
    [EscalationRate]                     FLOAT (53)       NULL,
    [LoanPricePerWattPricingOption]      INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_solar.Systems] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.Systems_solar.Proposals_Guid] FOREIGN KEY ([Guid]) REFERENCES [solar].[Proposals] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[Systems]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Guid]
    ON [solar].[Systems]([Guid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[Systems]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[Systems]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[Systems]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[Systems]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[Systems]([ExternalID] ASC);

