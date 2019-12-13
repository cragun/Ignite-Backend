CREATE TABLE [dbo].[States] (
    [Guid]               UNIQUEIDENTIFIER  NOT NULL,
    [Abbreviation]       NVARCHAR (50)     NULL,
    [CensusID]           NVARCHAR (50)     NULL,
    [Latitude]           REAL              NULL,
    [Longitude]          REAL              NULL,
    [NWLatitude]         REAL              NULL,
    [NWLongitude]        REAL              NULL,
    [SELatitude]         REAL              NULL,
    [SELongitude]        REAL              NULL,
    [ParentID]           UNIQUEIDENTIFIER  NULL,
    [Centroid]           [sys].[geography] NULL,
    [Shape]              [sys].[geography] NULL,
    [ShapeReduced]       [sys].[geography] NULL,
    [BoundingBox]        [sys].[geography] NULL,
    [HousingCount]       INT               DEFAULT ((0)) NOT NULL,
    [ResidentCount]      INT               DEFAULT ((0)) NOT NULL,
    [Radius]             REAL              DEFAULT ((0)) NOT NULL,
    [QuadKey1]           NVARCHAR (1)      NULL,
    [QuadKey2]           NVARCHAR (2)      NULL,
    [QuadKey3]           NVARCHAR (3)      NULL,
    [QuadKey4]           NVARCHAR (4)      NULL,
    [QuadKey5]           NVARCHAR (5)      NULL,
    [QuadKey6]           NVARCHAR (6)      NULL,
    [QuadKey7]           NVARCHAR (7)      NULL,
    [QuadKey8]           NVARCHAR (8)      NULL,
    [QuadKey9]           NVARCHAR (9)      NULL,
    [QuadKey10]          NVARCHAR (10)     NULL,
    [QuadKey11]          NVARCHAR (11)     NULL,
    [QuadKey12]          NVARCHAR (12)     NULL,
    [QuadKey13]          NVARCHAR (13)     NULL,
    [QuadKey14]          NVARCHAR (14)     NULL,
    [QuadKey15]          NVARCHAR (15)     NULL,
    [QuadKey16]          NVARCHAR (16)     NULL,
    [QuadKey17]          NVARCHAR (17)     NULL,
    [QuadKey18]          NVARCHAR (18)     NULL,
    [QuadKey19]          NVARCHAR (19)     NULL,
    [QuadKey20]          NVARCHAR (20)     NULL,
    [QuadKey21]          NVARCHAR (21)     NULL,
    [QuadKey22]          NVARCHAR (22)     NULL,
    [QuadKey23]          NVARCHAR (23)     NULL,
    [QuadKey24]          NVARCHAR (24)     NULL,
    [Id]                 BIGINT            IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100)    NULL,
    [Flags]              BIGINT            NULL,
    [TenantID]           INT               DEFAULT ((0)) NOT NULL,
    [DateCreated]        DATETIME          DEFAULT ('1900-01-01T00:00:00.000') NOT NULL,
    [DateLastModified]   DATETIME          NULL,
    [CreatedByName]      NVARCHAR (100)    NULL,
    [CreatedByID]        UNIQUEIDENTIFIER  NULL,
    [LastModifiedBy]     UNIQUEIDENTIFIER  NULL,
    [LastModifiedByName] NVARCHAR (100)    NULL,
    [Version]            INT               DEFAULT ((0)) NOT NULL,
    [IsDeleted]          BIT               DEFAULT ((0)) NOT NULL,
    [ExternalID]         NVARCHAR (50)     NULL,
    [TagString]          NVARCHAR (1000)   NULL,
    CONSTRAINT [PK_dbo.States] PRIMARY KEY NONCLUSTERED ([Guid] ASC)
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[States]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[States]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[States]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[States]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[States]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[States]([ExternalID] ASC);

