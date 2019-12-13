create procedure proc_PeopleLiteForOU
(
@OUID UniqueIdentifier,
@Deep bit = 0
)

as


--exec proc_PeopleLiteForOU @OUID='5531F127-6F52-4D67-B5B5-E936CD2C5DDD', @Deep=1


select p.Guid,FirstName,LastName,EmailAddressString as EmailAddress,PhoneNumber, p.Version
from People p
inner join ouAssociations oua on p.guid=oua.PersonID
left outer join
(
select pn.PersonID,Number as PhoneNumber
from PhoneNumbers pn
inner join
(
select personid,max(id) as ID
from PhoneNumbers 
where personid is not null
and isDeleted =0
and isPrimary=1
group by personid
)
as data on pn.id = data.id

)
as phones on p.guid=phones.personID

where ((@Deep = 0 and oua.OUID=@OUID)  or (@Deep = 1 and oua.ouid in (select guid from dbo.OUTree(@OUID))))

and (p.IsDeleted=0 and oua.IsDeleted = 0)


order by lastname,firstname