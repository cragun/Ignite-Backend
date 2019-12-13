create procedure [dbo].[proc_SelectOUHierarchy]
(
@OUID UniqueIdentifier
)

as


WITH OUTree ([Guid])
as
(
select  OUs.Guid from OUs where  OUs.Guid=@OUID

Union All
select OUs.Guid 
from OUs
Inner JOin OUTree as t on OUs.ParentID=t.Guid
)

select * from OUs where Guid in (select Guid from OUTree)

