CREATE procedure [dbo].[proc_LayersForOU]
(
@OUID UniqueIdentifier
)

as


--exec [proc_LayersForOU] @OUID='2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'
--exec [proc_LayersForOU] 'F3A25F2C-AD03-4C68-B37A-7C105D861B14'

select l.*
from Layers l
inner join oulayers oul on l.guid=oul.layerid
where oul.ouid = dbo.OURoot(@OUID) 