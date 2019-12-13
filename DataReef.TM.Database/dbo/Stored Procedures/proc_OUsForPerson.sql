CREATE procedure [dbo].[proc_OUsForPerson]
(
@PersonID UniqueIdentifier
)

as

--exec proc_OUsForPerson @PersonID='07756664-e6cb-46df-bcbf-b207066269f8'


set nocount on

create table #t
(
OUID UniqueIdentifier
)
create table #r
(
OUID UniqueIdentifier
)

insert #t select distinct OUID from OUAssociations where PersonID=@PersonID and IsDeleted=0

declare @OUID UniqueIDentifier

while exists (select *  from #t)
begin


select top 1 @OUID = OUID from #t
insert #r select guid from dbo.OUTree(@OUID)
delete from #t where OUID=@OUID

end


Select *,'' as BatchPrescreenTableName,'' as WellKnownText
from OUS ou inner join #r r on ou.guid=r.OUID

drop table #t
drop table #r