
		IF EXISTS (SELECT * FROM sysobjects WHERE name='proc_GetOUTreeTerritories') BEGIN
			DROP PROCEDURE [dbo].[proc_GetOUTreeTerritories];
		END
		GO
		CREATE PROCEDURE [dbo].[proc_GetOUTreeTerritories]
		(
			@OUID UniqueIdentifier,
			@TerritoryName Nvarchar(100),
			@IncludeDeleted BIT = 0
		)
		AS
		BEGIN

			DECLARE @OUTree TABLE(Guid UNIQUEIDENTIFIER)

			INSERT INTO @OUTree
			exec proc_OUAndChildrenGuids @OUID = @OUID

			SELECT * FROM [dbo].[Territories] T
			WHERE T.Name LIKE '%' + @TerritoryName + '%' AND
			EXISTS (SELECT 1 FROM   @OUTree Tree  WHERE  Tree.Guid = T.OUID) AND 
			(@IncludeDeleted = 1 OR (@IncludeDeleted = 0 AND T.IsDeleted = 0))

		END