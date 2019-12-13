CREATE PROCEDURE proc_OUAndChildrenGuids
	@OUID UniqueIdentifier
AS
BEGIN

	--exec [proc_OUAndChildrenGuids] @OUID='99D8B6CA-F404-429F-AE8B-9D18019FCA40'

	WITH OUTree ([Guid])
	AS
	(
		SELECT OUs.Guid 
		FROM OUs 
		WHERE OUs.Guid = @OUID

		UNION ALL

		SELECT OUs.Guid 
		FROM OUs 
		INNER JOIN OUTree AS T
		ON OUs.ParentID = T.Guid
		WHERE OUs.IsDeleted = 0
)

select Guid from OUTree

END
