UPDATE MediaItems
SET MediaType = 1
WHERE [Guid] IN
(
	SELECT [Guid]
	FROM MediaItems
	WHERE Guid IN
		(
		SELECT Distinct MediaId
		FROM OUMediaItems
		WHERE ParentId IN
			(
			SELECT Distinct [Guid]
			FROM OUMediaItems
			WHERE ParentId IN
				(SELECT [Guid]
				FROM OUMediaItems
				WHERE MediaID IN
					(
					SELECT [Guid]
					FROM MediaItems
					WHERE Name = 'Training Videos'
					AND IsDeleted = 0
					)
					AND IsDeleted = 0
				)
				AND IsDeleted = 0
			)
		)
		UNION

		SELECT Distinct MI.[Guid]
		FROM OUMediaItems OMI
		INNER JOIN MediaItems MI
		ON OMI.MediaID = MI.Guid
		WHERE OMI.ParentId IN
			(SELECT [Guid]
			FROM OUMediaItems
			WHERE MediaID IN
				(
				SELECT [Guid]
				FROM MediaItems
				WHERE Name = 'Training Videos'
				AND IsDeleted = 0
				)
				AND IsDeleted = 0
			)
			AND MI.IsDeleted = 0
			AND OMI.IsDeleted = 0
		
		UNION 
		
		SELECT [Guid]
		FROM MediaItems
		WHERE Name = 'Training Videos'
		AND IsDeleted = 0
)
GO

ALTER procedure [dbo].[proc_OUMediaItems]
(
	@OUID UniqueIdentifier,
	@IncludeDeleted BIT = 0,
	@MediaType INT = -1
)

as
  -- exec proc_OUMediaItems 'F3A25F2C-AD03-4C68-B37A-7C105D861B14', 1, 1

DECLARE @OUTree TABLE(Guid UNIQUEIDENTIFIER)

INSERT INTO @OUTree
exec proc_OUAndChildrenGuids @OUID = @OUID

SELECT 
	OUMI.Guid, 
	MI.IsFolder, 
	OUMI.ParentId, 
	@OUID AS OUID, 
	MI.MimeType,
	MI.Url,
	MI.Size,
	MI.Name,
	MI.DateCreated,
	MI.DateLastModified,
	MI.Version,
	MI.IsDeleted,

	MI.AuthenticationnToken,
	MI.Id,
	MI.Flags,
	MI.TenantID,
	MI.CreatedByName,
	MI.CreatedByID,
	MI.LastModifiedBy,
	MI.LastModifiedByName,
	MI.ExternalID,
	MI.TagString,
	MI.MediaType

FROM  MediaItems MI

INNER JOIN OUMediaItems OUMI
	ON	MI.Guid = OUMI.MediaID AND 
		OUMI.OUID = @OUID AND 
		(@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND OUMI.IsDeleted = 0))

WHERE (@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND MI.IsDeleted = 0))
	  AND (@MediaType = -1 OR (@MediaType > -1 AND MI.MediaType = @MediaType))

GO

