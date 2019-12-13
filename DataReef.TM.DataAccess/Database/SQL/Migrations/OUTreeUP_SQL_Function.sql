
CREATE FUNCTION [dbo].[OUTreeUP]
(
@OUID Varchar(50)
)
returns Table

as

RETURN

WITH OUTree ([Guid], [ParentId])
as
(
	SELECT  OUs.Guid, OUs.ParentID
	FROM OUs 
	WHERE OUs.Guid = @OUID 
		  AND IsDeleted=0

	UNION ALL
	
	SELECT OUs.Guid, OUs.ParentId
	FROM OUs
	INNER JOIN OUTree AS T 
		ON T.ParentId = OUs.Guid
	WHERE OUs.IsDeleted = 0
)

SELECT [Guid] 
FROM OUTree

