CREATE procedure [dbo].[proc_OUMediaItems]
(
	@OUID UniqueIdentifier,
	@IncludeDeleted BIT = 0
)

as
  --exec proc_OUMediaItems 'F18CC655-9D29-4320-B573-350F3ADE3962', 1

DECLARE @OUTree TABLE(Guid UNIQUEIDENTIFIER)

INSERT INTO @OUTree
exec proc_OUAndChildrenGuids @OUID = @OUID

SELECT * 
FROM  MediaItems
WHERE Guid IN 
(
	SELECT MediaID 
	FROM OUMediaItems
	WHERE OUID in
	(
	 SELECT [Guid] FROM @OUTree
	) AND 
	(
	(@IncludeDeleted = 1) 
		OR
	 (@IncludeDeleted = 0 AND IsDeleted = 0)
	)
)
AND ((@IncludeDeleted = 1) 
	 OR
	 (@IncludeDeleted = 0 AND IsDeleted = 0)
	 )
