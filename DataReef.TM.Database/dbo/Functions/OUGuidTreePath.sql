

CREATE function [dbo].[OUGuidTreePath]
(
@OUID Varchar(50)
)
returns Table

as

RETURN

WITH RecursiveCTE
As 
( SELECT  p.Name
 , p.Guid
 , CAST(p.Guid AS VARCHAR(MAX)) AS TreePath
 , ParentID  
 FROM OUs p  
 WHERE p.Guid =@OUID and IsDeleted=0
 UNION ALL
 SELECT p.Name
  , p.Guid
  , CAST(H.TreePath + '|'
   + CAST(p.Guid AS VARCHAR(100)) AS VARCHAR(MAX))
   AS TreePath
  , p.ParentID  
 FROM dbo.OUs p  
 INNER JOIN RecursiveCTE H
 ON H.Guid = p.ParentID 
where p.IsDeleted=0  
)  
SELECT Name
 , Guid
 , ParentID 
 , TreePath
FROM RecursiveCTE
