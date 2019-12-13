


CREATE function [dbo].[FlatHierarchy]
(
@OUID Varchar(50)
)
returns Table

as

RETURN

WITH CTE AS (
SELECT TreePath,Guid, Replace(Name,'&',' and ') as Name
 ,CONVERT(XML,'<Product><Lvl>' + REPLACE([TreePath],'|', '</Lvl><Lvl>') + '</Lvl></Product>')
 AS Prod_Hierarchy
FROM OUTreePath(@OUID) a
)

, LevelBreakdown AS (
SELECT  Guid
 , Name
 , Prod_Hierarchy.value('(Product/Lvl)[1]','varchar(max)') AS [Level1]
 , Prod_Hierarchy.value('(Product/Lvl)[2]','varchar(max)') AS [Level2]
 , Prod_Hierarchy.value('(Product/Lvl)[3]','varchar(max)') AS [Level3]
 , Prod_Hierarchy.value('(Product/Lvl)[4]','varchar(max)') AS [Level4]
 , Prod_Hierarchy.value('(Product/Lvl)[5]','varchar(max)') AS [Level5]
 , Prod_Hierarchy.value('(Product/Lvl)[6]','varchar(max)') AS [Level6]
 , Prod_Hierarchy.value('(Product/Lvl)[7]','varchar(max)') AS [Level7]
 , Prod_Hierarchy.value('(Product/Lvl)[8]','varchar(max)') AS [Level8]
 , Prod_Hierarchy.value('(Product/Lvl)[9]','varchar(max)') AS [Level9]
 FROM cte)

 select * from LevelBreakdown


