CREATE TABLE [dbo].[PropertyAttributes] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [PropertyID]         UNIQUEIDENTIFIER NOT NULL,
    [Description]        NVARCHAR (200)   NULL,
    [Value]              NVARCHAR (50)    NULL,
    [AttributeKey]       NVARCHAR (50)    NULL,
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
    [OwnerID]            NVARCHAR (50)    NULL,
    [DisplayType]        NVARCHAR (50)    NULL,
    [ExpiryMinutes]      INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.PropertyAttributes] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.PropertyAttributes_dbo.Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[PropertyAttributes]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PropertyID]
    ON [dbo].[PropertyAttributes]([PropertyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[PropertyAttributes]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[PropertyAttributes]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[PropertyAttributes]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[PropertyAttributes]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[PropertyAttributes]([ExternalID] ASC);

