CREATE procedure [dbo].[prRptOUSummary]
(
@StartDate DateTime,
@EndDate DateTime,
@RootOUID UniqueIdentifier
)

as

--exec prRptOUSummary @StartDate ='1/24/2015',  @EndDate = '7/25/2015',  @RootOUID='5891BD95-65DE-4700-8B4B-2FEBEBF32FB5'

select Guid,Name,Level2 into #ou 
from  FlatGuidHierarchy(@RootOUID)

select @StartDate=DateAdd(hour,6,@StartDate)
select @EndDate = DateAdd(HOur,6,@EndDate)

select data.*,
ProposalCount*1.0/PersonCount as ProposalsPerPerson,
ContactCount*1.0/PersonCount as ContactsPerPerson,
ProposalCount*1.0/ContactCount as ProposalRate,
SaleCount*1.0/ContactCount as SaleRate,
CallbackCount*1.0/PersonCount as CallbacksPerPerson,
SaleCount*1.0/PersonCount as SalesPerPerson


from 
(

select ous.Name,OUID,SUm(Contacts) as ContactCount,
Sum(Callbacks) as CallbackCount,
Sum(Proposals) as ProposalCount,
Sum(Sales) as SaleCount,
Sum(People) as PersonCount
from
(

select ISNull(Level2,@RootOUID) as OUID,Contacts,IsNull(Sales,0) as Sales,IsNull(CallBacks,0) as CallBacks,IsNull(Proposals,0)  as Proposals,AvgPersonCount as People
from #ou ou

inner join
(
	select OUID,Count(*) as Contacts
	from Inquiries i 
	inner join #ou ou on OUID = ou.guid
	where i.DateCreated between @StartDate and @EndDate
	and i.status>=2
	group by OUID

) as c on ou.guid=c.OUID


left outer join
(
	select OUID,Count(*) as Callbacks
	from Inquiries i 
	inner join #ou ou on OUID = ou.guid
	where i.DateCreated between @StartDate and @EndDate
	and i.status=2
	group by OUID

) as cb on ou.guid=cb.OUID

left outer join
(
	select OUID,Count(*) as Proposals
	from Inquiries i 
	inner join #ou ou on OUID = ou.guid
	where i.DateCreated between @StartDate and @EndDate
	and i.status=100
	group by OUID

) as pp on ou.guid=pp.OUID

left outer join
(
	select OUID,Count(*) as Sales
	from Inquiries i 
	inner join #ou ou on OUID = ou.guid
	where i.DateCreated between @StartDate and @EndDate
	and i.status=110
	group by OUID

) as sl on ou.guid=sl.OUID


	inner join
	(
		select OUID,Avg(PersonCount) as AvgPersonCount
		from
		(
		select Date,OUID,Count(*)*1.0 as PersonCount 
		from
		(

		select distinct Convert(Date,DAteADd(Hour,-6,i.DateCreated)) as Date,OUID,PersonID 
		from inquiries i
		inner join #ou ou on OUID = ou.guid
		where i.DateCreated between @Startdate and @EndDate

		) as PeopleData group by DAte,OUID
		) as PeopleDataAvg
		group by OUID

	) as P on ou.Guid = P.OUID
)
as data
inner join ous on data.ouid=ous.guid
group by OUID,ous.Name

)

as data

drop table #ou