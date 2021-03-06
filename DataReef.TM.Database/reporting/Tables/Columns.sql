CREATE TABLE [reporting].[Columns] (
    [Guid]                   UNIQUEIDENTIFIER NOT NULL,
    [ReportID]               UNIQUEIDENTIFIER NOT NULL,
    [Width]                  INT              NOT NULL,
    [Position]               INT              NOT NULL,
    [FormatString]           NVARCHAR (MAX)   NULL,
    [Caption]                NVARCHAR (MAX)   NULL,
    [IsHidden]               BIT              NOT NULL,
    [DrillDownReportID]      UNIQUEIDENTIFIER NULL,
    [DrillDownParameterName] NVARCHAR (MAX)   NULL,
    [Id]                     BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                   NVARCHAR (100)   NULL,
    [Flags]                  BIGINT           NULL,
    [TenantID]               INT              NOT NULL,
    [DateCreated]            DATETIME         NOT NULL,
    [DateLastModified]       DATETIME         NULL,
    [CreatedByName]          NVARCHAR (100)   NULL,
    [CreatedByID]            UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]         UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]     NVARCHAR (100)   NULL,
    [Version]                INT              NOT NULL,
    [IsDeleted]              BIT              NOT NULL,
    [ExternalID]             NVARCHAR (50)    NULL,
    [TagString]              NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_reporting.Columns] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_reporting.Columns_reporting.Reports_ReportID] FOREIGN KEY ([ReportID]) REFERENCES [reporting].[Reports] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [reporting].[Columns]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReportID]
    ON [reporting].[Columns]([ReportID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [reporting].[Columns]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [reporting].[Columns]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [reporting].[Columns]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [reporting].[Columns]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [reporting].[Columns]([ExternalID] ASC);

