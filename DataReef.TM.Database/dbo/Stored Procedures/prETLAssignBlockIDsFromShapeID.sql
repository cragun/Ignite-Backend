create procedure prETLAssignBlockIDsFromShapeID
(
@ShapeID Varchar(50)
)

as

declare @State Varchar(2)


declare @ShapeText varchar(max)
select top 1 @ShapeText = Shape.ToString(), @State=State
from geospatial..ShapeDefinitions where shapeid=@ShapeID


DECLARE @g geography;
SET @g = geography::STGeomFromText(@ShapeText, 4326).MakeValid()
select @g = case when @g.EnvelopeAngle() >= 90 THEN @g.ReorientObject() else @g end

declare @gm geometry
select @gm = geometry::STGeomFromText(@ShapeText, 4326).MakeValid()

 declare @MinX Float select @Minx=  geometry::EnvelopeAggregate(@gm).STPointN(1).STX  
 declare @MinY Float select @MinY=  geometry::EnvelopeAggregate(@gm).STPointN(1).STY 
 declare @MaxX Float select @MaxX=  geometry::EnvelopeAggregate(@gm).STPointN(3).STX  
 declare @MaxY Float select @MaxY=  geometry::EnvelopeAggregate(@gm).STPointN(3).STY  


select locationid into #l
from geospatial..Locations L with (nolock)
where l.Lon >= @MinX and l.Lon<=@MaxX
and l.Lat >= @MinY and L.Lat <=@MaxY
and @g.STContains([Location])=1


update Geospatial..Locations 
set BlockID=@ShapeID
from Geospatial..Locations l
inner join #l ll on l.locationid=ll.locationid
where state=@State


drop table #l
