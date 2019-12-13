CREATE TABLE [dbo].[Occupants] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [FirstName]          NVARCHAR (50)    NULL,
    [LastName]           NVARCHAR (50)    NULL,
    [PropertyID]         UNIQUEIDENTIFIER NOT NULL,
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
    [MiddleInitial]      NVARCHAR (MAX)   NULL,
    [LastNameSuffix]     NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_dbo.Occupants] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Occupants_dbo.Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Occupants]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PropertyID]
    ON [dbo].[Occupants]([PropertyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Occupants]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Occupants]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Occupants]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Occupants]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Occupants]([ExternalID] ASC);

