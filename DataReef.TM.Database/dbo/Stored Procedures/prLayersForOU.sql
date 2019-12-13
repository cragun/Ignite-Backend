CREATE procedure prLayersForOU
(
@OUID UniqueIdentifier
)

as


--exec prLayersForOU @OUID='2B650E8E-80C8-4E3C-B5A0-7F87BD2C8857'
--exec prLayersForOU 'F3A25F2C-AD03-4C68-B37A-7C105D861B14'

select LayerKey as LayerID, l.Name as LayerName, case VisualizationType when 0 then 'SizedCircles' else 'Shape' end as LayerType,Description,DefaultColor as Color
from Layers l
inner join oulayers oul on l.guid=oul.layerid
where oul.ouid = dbo.OURoot(@OUID)
order by LayerName
 