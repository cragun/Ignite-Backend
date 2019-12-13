CREATE TABLE [solar].[Proposals] (
    [Guid]                                   UNIQUEIDENTIFIER NOT NULL,
    [Status]                                 INT              NOT NULL,
    [PersonID]                               UNIQUEIDENTIFIER NOT NULL,
    [NameOfOwner]                            NVARCHAR (MAX)   NULL,
    [Description]                            NVARCHAR (MAX)   NULL,
    [PropertyID]                             UNIQUEIDENTIFIER NULL,
    [AddressID]                              UNIQUEIDENTIFIER NULL,
    [Address]                                NVARCHAR (MAX)   NULL,
    [Address2]                               NVARCHAR (MAX)   NULL,
    [City]                                   NVARCHAR (MAX)   NULL,
    [State]                                  NVARCHAR (MAX)   NULL,
    [ZipCode]                                NVARCHAR (MAX)   NULL,
    [PlusFour]                               NVARCHAR (MAX)   NULL,
    [Lat]                                    REAL             NOT NULL,
    [Lon]                                    REAL             NOT NULL,
    [IntegrationDate]                        DATETIME         NULL,
    [Id]                                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                                   NVARCHAR (100)   NULL,
    [Flags]                                  BIGINT           NULL,
    [TenantID]                               INT              NOT NULL,
    [DateCreated]                            DATETIME         NOT NULL,
    [DateLastModified]                       DATETIME         NULL,
    [CreatedByName]                          NVARCHAR (100)   NULL,
    [CreatedByID]                            UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]                         UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]                     NVARCHAR (100)   NULL,
    [Version]                                INT              NOT NULL,
    [IsDeleted]                              BIT              NOT NULL,
    [ExternalID]                             NVARCHAR (50)    NULL,
    [TagString]                              NVARCHAR (1000)  NULL,
    [SalesTax]                               REAL             CONSTRAINT [DF__Proposals__Sales__5F3414E9] DEFAULT ((0)) NOT NULL,
    [SalesRepAvgUtilityCost]                 REAL             CONSTRAINT [DF__Proposals__Sales__7B9B496D] DEFAULT ((0)) NOT NULL,
    [GenabilityElectricityProviderProfileID] NVARCHAR (MAX)   NULL,
    [GenabilitySolarProviderProfileID]       NVARCHAR (MAX)   NULL,
    [DateSigned]                             DATETIME         CONSTRAINT [DF__Proposals__DateS__2B203F5D] DEFAULT ('1900-01-01T00:00:00.000') NULL,
    [SignedProposalURL]                      NVARCHAR (MAX)   NULL,
    [UnsignedContractURL]                    NVARCHAR (MAX)   NULL,
    [SignedContractURL]                      NVARCHAR (MAX)   NULL,
    [ContractExpiryDate]                     DATETIME         CONSTRAINT [DF__Proposals__Contr__5A9A4855] DEFAULT ('1900-01-01T00:00:00.000') NULL,
    CONSTRAINT [PK_solar.Proposals] PRIMARY KEY NONCLUSTERED ([Guid] ASC)
);






GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[Proposals]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[Proposals]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[Proposals]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[Proposals]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[Proposals]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[Proposals]([ExternalID] ASC);

