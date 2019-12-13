CREATE function dbo.OURoot
(
@OUID UniqueIdentifier
)
returns UniqueIdentifier


as

begin

declare @ret UniqueIdentifier


;WITH c 
AS
(
    SELECT guid, ParentId, Name, 1 AS Depth
    FROM OUs
    where guid=@OUID

    UNION ALL

    SELECT t.guid, t.ParentId, t.Name, c.Depth + 1 AS 'Level'
    FROM OUs t  
    INNER JOIN c ON t.guid = c.ParentId
)



select top 1 @Ret= Guid from
(
SELECT guid,
       ParentID, 
       Name,
       MAX(Depth) OVER() - Depth + 1 AS InverseDepth
FROM c
) as data order by inversedepth


return @ret

end