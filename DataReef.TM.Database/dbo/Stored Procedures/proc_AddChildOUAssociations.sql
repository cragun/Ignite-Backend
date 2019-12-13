/*
looks up the associations and role for the person and the given ou and assigns all subous the same
meant to be fired by a trigger when adding an association
*/

CREATE procedure proc_AddChildOUAssociations
(
@PersonID UniqueIdentifier,
@OUID UniqueIdentifier,
@OUroleID UniqueIdentifier
)

as

If @OURoleID is not null and @PersonID is not null and @OUID is not null
begin

	insert into OUAssociations (Guid,PersonID,OUID,OURoleID,Name,TenantID,DateCreated,Version,IsDeleted)
	select NewID() as Guid,@PersonID as PersonID,OUID,@OURoleID as OURoleID,'Inherited',0 as TenantID,GetUTCDate() as DateCreated,1 as Version,0 as IsDeleted
	from
	(
	select guid as OUID from dbo.OUTree(@OUID) where guid!=@OUID
	and Guid not in (select OUID from OUAssociations where PersonID=@PersonID)
	)
	as Data

end
