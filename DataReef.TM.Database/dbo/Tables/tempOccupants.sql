CREATE TABLE [dbo].[tempOccupants] (
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
    [LastNameSuffix]     NVARCHAR (MAX)   NULL
);

