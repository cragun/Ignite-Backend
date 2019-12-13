create procedure proc_ProposalsLiteForOu
(
@OUID UniqueIdentifier,
@StartDate DateTime = Null,
@EndDate DateTime = Null,
@Deep bit = 1,
@pageNumber int = 1,
@PageSize int = 1000
)

as

--exec proc_ProposalsLiteForOu @OUID='C5F80842-F4EB-425E-A93D-0571E357D18A'

if (@StartDate is Null) Select @StartDate = '1/1/2010'
if (@EndDate is Null) Select @EndDate = '12/31/2020'


declare @guids table (Guid UniqueIdentifier)


select p.Guid as PropertyID,pp.guid as Id,pp.PersonID as SalesPersonID, NameOfOwner,pp.Address,pp.City,pp.State,pp.ZipCode,pp.Lat,pp.Lon,pp.Name as Name,pp.DateCreated,pp.DateLastModified,
ss.SystemSize,ss.PanelCount,ou.guid as OrganizationID,ou.Name as OrganizationName,ppl.FirstName + ' ' + ppl.LastName as salesPersonName
from Solar.Proposals pp
inner join properties p on pp.PropertyID = p.Guid
inner join Territories t on p.territoryID = t.guid
inner join Solar.Systems ss on pp.Guid=ss.guid
inner join ous ou on t.ouid=ou.guid
inner join people ppl on pp.personid=ppl.guid
where pp.DateLastModified between @StartDAte and @EndDAte
and pp.IsDeleted = 0
and 
(
(@Deep = 0 and t.OUID=@OUID) or (@Deep = 1 and t.OUID in (select Guid from dbo.OUtree(@OUID)))

)

order by pp.DateLastModified desc
OFFSET @PageSize * (@PageNumber - 1) ROWS
FETCH NEXT @PageSize ROWS ONLY

 