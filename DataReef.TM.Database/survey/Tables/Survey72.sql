CREATE TABLE [survey].[Survey72] (
    [Guid]                    UNIQUEIDENTIFIER NOT NULL,
    [ClientName]              NVARCHAR (MAX)   NOT NULL,
    [AlternateClientName]     NVARCHAR (MAX)   NULL,
    [StreetAddress]           NVARCHAR (MAX)   NOT NULL,
    [City]                    NVARCHAR (MAX)   NOT NULL,
    [State]                   NVARCHAR (MAX)   NOT NULL,
    [ZipCode]                 NVARCHAR (MAX)   NULL,
    [PhoneNumber]             NVARCHAR (MAX)   NOT NULL,
    [AlternatePhoneNumber]    NVARCHAR (MAX)   NULL,
    [Email]                   NVARCHAR (MAX)   NULL,
    [TypeOfResidence]         NVARCHAR (MAX)   NULL,
    [EquipmentAge]            INT              NOT NULL,
    [EquipmentLocation]       NVARCHAR (MAX)   NULL,
    [AgeOfHome]               INT              NOT NULL,
    [TypeOfAppointment]       NVARCHAR (MAX)   NULL,
    [Price]                   FLOAT (53)       NOT NULL,
    [AppointmentDate]         DATETIME         NOT NULL,
    [AppointmentTimeInterval] NVARCHAR (MAX)   NOT NULL,
    [RepresentativeNotes]     NVARCHAR (MAX)   NULL,
    [RepresentativeName]      NVARCHAR (MAX)   NULL,
    [LeadSource]              NVARCHAR (MAX)   NULL,
    [Id]                      BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                    NVARCHAR (100)   NULL,
    [Flags]                   BIGINT           NULL,
    [TenantID]                INT              NOT NULL,
    [DateCreated]             DATETIME         NOT NULL,
    [DateLastModified]        DATETIME         NULL,
    [CreatedByName]           NVARCHAR (100)   NULL,
    [CreatedByID]             UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]          UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]      NVARCHAR (100)   NULL,
    [Version]                 INT              NOT NULL,
    [IsDeleted]               BIT              NOT NULL,
    [ExternalID]              NVARCHAR (50)    NULL,
    [TagString]               NVARCHAR (1000)  NULL,
    CONSTRAINT [PK_survey.Survey72] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_survey.Survey72_dbo.Properties_Guid] FOREIGN KEY ([Guid]) REFERENCES [dbo].[Properties] ([Guid])
);


GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [survey].[Survey72]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Guid]
    ON [survey].[Survey72]([Guid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [survey].[Survey72]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [survey].[Survey72]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [survey].[Survey72]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [survey].[Survey72]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [survey].[Survey72]([ExternalID] ASC);

