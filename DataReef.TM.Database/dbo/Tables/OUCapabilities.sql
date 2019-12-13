CREATE TABLE [dbo].[OUCapabilities] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [OUID]               UNIQUEIDENTIFIER NOT NULL,
    [CapabilityID]       UNIQUEIDENTIFIER NULL,
    [StartDate]          DATETIME         NULL,
    [EndDate]            DATETIME         NULL,
    [DiscardInheritance] BIT              NOT NULL,
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
    CONSTRAINT [PK_dbo.OUCapabilities] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.OUCapabilities_dbo.Capabilities_CapabilityID] FOREIGN KEY ([CapabilityID]) REFERENCES [dbo].[Capabilities] ([Guid]),
    CONSTRAINT [FK_dbo.OUCapabilities_dbo.OUs_OUID] FOREIGN KEY ([OUID]) REFERENCES [dbo].[OUs] ([Guid])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[OUCapabilities]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[OUCapabilities]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[OUCapabilities]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[OUCapabilities]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[OUCapabilities]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CapabilityID]
    ON [dbo].[OUCapabilities]([CapabilityID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_OUID]
    ON [dbo].[OUCapabilities]([OUID] ASC);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[OUCapabilities]([Id] ASC);

