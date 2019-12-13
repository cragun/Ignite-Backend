CREATE TABLE [reporting].[OUReports] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [ReportID]           UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
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
    CONSTRAINT [PK_reporting.OUReports] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_reporting.OUReports_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid]),
    CONSTRAINT [FK_reporting.OUReports_reporting.Reports_ReportID] FOREIGN KEY ([ReportID]) REFERENCES [reporting].[Reports] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [reporting].[OUReports]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ReportID]
    ON [reporting].[OUReports]([ReportID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [reporting].[OUReports]([OUID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [reporting].[OUReports]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [reporting].[OUReports]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [reporting].[OUReports]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [reporting].[OUReports]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [reporting].[OUReports]([ExternalID] ASC);

