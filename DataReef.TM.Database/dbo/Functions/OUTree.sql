
CREATE function [dbo].[OUTree]
(
@OUID Varchar(50)
)
returns Table

as

RETURN

WITH OUTree ([Guid])
as
(
select  OUs.Guid from OUs where  OUs.Guid=@OUID and IsDeleted=0

Union All
select OUs.Guid 
from OUs
Inner JOin OUTree as t on OUs.ParentID=t.Guid
where  IsDeleted=0
)

select guid from OUs where Guid in (select Guid from OUTree)


