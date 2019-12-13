CREATE TABLE [dbo].[Inquiries] (
    [Guid]                        UNIQUEIDENTIFIER NOT NULL,
    [PropertyID]                  UNIQUEIDENTIFIER NOT NULL,
    [PersonID]                    UNIQUEIDENTIFIER NOT NULL,
    [Notes]                       NVARCHAR (500)   NULL,
    [Status]                      INT              NOT NULL,
    [FollowUpDate]                DATETIME         NULL,
    [ShouldIntegrateWithCalendar] BIT              NULL,
    [Color]                       NVARCHAR (50)    NULL,
    [Annotation]                  NVARCHAR (50)    NULL,
    [IsArchive]                   BIT              NOT NULL,
    [IsLead]                      BIT              NOT NULL,
    [Lat]                         FLOAT (53)       NULL,
    [Lon]                         FLOAT (53)       NULL,
    [Id]                          BIGINT           IDENTITY (1, 1) NOT NULL,
    [Name]                        NVARCHAR (100)   NULL,
    [Flags]                       BIGINT           NULL,
    [TenantID]                    INT              NOT NULL,
    [DateCreated]                 DATETIME         NOT NULL,
    [DateLastModified]            DATETIME         NULL,
    [CreatedByName]               NVARCHAR (100)   NULL,
    [CreatedByID]                 UNIQUEIDENTIFIER NULL,
    [LastModifiedBy]              UNIQUEIDENTIFIER NULL,
    [LastModifiedByName]          NVARCHAR (100)   NULL,
    [Version]                     INT              NOT NULL,
    [IsDeleted]                   BIT              NOT NULL,
    [ExternalID]                  NVARCHAR (50)    NULL,
    [TagString]                   NVARCHAR (1000)  NULL,
    [OUID]                        UNIQUEIDENTIFIER NULL,
    [ActivityType]                INT              DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_dbo.Inquiries] PRIMARY KEY NONCLUSTERED ([Guid] ASC),
    CONSTRAINT [FK_dbo.Inquiries_dbo.Properties_PropertyID] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Properties] ([Guid]),
    CONSTRAINT [FK_dbo.Inquiries_dbo.Users_PersonID] FOREIGN KEY ([PersonID]) REFERENCES [dbo].[Users] ([Guid])
);




GO
CREATE CLUSTERED INDEX [CLUSTERED_INDEX_ON_LONG]
    ON [dbo].[Inquiries]([Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PropertyID]
    ON [dbo].[Inquiries]([PropertyID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PersonID]
    ON [dbo].[Inquiries]([PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_TenantID]
    ON [dbo].[Inquiries]([TenantID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_DateCreated]
    ON [dbo].[Inquiries]([DateCreated] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_CreatedByID]
    ON [dbo].[Inquiries]([CreatedByID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Version]
    ON [dbo].[Inquiries]([Version] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalID]
    ON [dbo].[Inquiries]([ExternalID] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Inquiries_8_629577281__K18_K14_K5_K2_K3]
    ON [dbo].[Inquiries]([DateCreated] ASC, [Id] ASC, [Status] ASC, [PropertyID] ASC, [PersonID] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Inquiries_8_629577281__K18_K14_K5_K2]
    ON [dbo].[Inquiries]([DateCreated] ASC, [Id] ASC, [Status] ASC, [PropertyID] ASC);


GO
CREATE NONCLUSTERED INDEX [_dta_index_Inquiries_8_629577281__K2_K18_K14]
    ON [dbo].[Inquiries]([PropertyID] ASC, [DateCreated] ASC, [Id] ASC);


GO
CREATE trigger tgOUID on dbo.Inquiries
AFTER INSERT

as

begin


declare @guid UniqueIdentifier
select top 1 @Guid = Guid from Inserted

update Inquiries set OUID = t.OUID
from Inquiries i
inner join Properties p on i.PropertyID = p.guid
inner join Territories t on p.TerritoryID = t.Guid
where i.Guid=@Guid

end





--5


--select * from ous where guid='44497BD5-C0F1-47C1-AF4D-E4CAFEF4B8FD'