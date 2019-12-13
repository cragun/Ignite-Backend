CREATE TABLE [dbo].[PropertyAttributesBak] (
    [Guid]               UNIQUEIDENTIFIER NOT NULL,
    [PropertyID]         UNIQUEIDENTIFIER NOT NULL,
    [Description]        NVARCHAR (200)   NULL,
    [Value]              NVARCHAR (50)    NULL,
    [AttributeKey]       NVARCHAR (50)    NULL,
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
    [OwnerID]            NVARCHAR (50)    NULL,
    [DisplayType]        NVARCHAR (50)    NULL
);

