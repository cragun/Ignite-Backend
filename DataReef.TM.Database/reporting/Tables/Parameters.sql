CREATE TABLE [reporting].[Parameters] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [ReportID]           UNIQUEIDENTIFIER NOT NULL,
    [ParameterType]      NVARCHAR (MAX)   NULL,
    [IsRequired]         BIT              NOT NULL,
    [IsStartDate]        BIT              NOT NULL,
    [IsEndDate]          BIT              NOT NULL,
    [IsOUID]             BIT              NOT NULL,
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
    CONSTRAINT [PK_reporting.Parameters] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_reporting.Parameters_reporting.Reports_ReportID] FOREIGN KEY ([ReportID]) REFERENCES [reporting].[Reports] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [reporting].[Parameters]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReportID]
    ON [reporting].[Parameters]([ReportID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [reporting].[Parameters]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [reporting].[Parameters]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [reporting].[Parameters]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [reporting].[Parameters]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [reporting].[Parameters]([ExternalID] ASC);

