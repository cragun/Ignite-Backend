ALTER procedure [dbo].[proc_TerritoryAnalytics]
(
@TerritoryID Varchar(50)
)

as

--exec proc_TerritoryAnalytics 'c92eeb07-7d79-4172-900d-2fa7464270e0'

DECLARE @PropertyCount INT
DECLARE @CompleteCount INT
DECLARE @SALECOUNT INT
DECLARE @PrescreenPassed INT
DECLARE @PositivePrescreenNotContactedCount INT
DECLARE @AssignmentCount INT

SELECT @PropertyCount = Count(*) 
FROM Properties 
WHERE TerritoryID = @TerritoryID 
	AND ExternalID IS NULL
	AND IsDeleted = 0

select @CompleteCount = count(*) 
FROM Properties p 
inner join 
(
select propertyid,max(id) as MaxID
from inquiries
group by propertyid
)
as MaxIDs on p.guid=maxids.propertyid
INNER JOIN inquiries i 
ON maxids.maxid = i.id
WHERE p.TerritoryID = @TerritoryID
	  AND I.Status >= 3
	  AND P.IsDeleted = 0
	  AND I.IsDeleted = 0

SELECT @SaleCount = count(*) 
FROM Properties p 
inner join 
(
select propertyid,max(id) as MaxID
from inquiries
group by propertyid
)
as MaxIDs on p.guid=maxids.propertyid
INNER JOIN inquiries i 
ON maxids.maxid = i.id
WHERE p.TerritoryID = @TerritoryID
	  AND I.Status =100
	  AND P.IsDeleted = 0
	  AND I.IsDeleted = 0

SELECT Guid 
INTO #PropertiesWithPassedPrescreen
FROM Properties p
WHERE p.TerritoryID = @TerritoryID 
AND (select top 1 pa.DisplayType 
	from PropertyAttributes pa 
	where PropertyId = p.Guid 
	  and AttributeKey in ('prescreen', 'prescreen-instant', 'prescreen-instant-Spruce') 
	  and IsDeleted = 0 
	  and ExpirationDate > getutcdate() 
	order by DateCreated desc) like 'star%'


SELECT @PrescreenPassed = count(*) 
FROM #PropertiesWithPassedPrescreen

SELECT @PositivePrescreenNotContactedCount = COUNT(*)
FROM Inquiries I
WHERE I.PropertyID IN (SELECT Guid FROM #PropertiesWithPassedPrescreen)
	AND I.Status < 2

DROP TABLE #PropertiesWithPassedPrescreen

SELECT @AssignmentCount = COUNT(*)
FROM dbo.Assignments
WHERE TerritoryID = @TerritoryID
AND IsDeleted = 0

SELECT 
@PropertyCount AS PropertyCount,
@CompleteCount AS CompletedCount,
@SaleCount AS SaleCount,
@PrescreenPassed AS PrescreenPassCount,
@PositivePrescreenNotContactedCount AS PositivePrescreenNotContactedCount,
@AssignmentCount as AssignmentCount
