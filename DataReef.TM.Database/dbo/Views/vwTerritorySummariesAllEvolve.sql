

CREATE view [dbo].[vwTerritorySummariesAllEvolve]

as



select t.Name as TerritoryName,t.Guid,PropertyCount,t.DateCreated,Status,OU.Name as OUName,
DatePart(Year,t.DateCreated) as Year,DatePart(Month,t.DateCreated) as Month,DatePart(Week,t.DateCreated) as Week,
DateName(Month,t.DateCreated) as MonthName,DatePart(d,t.DateCreated) as Day,Convert(Date,t.DateCreated) as Date,
DateName(dw,t.DateCreated) as DayOfWeekName,
case when prescreendate is null then null else DateDiff(Day,PrescreenDate,GetUTCDAte()) end as TerritoryAge,
IsNull(ContactCount,0) as ContactCount,
IsNull(ProposalCount,0) as ProposalCount,
IsNull(CallbackCount,0) as CallbackCount,
IsNull(PrescreenCount,0) as PreScreenCount,
OU.Guid as OUID,
Level1,Level2,Level3,Level4,Level5,Level6,Level7,Level8,Level9
from Territories t
inner join Ous ou on t.ouid=ou.guid
left outer join
(
select TerritoryID,count(*) as ContactCount from Properties p
inner join vwLastInquiries i on p.guid=i.propertyid
where i.status>=2
group by TerritoryID
)
as Contacted on t.guid = contacted.territoryID

left outer join
(
select TerritoryID,count(*) as ProposalCount from Properties p
inner join vwLastInquiries i on p.guid=i.propertyid
where i.status=100
group by TerritoryID
)
as Proposals on t.guid = Proposals.territoryID

left outer join
(
select TerritoryID,count(*) as CallbackCount from Properties p
inner join vwLastInquiries i on p.guid=i.propertyid
where i.status=2
group by TerritoryID
)
as CallBacks on t.guid = CallBacks.territoryID

left outer join
(
select TerritoryID,Count(*) as PrescreenCount, Max(h.DateCreated) as PrescreenDate
from PrescreenBatches h
inner join prescreendetails d on h.guid = d.batchid
where convert(int,creditCategory)=1
group by TerritoryID
)
as PreScreens on t.guid = PreScreens.territoryID

inner join (select * from FlatHierarchy('5891BD95-65DE-4700-8B4B-2FEBEBF32FB5')) fh on  t.OUID=fh.guid

--when 0 then 'Not Contacted'
--when 1 then 'Not Home'
--when 2 then 'Call Back'
--when 3 then 'No Sale'
--when 100 then 'Proposal'



where t.ouid in (select Guid from dbo.OUTree('5891BD95-65DE-4700-8B4B-2FEBEBF32FB5'))
and t.IsDeleted=0







GO
GRANT SELECT
    ON OBJECT::[dbo].[vwTerritorySummariesAllEvolve] TO [reportingadmin]
    AS [dbo];

