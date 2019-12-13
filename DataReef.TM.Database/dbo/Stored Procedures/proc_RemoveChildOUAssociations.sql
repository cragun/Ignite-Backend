/*
deletes ( vs setting isdeleted=1, which would cause cascading triggers ) the associations from the subous
*/

create procedure proc_RemoveChildOUAssociations
(
@PersonID UniqueIdentifier,
@OUID UniqueIdentifier,
@OUroleID UniqueIdentifier
)

as

If @OURoleID is not null and @PersonID is not null and @OUID is not null
begin

	Delete from OUAssociations where PersonID=@PersonID and OURoleID=@OURoleID
	and OUID in (select guid from dbo.outree(@OUID))

end
