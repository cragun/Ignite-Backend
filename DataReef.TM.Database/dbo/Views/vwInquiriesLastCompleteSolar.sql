







CREATE view [dbo].[vwInquiriesLastCompleteSolar]
as

Select ous.Name as OrganizationName,p.FirstName + ' ' + P.LastName as SalesPerson,pr.Name,pr.Latitude,pr.Longitude,pr.Address1,pr.City,pr.State,pr.ZipCode,case when len(Upper(IsNull(pr.StreetName,'Unknown')))=0 then 'Unknown' else Upper(IsNull(pr.StreetName,'Unknown')) end  as StreetName,pr.HouseNumber,
p.Guid as PersonID,pr.Guid as PropertyID,i.Guid as InquiryID,
t.Name as TerritoryName,t.Guid as TerritoryID,
DatePart(Year,i.DateCreated) as Year,DatePart(Month,i.DateCreated) as Month,DatePart(Week,i.DateCreated) as Week,
DateName(Month,i.DateCreated) as MonthName,DatePart(d,i.DateCreated) as Day,Convert(Date,I.DateCreated) as Date,
DateName(dw,i.DateCreated) as DayOfWeekName,DatePart(hour,i.DateCreated) as Hour,
i.DateCreated as DateTime,  City + State + ZipCode as GeographyKey,

case i.Status 
when 0 then 'Not Contacted'
when 1 then 'Not Home'
when 2 then 'Call Back'
when 3 then 'No Sale'
when 100 then 'Proposal'
when 110 then 'Sale'
else 'Not Home'
end as Status, 
REPLACE ( REPLACE ( i.Notes , CHAR(13) , '' ) , CHAR(10) , ', ' )  as Notes,
ous.Guid as OUID,
Level1,Level2,Level3,Level4,Level5,Level6,Level7,Level8,Level9
from people p
inner join vwLastInquiries i on p.guid=i.personid
inner join properties pr on i.propertyid=pr.guid
inner join Territories t on pr.territoryid=t.guid
inner join ous on t.OUID=ous.guid
inner join (select * from FlatHierarchy('41F31979-213A-4A90-8893-ABE624A14CBB')) fh on  t.OUID=fh.guid
where t.ouid in (select Guid from dbo.OUTree('41F31979-213A-4A90-8893-ABE624A14CBB'))
--and ltrim(rtrim(p.lastname))!='Avenu 2'










GO
GRANT SELECT
    ON OBJECT::[dbo].[vwInquiriesLastCompleteSolar] TO [reportingadmin]
    AS [dbo];


GO
GRANT SELECT
    ON OBJECT::[dbo].[vwInquiriesLastCompleteSolar] TO [completesolar]
    AS [dbo];

