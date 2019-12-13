CREATE procedure [dbo].[proc_TerritoryAnalytics]
(
@TerritoryID Varchar(50)
)

as

--exec proc_TerritoryAnalytics 'c92eeb07-7d79-4172-900d-2fa7464270e0'

DECLARE @PropertyCount INT
DECLARE @CompleteCount INT
DECLARE @SALECOUNT INT
DECLARE @PositivePrescreenNotContactedCount INT



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


SELECT @PositivePrescreenNotContactedCount = COUNT(PD.Guid)
FROM Properties P
INNER JOIN Inquiries I
ON P.Guid = I.PropertyID
INNER JOIN PrescreenBatches PB
ON PB.TerritoryID = P.TerritoryID
INNER JOIN PrescreenDetails PD
ON PD.BatchID = PB.Guid
WHERE P.TerritoryID = @TerritoryID
	AND I.Status < 2
	AND PD.CreditCategory = 1


SELECT 
@PropertyCount AS PropertyCount,
@CompleteCount AS CompletedCount,
@SaleCount AS SaleCount,
@PositivePrescreenNotContactedCount AS PositivePrescreenNotContactedCount
