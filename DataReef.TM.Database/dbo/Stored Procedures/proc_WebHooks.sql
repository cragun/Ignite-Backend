create procedure proc_WebHooks
(
@OUID UniqueIdentifier,
@EventFlags bigint
)

as

begin


select * from dbo.WebHooks
where OUID in (select Guid from  dbo.OUTreeUp(@OUID))
and (@EventFlags = 0 or  (EventFlags | @EventFlags) = @EventFlags)

end