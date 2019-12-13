CREATE TABLE [dbo].[OUs] (
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
    [CentroidLat]             REAL             DEFAULT ((0)) NOT NULL,
    [CentroidLon]             REAL             DEFAULT ((0)) NOT NULL,
    [Radius]                  REAL             DEFAULT ((0)) NOT NULL,
    [IsDeletableByClient]     BIT              DEFAULT ((0)) NOT NULL,
    [RootOrganizationID]      UNIQUEIDENTIFIER NULL,
    [BatchPrescreenTableName] NVARCHAR (100)   NULL,
    [OrganizationType]        INT              NULL,
    [PartnerID]               NVARCHAR (50)    NULL,
    [CompanyID]               NVARCHAR (50)    NULL,
    [ShapesVersion]           INT              NULL,
    [RootOrganizationName]    NVARCHAR (MAX)   NULL,
    [TokenPriceInDollars]     REAL             NULL,
    [ActivityTypes]           INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.OUs] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUs_dbo.Accounts_AccountID] FOREIGN KEY ([AccountID]) REFERENCES [dbo].[Accounts] ([Guid]),
    CONSTRAINT [FK_dbo.OUs_dbo.OUs_ParentID] FOREIGN KEY ([ParentID]) REFERENCES [dbo].[OUs] ([Guid]),
    CONSTRAINT [FK_dbo.OUs_dbo.OUs_RootOrganizationID] FOREIGN KEY ([RootOrganizationID]) REFERENCES [dbo].[OUs] ([Guid])
);






GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUs]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ParentID]
    ON [dbo].[OUs]([ParentID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AccountID]
    ON [dbo].[OUs]([AccountID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUs]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUs]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUs]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUs]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUs]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RootOrganizationID]
    ON [dbo].[OUs]([RootOrganizationID] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_OUs_8_309576141__K3_K18_K7_K1_K8]
    ON [dbo].[OUs]([ParentID] ASC, [IsDeleted] ASC, [Id] ASC, [Guid] ASC, [Name] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_OUs_8_309576141__K1_K18_K3_K7_K8]
    ON [dbo].[OUs]([Guid] ASC, [IsDeleted] ASC, [ParentID] ASC, [Id] ASC, [Name] ASC);


GO

