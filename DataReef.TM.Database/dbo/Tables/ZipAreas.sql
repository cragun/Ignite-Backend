CREATE TABLE [dbo].[ZipAreas] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Status]             INT              NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
    [PropertyCount]      INT              NOT NULL,
    [ActiveStartDate]    DATETIME         NULL,
    [LastPurchaseDate]   DATETIME         NULL,
    [ExpiryMinutes]      INT              NOT NULL,
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
    CONSTRAINT [PK_dbo.ZipAreas] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.ZipAreas_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[ZipAreas]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[ZipAreas]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[ZipAreas]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[ZipAreas]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[ZipAreas]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[ZipAreas]([OUID] ASC);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[ZipAreas]([Id] ASC);

