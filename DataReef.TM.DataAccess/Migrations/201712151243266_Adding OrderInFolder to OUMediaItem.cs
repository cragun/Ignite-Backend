namespace DataReef.TM.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddingOrderInFoldertoOUMediaItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OUMediaItems", "OrderInFolder", c => c.Int(nullable: false));
            // Update proc_OUMediaItems
            Sql(@"ALTER procedure [dbo].[proc_OUMediaItems]
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
	MI.MediaType,
	OUMI.OrderInFolder

FROM  MediaItems MI

INNER JOIN OUMediaItems OUMI
	ON	MI.Guid = OUMI.MediaID AND 
		OUMI.OUID = @OUID AND 
		(@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND OUMI.IsDeleted = 0))

WHERE (@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND MI.IsDeleted = 0))
	  AND (@MediaType = -1 OR (@MediaType > -1 AND MI.MediaType = @MediaType))");

        }

        public override void Down()
        {
            DropColumn("dbo.OUMediaItems", "OrderInFolder");
        }
    }
}
