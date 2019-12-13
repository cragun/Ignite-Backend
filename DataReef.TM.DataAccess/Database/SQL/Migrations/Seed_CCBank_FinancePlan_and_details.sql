
CREATE PROC #InsertFinancePlanDetail
(
	@OUID UniqueIdentifier,
	@ProviderID UniqueIdentifier,
	@Name VARCHAR(100),
	@YEARS INT,
	@NINPMonths INT,
	@APR FLOAT
)
AS

	DECLARE @id UNIQUEIDENTIFIER
	DECLARE @Index INT
	SET @id = NEWID()
	SET @Index = 0

	INSERT INTO finance.PlanDefinitions
	(Guid, ProviderID, Type, IsDisabled, Name, TenantID, DateCreated, Version, IsDeleted, TermInYears)
	VALUES
	(@id, @ProviderID, 2, 0, @Name, 0, GETUTCDATE(), 0, 0, @YEARS)

	INSERT INTO finance.OUAssociation
	(Guid, OUID, FinancePlanDefinitionID, TenantID, DateCreated, Version, IsDeleted)
	VALUES
	(NEWID(), @OUID, @id, 0, GETUTCDATE(), 0, 0)

	IF (@NINPMonths < 18)
	BEGIN
		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantID, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @id, @APR, @Index, 0, 0, @NINPMonths, 'Deferred Period', 0, GETUTCDATE(), 0, 0, 0, 0, 0, 0, 0)

		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantID, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @id, @APR, @Index + 1, 0, 0, 18 - @NINPMonths, 'Regular Period w/ incentives', 0, GETUTCDATE(), 0, 0, 0, 0, 1, 1, 1)

		SET @Index = @Index + 2
	END
	ELSE
	BEGIN
		INSERT INTO finance.FinanceDetails
		(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantID, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
		VALUES
		(NEWID(), @id, @APR,@Index, 0, 0, @NINPMonths, 'Deferred Period', 0, GETUTCDATE(), 0, 0, 0, 0, 0, 0, 1)
		SET @Index = @Index + 1
	END

	INSERT INTO finance.FinanceDetails
	(Guid, FinancePlanDefinitionID, Apr, [Order], PrincipalIsPaid, ApplyPrincipalReductionAfter, Months, Name, TenantID, DateCreated, Version, IsDeleted, InterestIsPaid, IsSpruced, PrincipalType, InterestType, ApplyReductionAfterPeriod)
	VALUES
	(NEWID(), @id, @APR, @Index, 0, 0, @YEARS * 12 - 18, 'Regular Period', 0, GETUTCDATE(), 0, 0, 0, 0, 1, 1, 0)
GO

BEGIN TRAN

DECLARE @ProviderID UNIQUEIDENTIFIER
SET @ProviderID = '24b8a5ec-3819-48c1-a4a5-d49a6041b56e'

INSERT INTO finance.Providers
(Guid, ImageUrl, Name, TenantId, DateCreated, Version, IsDeleted, PhoneNumber)
VALUES
(@ProviderID, 'https://www.ccbankutah.com/Sitefinity/WebsiteTemplates/CapitalCommunityBank/App_Themes/CapitalCommunityBank/Images/logo.png', 'Capital Community Bank', 0, GETUTCDATE(), 0, 0, '(844) 635-3636')


DECLARE @OUID UNIQUEIDENTIFIER
-- for Dev and Stage, use Solar #1
IF EXISTS(SELECT Guid FROM OUs WHERE Name = 'Elements')
	BEGIN
		SELECT @OUID = Guid FROM OUs WHERE Name = 'Elements' AND IsDeleted = 0
	END
ELSE
	SELECT @OUID = Guid FROM OUs WHERE Name = 'Solar #1' AND IsDeleted = 0

DECLARE @Years INT
DECLARE @Apr FLOAT

SET @Years = 10
SET @Apr = 5.99

EXEC #InsertFinancePlanDetail @ouid, @ProviderID, 'CCBank 10 / 5.99% (12m NI/NP)', @Years, 12, @Apr
EXEC #InsertFinancePlanDetail @ouid, @ProviderID, 'CCBank 10 / 5.99% (15m NI/NP)', @Years, 15, @Apr
EXEC #InsertFinancePlanDetail @ouid, @ProviderID, 'CCBank 10 / 5.99% (18m NI/NP)', @Years, 18, @Apr

SET @Years = 25
SET @Apr = 4.99

EXEC #InsertFinancePlanDetail @ouid, @ProviderID, 'CCBank 25 / 4.99% (12m NI/NP)', @Years, 12, @Apr
EXEC #InsertFinancePlanDetail @ouid, @ProviderID, 'CCBank 25 / 4.99% (15m NI/NP)', @Years, 15, @Apr
EXEC #InsertFinancePlanDetail @ouid, @ProviderID, 'CCBank 25 / 4.99% (18m NI/NP)', @Years, 18, @Apr


COMMIT TRAN

DROP PROC #InsertFinancePlanDetail
