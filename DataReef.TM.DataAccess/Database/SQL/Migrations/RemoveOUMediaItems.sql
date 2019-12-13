

		/* UPDATE EXISTING MEDIA ITEMS WITH INFORMATION FROM CORRESPONDING OUMediaItem */
		UPDATE MI
		SET 
			MI.ParentID				= OUMIParent.MediaID, 
			MI.IsDeleted			= OUMI.IsDeleted, 
			MI.DateLastModified		= OUMI.DateLastModified, 
			MI.LastModifiedBy		= OUMI.LastModifiedBy,
			MI.LastModifiedByName	= OUMI.LastModifiedByName,
			MI.Version				= OUMI.Version,
			MI.TenantID				= OUMI.TenantID,
			MI.ExternalID			= OUMI.ExternalID,
			MI.TagString			= OUMI.TagString
		FROM [dbo].[MediaItems] MI 
		INNER JOIN [dbo].[OUMediaItems] OUMI 
		ON MI.OUID = OUMI.OUID AND MI.Guid = OUMI.MediaID
		LEFT JOIN [dbo].[OUMediaItems] OUMIParent  
		ON OUMI.ParentId = OUMIParent.Guid  AND MI.Guid = OUMI.MediaID

		/* INSERT MEDIA ITEMS WITH NO CORRESPONDING OUMediaItem */
		
		SET IDENTITY_INSERT [dbo].[MediaItems] ON;
		
		INSERT INTO [dbo].[MediaItems]
			(Guid, IsFolder, ParentID, OUID, MimeType, Url, Size, AuthenticationnToken, Id, Name, Flags, TenantID, DateCreated, DateLastModified, CreatedByName, CreatedByID, LastModifiedBy, LastModifiedByName, Version, IsDeleted, ExternalID, TagString)
			SELECT NEWID(), OriginalMI.IsFolder, OriginalMI.ParentID, OUMI.OUID, OriginalMI.MimeType, OriginalMI.Url, OriginalMI.Size, 
			OriginalMI.AuthenticationnToken, OUMI.Id, OriginalMI.Name, OriginalMI.Flags, OUMI.TenantID, OUMI.DateCreated, 
			OUMI.DateLastModified, OUMI.CreatedByName, OUMI.CreatedByID, OUMI.LastModifiedBy, OUMI.LastModifiedByName, OUMI.Version,
			OUMI.IsDeleted, OUMI.ExternalID, OUMI.TagString
			FROM [dbo].[OUMediaItems] OUMI 
			LEFT JOIN [dbo].[MediaItems] MI1
			ON OUMI.MediaID = MI1.Guid AND OUMI.OUID = MI1.OUID
			LEFT JOIN [dbo].[MediaItems] OriginalMI
			ON OUMI.MediaID = OriginalMI.Guid
		WHERE MI1.Guid IS NULL
		
		SET IDENTITY_INSERT [dbo].[MediaItems] OFF;


		/* DELETE OBSOLETE STORED PROCEDURES */

		IF EXISTS (SELECT * FROM sysobjects WHERE name='proc_MediaItems') BEGIN
			DROP PROCEDURE [dbo].[proc_MediaItems];
		END

		IF EXISTS (SELECT * FROM sysobjects WHERE name='proc_OUMediaItems') BEGIN
			DROP PROCEDURE [dbo].[proc_OUMediaItems];
		END

		IF EXISTS (SELECT * FROM sysobjects WHERE name='proc_OUMediaItems2') BEGIN
			DROP PROCEDURE [dbo].[proc_OUMediaItems2];
		END

		/* ADD NEW STORED PROCEDURES */

		IF EXISTS (SELECT * FROM sysobjects WHERE name='proc_GetOUTreeMediaItems') BEGIN
			DROP PROCEDURE [dbo].[proc_GetOUTreeMediaItems];
		END
		GO
		CREATE PROCEDURE [dbo].[proc_GetOUTreeMediaItems]
		(
			@OUID UniqueIdentifier,
			@IncludeDeleted BIT = 0
		)
		AS
		BEGIN
			DECLARE @OUTree TABLE(Guid UNIQUEIDENTIFIER)

			INSERT INTO @OUTree
			exec proc_OUAndChildrenGuids @OUID = @OUID


			SELECT * 
			FROM  MediaItems
			WHERE OUID IN
			(
				SELECT Guid FROM @OUTree
			)
			AND ((@IncludeDeleted = 1) 
				 OR
				 (@IncludeDeleted = 0 AND IsDeleted = 0)
				 )
		END