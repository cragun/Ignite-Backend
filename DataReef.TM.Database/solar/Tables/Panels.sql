CREATE TABLE [solar].[Panels] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [IsActive]           BIT              NOT NULL,
    [Description]        NVARCHAR (MAX)   NULL,
    [Watts]              INT              NOT NULL,
    [Width]              INT              NOT NULL,
    [Height]             INT              NOT NULL,
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
    CONSTRAINT [PK_solar.Panels] PRIMARY KEY NONCLUSTERED ([Guid] ASC)
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[Panels]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[Panels]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[Panels]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[Panels]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[Panels]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[Panels]([ExternalID] ASC);

