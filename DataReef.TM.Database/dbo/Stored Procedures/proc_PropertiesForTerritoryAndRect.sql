CREATE procedure proc_PropertiesForTerritoryAndRect
(
@TerritoryID UniqueIdentifier,
@Left Float,
@Right Float,
@Top Float,
@Bottom Float
)

as

--exec proc_PropertiesForTerritoryAndRect 'AC412530-BCC7-41B6-B66C-48DD3096D512',  @Left = -121.774971, @Right = -121.76727, @Bottom=38.661,  @top = 38.662929


select top 1000 Guid from Properties where TerritoryID=@TerritoryID
and Latitude between @Bottom and @Top
and Longitude between @left and @Right
