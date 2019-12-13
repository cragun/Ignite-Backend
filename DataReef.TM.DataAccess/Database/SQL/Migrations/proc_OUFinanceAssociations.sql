SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Daniel Horon
-- Create date: 2017-02-10
-- Description:	Retrieves Finance Plan Definitions associated to OU or to the closest ancestor
-- =============================================
CREATE PROCEDURE proc_OUFinanceAssociations
	@OUID UNIQUEIDENTIFIER
AS
BEGIN
	
	
	DECLARE @ASSOCS TABLE(
		Guid UNIQUEIDENTIFIER, 
		Idx INT)

	DECLARE @MIN INT

	INSERT INTO @ASSOCS
	SELECT FinancePlanDefinitionID, OUT.Idx 
	FROM finance.OUAssociation OUA INNER JOIN
	(select Guid, ROW_NUMBER() OVER (ORDER BY (SELECT 1)) AS Idx
	 from OUTreeUP(@OUID)
	) OUT
	ON OUA.OUID = OUT.Guid
	WHERE OUA.IsDeleted = 0

	SELECT @MIN = MIN(Idx) 
	FROM @ASSOCS

	SELECT Guid
	FROM finance.PlanDefinitions
	WHERE Guid IN (
		SELECT Guid
		FROM @ASSOCS
		WHERE Idx = @MIN
		)
	AND IsDisabled = 0
	AND IsDeleted = 0

END
GO
