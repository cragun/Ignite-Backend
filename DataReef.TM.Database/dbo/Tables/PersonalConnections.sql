CREATE TABLE [dbo].[PersonalConnections] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [Message]            NVARCHAR (100)   NULL,
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
    [FromPersonID]       UNIQUEIDENTIFIER NOT NULL,
    [ToPersonID]         UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_dbo.PersonalConnections] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.PersonalConnections_dbo.People_FromPerson_Guid] FOREIGN KEY ([FromPersonID]) REFERENCES [dbo].[People] ([Guid]),
    CONSTRAINT [FK_dbo.PersonalConnections_dbo.People_ToPerson_Guid] FOREIGN KEY ([ToPersonID]) REFERENCES [dbo].[People] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[PersonalConnections]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[PersonalConnections]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[PersonalConnections]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[PersonalConnections]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[PersonalConnections]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[PersonalConnections]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_FromPersonID]
    ON [dbo].[PersonalConnections]([FromPersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ToPersonID]
    ON [dbo].[PersonalConnections]([ToPersonID] ASC);

