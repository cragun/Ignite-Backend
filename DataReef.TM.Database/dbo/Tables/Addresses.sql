CREATE TABLE [dbo].[Addresses] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NULL,
    [PersonID]           UNIQUEIDENTIFIER NULL,
    [Address1]           NVARCHAR (50)    NULL,
    [Address2]           NVARCHAR (20)    NULL,
    [City]               NVARCHAR (50)    NULL,
    [State]              NVARCHAR (2)     NULL,
    [ZipCode]            NVARCHAR (5)     NULL,
    [PlusFour]           NVARCHAR (4)     NULL,
    [County]             NVARCHAR (50)    NULL,
    [Latitude]           REAL             NOT NULL,
    [Longitude]          REAL             NOT NULL,
    [Description]        NVARCHAR (400)   NULL,
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
    CONSTRAINT [PK_dbo.Addresses] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Addresses_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid]),
    CONSTRAINT [FK_dbo.Addresses_dbo.People_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Addresses]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[Addresses]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[Addresses]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Addresses]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Addresses]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Addresses]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Addresses]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Addresses]([ExternalID] ASC);

