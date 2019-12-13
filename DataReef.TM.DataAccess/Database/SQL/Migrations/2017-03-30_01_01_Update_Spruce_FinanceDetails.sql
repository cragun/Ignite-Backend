
CREATE PROC #tempInsertFinancePlanDetails
	@PlanName NVARCHAR(250),
	@IntroTerm INT,
	@Apr FLOAT,
	@ReductionType INT = 1,
	@CashLike BIT = 0
AS
BEGIN

	DECLARE @PlanDefinitionID UNIQUEIDENTIFIER
	DECLARE @Order INT
	DECLARE @TermInYears INT

	SELECT @PlanDefinitionID = Guid, @TermInYears = TermInYears
	FROM finance.PlanDefinitions
	WHERE Name = @PlanName

	SET @Order = 0

	IF (@PlanDefinitionID IS NULL)
	BEGIN
		RETURN 1
	END 

	/*Special case for GS 5 / 0.00% */
	IF (@CashLike = 1)
	BEGIN
		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @PlanDefinitionID, @Apr, 0 /*Order*/, 0, 0, 18 /*Months*/, 'Introductory Period w/ Incentives', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 0 /*PrincipalType*/, 0 /*InterestType*/, 1)

		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @PlanDefinitionID, @Apr, 1 /*Order*/, 0, 0, 41 /*Months*/, 'Introductory Period', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 0 /*PrincipalType*/, 0 /*InterestType*/, 0)

		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @PlanDefinitionID, @Apr, 2 /*Order*/, 0, 0, 1 /*Months*/, 'Main Loan Period', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 1 /*PrincipalType*/, 0 /*InterestType*/, 0)

		RETURN 0
	END

	IF (@IntroTerm > 0)
	BEGIN
		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @PlanDefinitionID, @Apr, @Order, 0, 0, @IntroTerm, 'Introductory Period', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 0 /*PrincipalType*/, 1, @ReductionType)
		SET @Order = @Order + 1
	END
	ELSE
	BEGIN
		SET @IntroTerm = 18
		IF (@ReductionType <> 2)
		BEGIN
			INSERT INTO finance.FinanceDetails
			(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
			VALUES
			(NEWID(), @PlanDefinitionID, @Apr, @Order, 0, 0, @IntroTerm, 'Introductory Period', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 1 /*PrincipalType*/, 1 /*InterestType*/, @ReductionType)
			SET @Order = @Order + 1		
		END
		ELSE
		BEGIN
			INSERT INTO finance.FinanceDetails
			(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
			VALUES
			(NEWID(), @PlanDefinitionID, @Apr, @Order, 0, 0, @IntroTerm, 'Introductory Period', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 0 /*PrincipalType*/, 0 /*InterestType*/, @ReductionType)
			SET @Order = @Order + 1		
		END
		
	END


	IF (@ReductionType <> 2)
	BEGIN
		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantId, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @PlanDefinitionID, @Apr, @Order, 0, 0, (@TermInYears * 12) - @IntroTerm, 'Principal and Interest', 0, GETUTCDATE(), 0, 0, 0, 0 /*IsSpruced*/, 1 /*PrincipalType*/, 1 /*InterestType*/, 0)
	END

END
GO

BEGIN TRAN
BEGIN TRY

	UPDATE finance.PlanDefinitions
	SET Name = REPLACE(Name, 'GreenSky', 'GS')

	UPDATE finance.PlanDefinitions
	SET Name = 'GS 20 / 5.99%'
	WHERE Name = 'GS 20/5.99%'

	DELETE FROM finance.financeDetails
	where FinancePlanDefinitionID in
	(
		select Guid
		from finance.plandefinitions
		where name IN ('GS 5 / 0.00%', 'GS 20/5.99%', 'GS 20 / 5.99%', 'GS 12 / 2.99%', 'Mosaic 10 / 3.99%', 'Mosaic 15 / 4.99%', 'Mosaic 20 / 5.99%')
	)

	EXEC #tempInsertFinancePlanDetails 'Mosaic 10 / 3.99%', 18, 3.99
	EXEC #tempInsertFinancePlanDetails 'Mosaic 15 / 4.99%', 18, 4.99
	EXEC #tempInsertFinancePlanDetails 'Mosaic 20 / 5.99%', 18, 5.99

	EXEC #tempInsertFinancePlanDetails 'GS 12 / 2.99%', 0, 2.99
	EXEC #tempInsertFinancePlanDetails 'GS 20 / 5.99%', 18, 5.99
	EXEC #tempInsertFinancePlanDetails 'GS 5 / 0.00%', 0, 0, 2, 1


	COMMIT TRAN
END TRY
BEGIN CATCH

	ROLLBACK TRAN	

END CATCH

DROP PROC #tempInsertFinancePlanDetails

