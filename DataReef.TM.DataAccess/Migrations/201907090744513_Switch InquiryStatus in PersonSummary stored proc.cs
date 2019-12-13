namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SwitchInquiryStatusinPersonSummarystoredproc : DbMigration
    {
        public override void Up()
        {
            string query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[proc_PersonAnalytics]
(
@PersonID UniqueIdentifier
)

as

--exec [proc_PersonAnalytics] @PersonID='C192F683-08B5-4429-8C05-4F8CF04CF6D7'

DECLARE @ActiveTerritoryCount Int
DECLARE @LastInquiryDate DateTime

DECLARE @SalesToday INT
DECLARE @SalesThisWeek INT
DECLARE @SalesThisMonth INT
DECLARE @SalesThisYear INT
DECLARE @SalesAllTime INT


SELECT @ActiveTerritoryCount =COUNT(DISTINCT T.Guid)
FROM Assignments A
INNER JOIN Territories T
ON A.TerritoryID = T.Guid 
INNER JOIN OUs O
ON T.OUID = O.Guid
WHERE A.PersonID = @PersonID
  AND A.IsDeleted = 0 
  AND T.IsDeleted = 0
  AND O.IsDeleted = 0
  AND A.Status = 0

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
@SalesAllTime = COUNT(PersonID),
@LastInquiryDate = Max(DateCreated)
FROM (
	SELECT DISTINCT I.PersonID, I.DateCreated
	FROM Inquiries I
	WHERE I.PersonID = @PersonID
		AND I.IsDeleted = 0
		AND I.Disposition = 'Completed'
) AS Data


select 
@ActiveTerritoryCount as ActiveTerritoryCount, 
@SalesThisYear As YTDSales,
@LastInquiryDate as LastInquiryDate,
@SalesToday AS SalesToday,
@SalesThisWeek AS SalesThisWeek,
@SalesThisMonth AS SalesThisMonth,
@SalesThisYear AS SalesThisYear,
@SalesAllTime AS SalesAllTime

GO
";
            Sql(query);
        }
        
        public override void Down()
        {

            var query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[proc_PersonAnalytics]
(
@PersonID UniqueIdentifier
)

as

--exec [proc_PersonAnalytics] @PersonID='C192F683-08B5-4429-8C05-4F8CF04CF6D7'

DECLARE @ActiveTerritoryCount Int
DECLARE @LastInquiryDate DateTime

DECLARE @SalesToday INT
DECLARE @SalesThisWeek INT
DECLARE @SalesThisMonth INT
DECLARE @SalesThisYear INT
DECLARE @SalesAllTime INT


SELECT @ActiveTerritoryCount =COUNT(DISTINCT T.Guid)
FROM Assignments A
INNER JOIN Territories T
ON A.TerritoryID = T.Guid 
INNER JOIN OUs O
ON T.OUID = O.Guid
WHERE A.PersonID = @PersonID
  AND A.IsDeleted = 0 
  AND T.IsDeleted = 0
  AND O.IsDeleted = 0
  AND A.Status = 0

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
@SalesAllTime = COUNT(PersonID),
@LastInquiryDate = Max(DateCreated)
FROM (
	SELECT DISTINCT I.PersonID, I.DateCreated
	FROM Inquiries I
	WHERE I.PersonID = @PersonID
		AND I.IsDeleted = 0
		AND I.Status = 100
) AS Data


select 
@ActiveTerritoryCount as ActiveTerritoryCount, 
@SalesThisYear As YTDSales,
@LastInquiryDate as LastInquiryDate,
@SalesToday AS SalesToday,
@SalesThisWeek AS SalesThisWeek,
@SalesThisMonth AS SalesThisMonth,
@SalesThisYear AS SalesThisYear,
@SalesAllTime AS SalesAllTime

GO
";

            Sql(query);
        }
    }
}
