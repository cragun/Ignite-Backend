CREATE procedure prRptOUSummaryByRep
(
@StartDate DateTime,
@EndDate DateTime,
@RootOUID UniqueIdentifier
)

as


--select @StartDate = '7/1/2015'
--select @EndDate = '7/25/2015'
--select @RootOUID='5891BD95-65DE-4700-8B4B-2FEBEBF32FB5'

--exec prRptOUSummaryByRep @StartDate ='1/24/2015',  @EndDate = '7/25/2015',  @RootOUID='5891BD95-65DE-4700-8B4B-2FEBEBF32FB5'

select Guid,Name,Level2 into #ou 
from  FlatGuidHierarchy(@RootOUID)

select @StartDate=DateAdd(hour,6,@StartDate)
select @EndDate = DateAdd(HOur,6,@EndDate)


select p.FirstName + ' ' + P.LastName as SalesRepName,p.PersonID,
ContactCount,
IsNull(CallbackCount,0) as CallbackCount,
IsNull(ProposalCount,0) as ProposalCount,
IsNull(SaleCount,0) as SaleCount,
IsNull(DayCount,0) as DayCount



from
(
select distinct FirstName,LastName,i.PersonID as PersonID
from People p
inner join inquiries i on p.guid=i.personID
where i.datecreated between @startdate and @enddate
and i.ouid in (select guid from #ou)
) as P

inner join
(
	select PersonID,Count(*) as ContactCount
	from Inquiries i 
	where i.DateCreated between @StartDate and @EndDate
	and i.status>=2
	group by PersonID

) as c on p.PersonID=c.PersonID

left outer join
(
	select PersonID,Count(*) as CallbackCount
	from Inquiries i 
	where i.DateCreated between @StartDate and @EndDate
	and i.status=2
	group by PersonID

) as cb on p.PersonID=cb.PersonID

left outer join
(
	select PersonID,Count(*) as ProposalCount
	from Inquiries i 
	where i.DateCreated between @StartDate and @EndDate
	and i.status=100
	group by PersonID

) as pp on p.PersonID=pp.PersonID


left outer join
(
	select PersonID,Count(*) as SaleCount
	from Inquiries i 
	where i.DateCreated between @StartDate and @EndDate
	and i.status=110
	group by PersonID

) as sl on p.PersonID=sl.PersonID

left outer join
(
	select PersonID,Count(*) as DayCount
	from
	(
	select distinct PersonID,Convert(Date,DateCreated) as Date
	from Inquiries i 
	where i.DateCreated between @StartDate and @EndDate
	) as Data
	group by PersonID
) as days on p.PersonID=days.PersonID

order by LastName,FirstName


drop table #ou