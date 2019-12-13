CREATE TABLE [dbo].[AreaPurchases] (
    [Guid]                UNIQUEIDENTIFIER NOT NULL,
    [PersonID]            UNIQUEIDENTIFIER NOT NULL,
    [AreaID]              UNIQUEIDENTIFIER NOT NULL,
    [CompletionDate]      DATETIME         NULL,
    [ErrorString]         NVARCHAR (MAX)   NULL,
    [NumberOfTokens]      INT              NOT NULL,
    [TokenPriceInDollars] REAL             NOT NULL,
    [Id]                  BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (100)   NULL,
    [Flags]               BIGINT           NULL,
    [TenantID]            INT              NOT NULL,
    [DateCreated]         DATETIME         NOT NULL,
    [DateLastModified]    DATETIME         NULL,
    [CreatedByName]       NVARCHAR (100)   NULL,
    [CreatedByID]         UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]      UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]  NVARCHAR (100)   NULL,
    [Version]             INT              NOT NULL,
    [IsDeleted]           BIT              NOT NULL,
    [ExternalID]          NVARCHAR (50)    NULL,
    [TagString]           NVARCHAR (1000)  NULL,
    [Status]              INT              DEFAULT ((0)) NOT NULL,
    [OUID]                UNIQUEIDENTIFIER DEFAULT ('00000000-0000-0000-0000-000000000000') NOT NULL,
    CONSTRAINT [PK_dbo.AreaPurchases] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.AreaPurchases_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid]),
    CONSTRAINT [FK_dbo.AreaPurchases_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid]),
    CONSTRAINT [FK_dbo.AreaPurchases_dbo.ZipAreas_AreaID] FOREIGN KEY ([AreaID]) REFERENCES [dbo].[ZipAreas] ([Guid])
);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[AreaPurchases]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[AreaPurchases]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[AreaPurchases]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[AreaPurchases]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[AreaPurchases]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[AreaPurchases]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[AreaPurchases]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AreaID]
    ON [dbo].[AreaPurchases]([AreaID] ASC);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[AreaPurchases]([Id] ASC);

