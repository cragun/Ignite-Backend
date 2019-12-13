CREATE procedure [dbo].[proc_OUMediaItems2]
(
	@OUID UniqueIdentifier,
	@IncludeDeleted BIT = 0
)

as
  --exec proc_OUMediaItems 'F18CC655-9D29-4320-B573-350F3ADE3962', 1

DECLARE @OUTree TABLE(Guid UNIQUEIDENTIFIER)

INSERT INTO @OUTree
exec proc_OUAndChildrenGuids @OUID = @OUID

SELECT 
	MI.Guid, 
	MI.IsFolder, 
	OUMI2.MediaId as ParentId, 
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
	MI.TagString

FROM  MediaItems MI

INNER JOIN OUMediaItems OUMI
	ON	MI.Guid = OUMI.MediaID AND 
		OUMI.OUID = @OUID AND 
		(@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND OUMI.IsDeleted = 0))

LEFT JOIN OUMediaItems OUMI2
	ON	OUMI.ParentID = OUMI2.Guid AND 
		OUMI2.OUID = @OUID AND 
		(@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND OUMI2.IsDeleted = 0))

WHERE (@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND MI.IsDeleted = 0))
