CREATE TABLE [dbo].[OURoles] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [IsActive]           BIT              NOT NULL,
    [IsAdmin]            BIT              NOT NULL,
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
    [IsOwner]            BIT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.OURoles] PRIMARY KEY NONCLUSTERED ([Guid] ASC)
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OURoles]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OURoles]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OURoles]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OURoles]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OURoles]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OURoles]([ExternalID] ASC);

