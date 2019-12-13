UPDATE finance.FinanceDetails
SET [Order] = [Order] + 1

UPDATE finance.FinanceDetails
SET IsSpruced = 1
WHERE Name = 'Introductory Period'

Insert into finance.FinanceDetails (Guid,FinancePlanDefinitionID,Apr,[Order],PrincipalIsPaid,ApplyPrincipalReductionAfter,Months,Name,TenantID,DateCreated,Version,IsDeleted, IsSpruced)

select NewID() as Guid, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 10 / 3.99%') as FinancialPlanDefintionID,3.99 as Apr,0 as [Order],0 as PrincipalIsPaid, 0 as ApplyPrincipalReductionAfter,2 as Months,'Deferred Period' as Name,0 as TeanntID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted, 0 as IsSpruced
union all
select NewID() as Guid, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 20 / 4.99%') as FinancialPlanDefintionID,4.99 as Apr,0 as [Order],0 as PrincipalIsPaid, 0 as ApplyPrincipalReductionAfter,2 as Months,'Deferred Period' as Name,0 as TeanntID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted, 0 as IsSpruced


-- Insert finance OU associations
INSERT INTO [finance].[OUAssociation]
(Guid, OUID, FinancePlanDefinitionID, TenantID, DateCreated, Version, IsDeleted)
SELECT NEWID() AS Guid, (select top 1 guid from OUs where Name='DataReef Solar' AND ParentID IS NULL) as OUID, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 10 / 3.99%') as FinancialPlanDefintionID, 0 AS TenantID, GETUTCDATE() AS DateCreated, 0 AS Version, 0 AS IsDeleted
UNION ALL 
SELECT NEWID() AS Guid, (select top 1 guid from OUs where Name='DataReef Solar' AND ParentID IS NULL) as OUID, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 20 / 4.99%') as FinancialPlanDefintionID, 0 AS TenantID, GETUTCDATE() AS DateCreated, 0 AS Version, 0 AS IsDeleted
UNION ALL
SELECT NEWID() AS Guid, (select top 1 guid from OUs where Name='Solcius' AND ParentID IS NULL) as OUID, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 10 / 3.99%') as FinancialPlanDefintionID, 0 AS TenantID, GETUTCDATE() AS DateCreated, 0 AS Version, 0 AS IsDeleted
UNION ALL 
SELECT NEWID() AS Guid, (select top 1 guid from OUs where Name='Solcius' AND ParentID IS NULL) as OUID, (select top 1 guid from finance.PlanDefinitions where Name='Spruce 20 / 4.99%') as FinancialPlanDefintionID, 0 AS TenantID, GETUTCDATE() AS DateCreated, 0 AS Version, 0 AS IsDeleted
