namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorSPforOUAnalytics : DbMigration
    {
        public override void Up()
        {
            #region OUAnalytics
            //update OU Analytics
            var query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[proc_OUAnalytics]
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
	SELECT  DISTINCT I.PersonID, I.DateCreated, I.Disposition
	FROM Inquiries I
	INNER JOIN properties p on i.propertyID = p.Guid
	INNER JOIN Territories t on p.TerritoryID = T.Guid
	WHERE T.OUID IN (SELECT Guid FROM @OUTree)
		AND T.IsDeleted = 0
		AND I.IsDeleted = 0
		AND P.IsDeleted = 0
		AND I.Disposition = 'Completed'
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

GO
";

            Sql(query);
            #endregion

            //update TerritoryAnalytics
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
	  AND I.Disposition != 'NotHome' AND I.Disposition != 'CallBack'
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
	  AND I.Disposition = 'Completed'
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
	AND I.Disposition IN ('NotHome', 'CallBack')

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

GO
";
            Sql(query);

            //update PropertiyGuidsByOUAndStatusPaged
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[proc_PropertiyGuidsByOUAndStatusPaged]
(
@OUID UniqueIdentifier,
@Disposition nvarchar(50),
@PageIndex int=0,
@ItemsPerPage int=0
)

as

-- exec proc_PropertiyGuidsByOUAndStatusPaged 'd933a7c3-67cf-4955-97f3-70595198bc8d', 90, 1, 20

if @PageIndex IS NULL SELECT @PageIndex = 0
if @ItemsPerPage IS NULL SELECT @ItemsPerPage = 20


SELECT P.Guid 
FROM
(
	SELECT DISTINCT P.Guid, P.Name
	FROM Properties P with (nolock)
	INNER JOIN Territories T 
		ON p.TerritoryID = T.Guid
	JOIN Inquiries I 
		ON P.Guid = I.PropertyID
	LEFT OUTER JOIN Inquiries I2 
		ON (P.Guid = I2.PropertyID AND I.Id < I2.Id)
	WHERE i.Disposition = @Disposition
	and T.OUID in (select Guid from dbo.OUTree(@OUID))
) P
ORDER BY P.Name

OFFSET (@PageIndex * @ItemsPerPage) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

GO";
            Sql(query);

            //update prPropertiesByOUAndStatusPaged

            //first update the view in order to contain the Disposition column
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER view [dbo].[vwLastInquiries]
as


select i.* from Inquiries i
inner join 
(

select PropertyID,Max(ID) as ID
from Inquiries
group by propertyid
)
m on i.id=m.id
GO
";
            Sql(query);
            //update the stored procedure
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[prPropertiesByOUAndStatusPaged]
(
@OUID UniqueIdentifier,
@Disposition nvarchar(50),
@PageIndex int=0,
@ItemsPerPage int=0
)

as


if @PageIndex IS NULL SELECT @PageIndex = 0
if @ItemsPerPage IS NULL SELECT @ItemsPerPage = 20


SELECT P.*
FROM Properties P with (nolock)
inner join Territories T on p.TerritoryID =T.Guid
inner join vwLastInquiries li on p.guid=li.propertyid
WHERE li.Disposition=@Disposition
and t.ouid in (select Guid from dbo.OUTree(@OUID))

ORDER BY p.Name
OFFSET (@PageIndex * @ItemsPerPage) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY


GO
";
            Sql(query);

            //update prRptInquirySummary
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[prRptInquirySummary]
(
@StartDate DateTime,
@EndDate DAteTime,
@RootOUID Varchar(50)
)

as

begin



select guid into #ou from dbo.OUTree(@RootOUID)  

--exec prRptInquirySummary @StartDate='3/1/2015', @EndDate = '5/1/2015', @RootOUID='41F31979-213A-4A90-8893-ABE624A14CBB'

 


Select OU.Name as OUName, OU.Guid as OUID,P.LastName + ', ' + P.FirstName as PersonName,p.Guid as PersonID,InquiryCount,IsNull(ContactCount,0) as ContactCount,
IsNull(SaleCount,0) as SaleCount,
IsNull(SaleCount,0)/(InquiryCount *1.0) as SaleRate,
IsNull(ContactCount,0)/(InquiryCount *1.0) as ContactRate,
case when IsNull(ContactCount,0)=0 then 0.0 else Round( IsNull(SaleCount,0)/(IsNull(ContactCount,0) *1.0),2,1) end as CloseRate


from people p

inner join 
(
 select OUID, PersonId,Count(*) as InquiryCount
from
(
select distinct personid, propertyid,t.OUId
from inquiries i
inner join properties p on i.propertyid=p.guid
inner join territories t on p.territoryid=t.guid
where i.datecreated between @StartDate and @EndDAte
and t.ouid in (select guid from #ou)
)
as DAta group by PersonID,OUID
)
as doors on p.guid=doors.personid

left outer join
(
select PersonId,Count(*) as ContactCount
from
(
select
distinct personid, propertyid
from inquiries i
inner join properties p on i.propertyid=p.guid
inner join territories t on p.territoryid=t.guid
where i.datecreated between @StartDate and @EndDAte
and i.Disposition != 'NotHome' AND I.Disposition != 'CallBack'
and t.ouid in (select guid from #ou)
)
as DAta group by PersonID
)
as Contacts on p.guid = contacts.personid


left outer join
(
select PersonId,Count(*) as SaleCount
from
(
select
distinct personid, propertyid
from inquiries i
inner join properties p on i.propertyid=p.guid
inner join territories t on p.territoryid=t.guid
where i.datecreated between @StartDate and @EndDAte
and i.Disposition= 'Completed'
and t.ouid in (select guid from #ou)
)
as DAta group by PersonID
)
as Sales on p.guid = sales.personid

inner join OUS ou on doors.ouid=ou.guid

order by OUName,PersonName

drop table #ou

end

GO
";
            Sql(query);

            //update prRptPropertiesByPersonDateOUStatus
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[prRptPropertiesByPersonDateOUStatus]
(
@PersonID Varchar(50),
@InquiryDisposition nvarchar(50),
@StartDate DateTime,
@EndDate DAteTime,
@RootOUID Varchar(50)
)

as

begin



--select guid into #ou from dbo.OUTree(@RootOUID)  

--exec prRptPropertiesByPersonDateOUStatus @PersonID='CFAA69E8-CA94-4558-8BBB-D2F9ABD3C017', @InquiryStatus=1, @StartDate='3/1/2015', @EndDate = '5/1/2015', @RootOUID='41F31979-213A-4A90-8893-ABE624A14CBB'

select distinct P.Guid as Guid
from Inquiries I
inner join properties p on i.propertyid=p.guid
inner join territories t on p.TerritoryID=t.Guid
where i.PersonID=@PersonID
and I.Disposition=@InquiryDisposition
and Convert(Date,I.DateCreated) between @StartDate and @EndDate
and t.ouid in (select guid from dbo.OUTree(@RootOUID))


--drop table #ou


end

GO
";
            Sql(query);
        }

        public override void Down()
        {
            //update OUAnalytics
            var query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[proc_OUAnalytics]
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

GO
";
            Sql(query);

            //update TerritoryAnalytics
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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

GO
";
            Sql(query);

            //update PropertiyGuidsByOUAndStatusPaged
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[proc_PropertiyGuidsByOUAndStatusPaged]
(
@OUID UniqueIdentifier,
@Status int,
@PageIndex int=0,
@ItemsPerPage int=0
)

as

-- exec proc_PropertiyGuidsByOUAndStatusPaged 'd933a7c3-67cf-4955-97f3-70595198bc8d', 90, 1, 20

if @PageIndex IS NULL SELECT @PageIndex = 0
if @ItemsPerPage IS NULL SELECT @ItemsPerPage = 20


SELECT P.Guid 
FROM
(
	SELECT DISTINCT P.Guid, P.Name
	FROM Properties P with (nolock)
	INNER JOIN Territories T 
		ON p.TerritoryID = T.Guid
	JOIN Inquiries I 
		ON P.Guid = I.PropertyID
	LEFT OUTER JOIN Inquiries I2 
		ON (P.Guid = I2.PropertyID AND I.Id < I2.Id)
	WHERE i.status = @Status
	and T.OUID in (select Guid from dbo.OUTree(@OUID))
) P
ORDER BY P.Name

OFFSET (@PageIndex * @ItemsPerPage) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

GO
";
            Sql(query);

            //update prPropertiesByOUAndStatusPaged
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[prPropertiesByOUAndStatusPaged]
(
@OUID UniqueIdentifier,
@Status int,
@PageIndex int=0,
@ItemsPerPage int=0
)

as


if @PageIndex IS NULL SELECT @PageIndex = 0
if @ItemsPerPage IS NULL SELECT @ItemsPerPage = 20


SELECT P.*
FROM Properties P with (nolock)
inner join Territories T on p.TerritoryID =T.Guid
inner join vwLastInquiries li on p.guid=li.propertyid
WHERE li.status=@Status
and t.ouid in (select Guid from dbo.OUTree(@OUID))

ORDER BY p.Name
OFFSET (@PageIndex * @ItemsPerPage) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY


GO
";
            Sql(query);

            //update prRptInquirySummary
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[prRptInquirySummary]
(
@StartDate DateTime,
@EndDate DAteTime,
@RootOUID Varchar(50)
)

as

begin



select guid into #ou from dbo.OUTree(@RootOUID)  

--exec prRptInquirySummary @StartDate='3/1/2015', @EndDate = '5/1/2015', @RootOUID='41F31979-213A-4A90-8893-ABE624A14CBB'

 


Select OU.Name as OUName, OU.Guid as OUID,P.LastName + ', ' + P.FirstName as PersonName,p.Guid as PersonID,InquiryCount,IsNull(ContactCount,0) as ContactCount,
IsNull(SaleCount,0) as SaleCount,
IsNull(SaleCount,0)/(InquiryCount *1.0) as SaleRate,
IsNull(ContactCount,0)/(InquiryCount *1.0) as ContactRate,
case when IsNull(ContactCount,0)=0 then 0.0 else Round( IsNull(SaleCount,0)/(IsNull(ContactCount,0) *1.0),2,1) end as CloseRate


from people p

inner join 
(
 select OUID, PersonId,Count(*) as InquiryCount
from
(
select distinct personid, propertyid,t.OUId
from inquiries i
inner join properties p on i.propertyid=p.guid
inner join territories t on p.territoryid=t.guid
where i.datecreated between @StartDate and @EndDAte
and t.ouid in (select guid from #ou)
)
as DAta group by PersonID,OUID
)
as doors on p.guid=doors.personid

left outer join
(
select PersonId,Count(*) as ContactCount
from
(
select
distinct personid, propertyid
from inquiries i
inner join properties p on i.propertyid=p.guid
inner join territories t on p.territoryid=t.guid
where i.datecreated between @StartDate and @EndDAte
and i.status>=2
and t.ouid in (select guid from #ou)
)
as DAta group by PersonID
)
as Contacts on p.guid = contacts.personid


left outer join
(
select PersonId,Count(*) as SaleCount
from
(
select
distinct personid, propertyid
from inquiries i
inner join properties p on i.propertyid=p.guid
inner join territories t on p.territoryid=t.guid
where i.datecreated between @StartDate and @EndDAte
and i.status=100
and t.ouid in (select guid from #ou)
)
as DAta group by PersonID
)
as Sales on p.guid = sales.personid

inner join OUS ou on doors.ouid=ou.guid

order by OUName,PersonName

drop table #ou

end

GO
";
            Sql(query);

            //update prRptPropertiesByPersonDateOUStatus
            query = @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[prRptPropertiesByPersonDateOUStatus]
(
@PersonID Varchar(50),
@InquiryStatus int,
@StartDate DateTime,
@EndDate DAteTime,
@RootOUID Varchar(50)
)

as

begin



--select guid into #ou from dbo.OUTree(@RootOUID)  

--exec prRptPropertiesByPersonDateOUStatus @PersonID='CFAA69E8-CA94-4558-8BBB-D2F9ABD3C017', @InquiryStatus=1, @StartDate='3/1/2015', @EndDate = '5/1/2015', @RootOUID='41F31979-213A-4A90-8893-ABE624A14CBB'

select distinct P.Guid as Guid
from Inquiries I
inner join properties p on i.propertyid=p.guid
inner join territories t on p.TerritoryID=t.Guid
where i.PersonID=@PersonID
and I.Status=@InquirySTatus
and Convert(Date,I.DateCreated) between @StartDate and @EndDate
and t.ouid in (select guid from dbo.OUTree(@RootOUID))


--drop table #ou


end

GO
";
            Sql(query);
        }
    }
}
