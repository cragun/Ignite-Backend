CREATE TABLE [dbo].[ous_backup] (
    [Guid]                    UNIQUEIDENTIFIER NOT NULL,
    [Number]                  NVARCHAR (50)    NULL,
    [ParentID]                UNIQUEIDENTIFIER NULL,
    [IsDisabled]              BIT              NOT NULL,
    [AccountID]               UNIQUEIDENTIFIER NOT NULL,
    [Website]                 NVARCHAR (100)   NULL,
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
    [WellKnownText]           NVARCHAR (MAX)   NULL,
    [CentroidLat]             REAL             NOT NULL,
    [CentroidLon]             REAL             NOT NULL,
    [Radius]                  REAL             NOT NULL,
    [IsDeletableByClient]     BIT              NOT NULL,
    [RootOrganizationID]      UNIQUEIDENTIFIER NULL,
    [BatchPrescreenTableName] NVARCHAR (100)   NULL,
    [OrganizationType]        INT              NULL,
    [PartnerID]               NVARCHAR (50)    NULL,
    [CompanyID]               NVARCHAR (50)    NULL,
    [ShapesVersion]           INT              NULL,
    [RootOrganizationName]    NVARCHAR (MAX)   NULL
);

