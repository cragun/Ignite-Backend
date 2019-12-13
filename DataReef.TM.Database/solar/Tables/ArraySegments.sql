CREATE TABLE [solar].[ArraySegments] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [SolarArrayID]       UNIQUEIDENTIFIER NOT NULL,
    [Index]              INT              NOT NULL,
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
    [X]                  FLOAT (53)       NOT NULL,
    [Y]                  FLOAT (53)       NOT NULL,
    CONSTRAINT [PK_solar.ArraySegments] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_solar.ArraySegments_solar.Arrays_SolarArrayID] FOREIGN KEY ([SolarArrayID]) REFERENCES [solar].[Arrays] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [solar].[ArraySegments]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolarArrayID]
    ON [solar].[ArraySegments]([SolarArrayID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [solar].[ArraySegments]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [solar].[ArraySegments]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [solar].[ArraySegments]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [solar].[ArraySegments]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [solar].[ArraySegments]([ExternalID] ASC);

