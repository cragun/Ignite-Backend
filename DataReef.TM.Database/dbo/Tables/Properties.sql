CREATE TABLE [dbo].[Properties] (
    [Guid]                        UNIQUEIDENTIFIER  NOT NULL,
    [PropertyType]                INT               NOT NULL,
    [HouseNumber]                 NVARCHAR (20)     NULL,
    [IsEven]                      BIT               NOT NULL,
    [Address1]                    NVARCHAR (50)     NULL,
    [Address2]                    NVARCHAR (20)     NULL,
    [City]                        NVARCHAR (50)     NULL,
    [State]                       NVARCHAR (2)      NULL,
    [ZipCode]                     NVARCHAR (5)      NULL,
    [PlusFour]                    NVARCHAR (4)      NULL,
    [DeliveryPoint]               NVARCHAR (2)      NULL,
    [StreetName]                  NVARCHAR (50)     NULL,
    [TerritoryID]                 UNIQUEIDENTIFIER  NOT NULL,
    [SortOrder]                   INT               NOT NULL,
    [AddressID]                   UNIQUEIDENTIFIER  NULL,
    [Abbreviation]                NVARCHAR (50)     NULL,
    [CensusID]                    NVARCHAR (50)     NULL,
    [Latitude]                    REAL              NULL,
    [Longitude]                   REAL              NULL,
    [NWLatitude]                  REAL              NULL,
    [NWLongitude]                 REAL              NULL,
    [SELatitude]                  REAL              NULL,
    [SELongitude]                 REAL              NULL,
    [ParentID]                    UNIQUEIDENTIFIER  NULL,
    [Centroid]                    [sys].[geography] NULL,
    [Shape]                       [sys].[geography] NULL,
    [ShapeReduced]                [sys].[geography] NULL,
    [BoundingBox]                 [sys].[geography] NULL,
    [HousingCount]                INT               DEFAULT ((0)) NOT NULL,
    [ResidentCount]               INT               DEFAULT ((0)) NOT NULL,
    [Radius]                      REAL              DEFAULT ((0)) NOT NULL,
    [QuadKey1]                    NVARCHAR (1)      NULL,
    [QuadKey2]                    NVARCHAR (2)      NULL,
    [QuadKey3]                    NVARCHAR (3)      NULL,
    [QuadKey4]                    NVARCHAR (4)      NULL,
    [QuadKey5]                    NVARCHAR (5)      NULL,
    [QuadKey6]                    NVARCHAR (6)      NULL,
    [QuadKey7]                    NVARCHAR (7)      NULL,
    [QuadKey8]                    NVARCHAR (8)      NULL,
    [QuadKey9]                    NVARCHAR (9)      NULL,
    [QuadKey10]                   NVARCHAR (10)     NULL,
    [QuadKey11]                   NVARCHAR (11)     NULL,
    [QuadKey12]                   NVARCHAR (12)     NULL,
    [QuadKey13]                   NVARCHAR (13)     NULL,
    [QuadKey14]                   NVARCHAR (14)     NULL,
    [QuadKey15]                   NVARCHAR (15)     NULL,
    [QuadKey16]                   NVARCHAR (16)     NULL,
    [QuadKey17]                   NVARCHAR (17)     NULL,
    [QuadKey18]                   NVARCHAR (18)     NULL,
    [QuadKey19]                   NVARCHAR (19)     NULL,
    [QuadKey20]                   NVARCHAR (20)     NULL,
    [QuadKey21]                   NVARCHAR (21)     NULL,
    [QuadKey22]                   NVARCHAR (22)     NULL,
    [QuadKey23]                   NVARCHAR (23)     NULL,
    [QuadKey24]                   NVARCHAR (24)     NULL,
    [Id]                          BIGINT            IDENTITY (1, 1) NOT NULL,
    [Name]                        NVARCHAR (100)    NULL,
    [Flags]                       BIGINT            NULL,
    [TenantID]                    INT               DEFAULT ((0)) NOT NULL,
    [DateCreated]                 DATETIME          DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [DateLastModified]            DATETIME          NULL,
    [CreatedByName]               NVARCHAR (100)    NULL,
    [CreatedByID]                 UNIQUEIDENTIFIER  NULL,
    [LastModifiedBy]              UNIQUEIDENTIFIER  NULL,
    [LastModifiedByName]          NVARCHAR (100)    NULL,
    [Version]                     INT               DEFAULT ((0)) NOT NULL,
    [IsDeleted]                   BIT               DEFAULT ((0)) NOT NULL,
    [ExternalID]                  NVARCHAR (50)     NULL,
    [TagString]                   NVARCHAR (1000)   NULL,
    [IsArchive]                   BIT               CONSTRAINT [DF_Properties_IsArchive] DEFAULT ((0)) NOT NULL,
    [SunEdisonCustomerID]         NVARCHAR (MAX)    NULL,
    [GenabilityProviderAccountID] NVARCHAR (MAX)    NULL,
    [GenabilityAccountID]         NVARCHAR (MAX)    NULL,
    CONSTRAINT [PK_dbo.Properties] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Properties_dbo.Territories_TerritoryID] FOREIGN KEY ([TerritoryID]) REFERENCES [dbo].[Territories] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Properties]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ParentID]
    ON [dbo].[Properties]([ParentID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TerritoryID]
    ON [dbo].[Properties]([TerritoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Properties]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Properties]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Properties]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Properties]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Properties]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Properties_8_1557580587__K13_K56_K1]
    ON [dbo].[Properties]([TerritoryID] ASC, [Id] ASC, [Guid] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Properties_8_1557580587__K1_K56_K13]
    ON [dbo].[Properties]([Guid] ASC, [Id] ASC, [TerritoryID] ASC);


GO
CREATE NONCLUSTERED INDEX [idx_geo]
    ON [dbo].[Properties]([TerritoryID] ASC, [Latitude] ASC, [Longitude] ASC, [Id] ASC)
    INCLUDE([Guid]);


GO
CREATE NONCLUSTERED INDEX [idx_territoryid_guid]
    ON [dbo].[Properties]([TerritoryID] ASC, [Guid] ASC);

