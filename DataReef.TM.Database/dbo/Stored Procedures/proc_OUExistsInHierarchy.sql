create procedure [dbo].[proc_OUExistsInHierarchy]
(
@OUID UniqueIdentifier,
@ContainerOUID UniqueIdentifier
)

as

WITH OUTree ([Guid])
as
(
select  OUs.Guid from OUs where  OUs.Guid=@ContainerOUID

Union All
select OUs.Guid 
from OUs
Inner JOin OUTree as t on OUs.ParentID=t.Guid
)

select count(*) as [Result] from OUtree where Guid=@OUID

