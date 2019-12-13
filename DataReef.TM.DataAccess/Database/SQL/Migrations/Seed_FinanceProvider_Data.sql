DELETE FROM [finance].[OUAssociation]
DELETE FROM [finance].[FinanceDetails]
DELETE FROM [finance].[planDefinitions]
DELETE FROM [finance].[Providers]

SET IDENTITY_INSERT [finance].[Providers] ON 

GO
INSERT [finance].[Providers] ([Guid], [ImageUrl], [Id], [Name], [Flags], [TenantID], [DateCreated], [DateLastModified], [CreatedByName], [CreatedByID], [LastModifiedBy], [LastModifiedByName], [Version], [IsDeleted], [ExternalID], [TagString]) VALUES (N'77034eac-e0c2-42ee-a7ba-b3fa280ed0c5', N'http://sprucefinance.com/wp-content/uploads/2016/01/spruce-logo-600x215.png', 1, N'Spruce Financial', NULL, 0, CAST(N'2017-02-06 23:05:57.713' AS DateTime), NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
GO
INSERT [finance].[Providers] ([Guid], [ImageUrl], [Id], [Name], [Flags], [TenantID], [DateCreated], [DateLastModified], [CreatedByName], [CreatedByID], [LastModifiedBy], [LastModifiedByName], [Version], [IsDeleted], [ExternalID], [TagString]) VALUES (N'18be1087-2a37-481e-9283-f104b5c391fa', N'http://kbispressroom.com/wp-content/uploads/image1.jpg', 2, N'GreenSky', NULL, 0, CAST(N'2017-02-06 23:06:37.197' AS DateTime), NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
GO
INSERT [finance].[Providers] ([Guid], [ImageUrl], [Id], [Name], [Flags], [TenantID], [DateCreated], [DateLastModified], [CreatedByName], [CreatedByID], [LastModifiedBy], [LastModifiedByName], [Version], [IsDeleted], [ExternalID], [TagString]) VALUES (N'f1bd8a3f-3050-4849-a11e-f3debf67f150', N'http://www.joberoofing.com/wp-content/uploads/2016/06/enerbankusa-logo.jpg', 3, N'EnerBank', NULL, 0, CAST(N'2017-02-06 23:07:30.333' AS DateTime), NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
GO
INSERT [finance].[Providers] ([Guid], [ImageUrl], [Id], [Name], [Flags], [TenantID], [DateCreated], [DateLastModified], [CreatedByName], [CreatedByID], [LastModifiedBy], [LastModifiedByName], [Version], [IsDeleted], [ExternalID], [TagString]) VALUES (N'1979b4eb-e677-47f6-bd63-553ef867fdcf', N'http://www.californiabuildingstructures.com/wp-content/uploads/Mosaic.png', 4, N'Mosaic', NULL, 0, CAST(N'2017-02-06 23:11:12.557' AS DateTime), NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
GO
INSERT [finance].[Providers] ([Guid], [ImageUrl], [Id], [Name], [Flags], [TenantID], [DateCreated], [DateLastModified], [CreatedByName], [CreatedByID], [LastModifiedBy], [LastModifiedByName], [Version], [IsDeleted], [ExternalID], [TagString]) VALUES (N'144ca3a0-b5e0-448f-8ae4-55f57bdf51dd', N'https://pbs.twimg.com/profile_images/639160854210965504/IiBkiU2k.png', 5, N'PRMI', NULL, 0, CAST(N'2017-02-06 23:11:50.290' AS DateTime), NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, NULL)
GO
SET IDENTITY_INSERT [finance].[Providers] OFF
GO


insert into finance.planDefinitions ( Guid,ProviderID,Type,IsDisabled,Name,TenantID,DateCreated,Version,IsDeleted)


select NewID() as Guid,(select top 1 guid from finance.providers where name='Spruce Financial')  as ProviderID,2 as Type, 0 as IsDisabled,
'Spruce 10 / 3.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='Spruce Financial')  as ProviderID,2 as Type, 0 as IsDisabled,
'Spruce 20 / 4.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='Mosaic')  as ProviderID,2 as Type, 0 as IsDisabled,
'Mosaic 10 / 3.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='Mosaic')  as ProviderID,2 as Type, 0 as IsDisabled,
'Mosaic 15 / 4.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='Mosaic')  as ProviderID,2 as Type, 0 as IsDisabled,
'Mosaic 20 / 5.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted


union all

select NewID(),(select top 1 guid from finance.providers where name='GreenSky')  as ProviderID,2 as Type, 0 as IsDisabled,
'GreenSky 5 / 0.00%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='GreenSky')  as ProviderID,2 as Type, 0 as IsDisabled,
'GreenSky 12 / 2.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='GreenSky')  as ProviderID,2 as Type, 0 as IsDisabled,
'GreenSky 20 / 5.99%' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted


union all

select NewID(),(select top 1 guid from finance.providers where name='PRMI')  as ProviderID,3 as Type, 0 as IsDisabled,
'PRMI 5 Year Mortgage' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted


union all
select NewID(),(select top 1 guid from finance.providers where name='PRMI')  as ProviderID,3 as Type, 0 as IsDisabled,
'PRMI 10 Year Mortgage' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted


union all
select NewID(),(select top 1 guid from finance.providers where name='PRMI')  as ProviderID,3 as Type, 0 as IsDisabled,
'PRMI 15 Year Mortgage' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='PRMI')  as ProviderID,3 as Type, 0 as IsDisabled,
'PRMI 20 Year Mortgage' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='PRMI')  as ProviderID,3 as Type, 0 as IsDisabled,
'PRMI 25 Year Mortgage' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted

union all

select NewID(),(select top 1 guid from finance.providers where name='PRMI')  as ProviderID,3 as Type, 0 as IsDisabled,
'PRMI 30 Year Mortgage' as Name,0 as TenantID,GetUTCDate() as DAteCreated,1 as Version,0 as IsDeleted






Insert into finance.FinanceDetails (Guid,FinancePlanDefinitionID,Apr,[Order],PrincipalIsPaid,ApplyPrincipalReductionAfter,Months,Name,TenantID,DateCreated,Version,IsDeleted)


select NewID() as Guid, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 10 / 3.99%') as FinancialPlanDefintionID,3.99 as Apr,0 as [Order],0 as PrincipalIsPaid, 1 as ApplyPrincipalReductionAfter,16 as Months,'Introductory Period' as Name,0 as TeanntID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted
union all
select NewID() as Guid, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 10 / 3.99%') as FinancialPlanDefintionID,3.99 as Apr,1 as [Order],1 as PrincipalIsPaid, 0 as ApplyPrincipalReductionAfter,104 as Months,'Principal and Interest' as Name,0 as TeanntID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted
union all
select NewID() as Guid, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 20 / 4.99%') as FinancialPlanDefintionID,4.99 as Apr,0 as [Order],0 as PrincipalIsPaid, 1 as ApplyPrincipalReductionAfter,16 as Months,'Introductory Period' as Name,0 as TeanntID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted
union all
select NewID() as Guid, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 20 / 4.99%') as FinancialPlanDefintionID,4.99 as Apr,1 as [Order],1 as PrincipalIsPaid, 0 as ApplyPrincipalReductionAfter,224 as Months,'Principal and Interest' as Name,0 as TeanntID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted


