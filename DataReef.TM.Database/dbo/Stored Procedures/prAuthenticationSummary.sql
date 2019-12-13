CREATE PROCEDURE prAuthenticationSummary
    @fromDate datetime = null,
    @toDate datetime = null
AS
BEGIN

-- exec prAuthenticationSummary '2017-6-1'

    SET NOCOUNT ON;

    create table #ActiveBillableUsers
       (UserID uniqueidentifier,
        UserName nvarchar(50),
        FirstName nvarchar(100),
        LastName nvarchar(50),
        AuthenticatedDeviceCount int,
        RootOUNames nvarchar(Max),
        LastAuthenticatedDate datetime,
        DeletedDate datetime)

    insert into #ActiveBillableUsers
    select u.Guid as UserID
        ,null as UserName
        ,p.FirstName
        ,p.LastName
        ,0 as AuthenticatedDeviceCount
        ,null as RootOUNames
        ,null as LastAuthenticatedDate
        ,case when (u.IsDeleted = 1 or p.IsDeleted = 1) then p.DateLastModified else null end as DeletedDate
    from Users u
    join People p on p.Guid = u.Guid
    where u.IsNonBillable = 0 and ((u.IsDeleted = 0 and p.IsDeleted = 0) or (u.DateLastModified between @fromDate and @toDate))

    update #ActiveBillableUsers 
        set UserName = ISNULL((select top 1 c.UserName from credentials c where c.UserID = #ActiveBillableUsers.UserID order by c.IsDeleted desc), '')
        ,RootOUNames = ISNULL((select STUFF((SELECT ',' + RootOrganizationName 
        FROM (select distinct RootOrganizationName from OUS where Guid in (select OUID from OUAssociations where PersonId = #ActiveBillableUsers.UserID and IsDeleted = 0) and IsDeleted = 0) as q
        FOR XML PATH('')), 1, 1, '') AS RootOrganizationName), '')

    update #ActiveBillableUsers set AuthenticatedDeviceCount = (select count(distinct a.DeviceID) from Authentications a where a.UserId = #ActiveBillableUsers.UserID and a.DateAuthenticated between @fromDate and @toDate)
    update #ActiveBillableUsers set LastAuthenticatedDate = (select max(a.DateAuthenticated) from Authentications a where a.UserId = #ActiveBillableUsers.UserID and a.DateAuthenticated between @fromDate and @toDate)

    select * from #ActiveBillableUsers

    drop table #ActiveBillableUsers
END
GO