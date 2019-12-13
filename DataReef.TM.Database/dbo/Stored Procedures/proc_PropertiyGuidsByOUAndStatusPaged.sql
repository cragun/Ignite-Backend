CREATE procedure [dbo].[proc_PropertiyGuidsByOUAndStatusPaged]
(
@OUID UniqueIdentifier,
@Status int,
@PageIndex int=0,
@ItemsPerPage int=0
)

as


if @PageIndex IS NULL SELECT @PageIndex = 0
if @ItemsPerPage IS NULL SELECT @ItemsPerPage = 20


SELECT P.Guid
FROM Properties P with (nolock)
inner join Territories T on p.TerritoryID =T.Guid
inner join vwLastInquiries li on p.guid=li.propertyid
WHERE li.status=@Status
and t.ouid in (select Guid from dbo.OUTree(@OUID))

ORDER BY p.Name
OFFSET (@PageIndex * @ItemsPerPage) ROWS
FETCH NEXT @ItemsPerPage ROWS ONLY

