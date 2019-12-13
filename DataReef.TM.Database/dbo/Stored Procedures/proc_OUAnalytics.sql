create procedure [dbo].[proc_OUAnalytics]
(
@OUID UniqueIdentifier
)

as

--exec [proc_OUAnalytics] @OUID='99D8B6CA-F404-429F-AE8B-9D18019FCA40'

DECLARE @OUTree TABLE(Guid UNIQUEIDENTIFIER)
DECLARE @PersonCount Int
DECLARE @TerritoryCount int
DECLARE @SubOUsCount int
DECLARE @SalesToday INT
DECLARE @SalesThisWeek INT
DECLARE @SalesThisMonth INT
DECLARE @SalesThisYear INT
DECLARE @SalesAllTime INT


INSERT INTO @OUTree
exec proc_OUAndChildrenGuids @OUID = @OUID

SELECT @PersonCount = Count(DISTINCT OUA.PersonID)
FROM OUAssociations OUA
INNER JOIN People P
ON OUA.PersonID = P.Guid
WHERE OUID IN (SELECT Guid FROM @OUTree)
	 AND OUA.isdeleted = 0
	 AND P.IsDeleted = 0

SELECT @TerritoryCount = count(DISTINCT T.Guid) 
FROM Territories T
WHERE OUID IN (SELECT Guid FROM @OUTree)
	AND IsDeleted = 0

SELECT @SubOUsCount = COUNT(DISTINCT O.Guid)
FROM OUs O
WHERE O.ParentID = @OUID
	AND O.IsDeleted = 0

SELECT 
@SalesToday =  COUNT (CASE 
			WHEN Convert(Date, DateCreated) = Convert(Date, GETUTCDATE()) 
			THEN 
				PersonId 
			ELSE 
				NULL 
		END),
@SalesThisWeek = COUNT (CASE 
			WHEN 
					Convert(Date, DateCreated) >= DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), -1) -- Sunday
				AND
					Convert(Date, DateCreated) <= DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), 5) -- Saturday
			THEN 
				PersonId 
			ELSE 
				NULL 
		END),
@SalesThisMonth = COUNT (CASE
			WHEN 
					MONTH(DateCreated) = MONTH(GETDATE()) 
				AND
					YEAR(DateCreated) = YEAR(GETDATE()) 
			THEN 
				PersonId 
			ELSE 
				NULL 
		END),
@SalesThisYear = COUNT (CASE
			WHEN 
					YEAR(DateCreated) = YEAR(GETDATE()) 
			THEN 
				PersonId 
			ELSE 
				NULL 
		END),
@SalesAllTime = COUNT(PersonID)
FROM (
	SELECT  DISTINCT I.PersonID, I.DateCreated, I.Status
	FROM Inquiries I
	INNER JOIN properties p on i.propertyID = p.Guid
	INNER JOIN Territories t on p.TerritoryID = T.Guid
	WHERE T.OUID IN (SELECT Guid FROM @OUTree)
		AND T.IsDeleted = 0
		AND I.IsDeleted = 0
		AND P.IsDeleted = 0
		AND I.Status = 100
) AS Data

SELECT 
@SalesToday as TodayCount, 
@TerritoryCount As TerritoryCount,
@PersonCount as PersonCount,
@SubOUsCount AS SubOUsCount,
@SalesToday AS SalesToday,
@SalesThisWeek AS SalesThisWeek,
@SalesThisMonth AS SalesThisMonth,
@SalesThisYear AS SalesThisYear,
@SalesAllTime AS SalesAllTime
