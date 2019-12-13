create view vwLastInquiries
as


select i.* from Inquiries i
inner join 
(

select PropertyID,Max(ID) as ID
from Inquiries
group by propertyid
)
m on i.id=m.id
GO
GRANT SELECT
    ON OBJECT::[dbo].[vwLastInquiries] TO [reportingadmin]
    AS [dbo];

