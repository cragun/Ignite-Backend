CREATE PROC #tempInsertPlanDefinitions
@ProviderName NVARCHAR(250),
@Type INT,
@Name NVARCHAR(250),
@Years INT,
@PlanRate FLOAT = 0,
@IntegrationProvider INT = 0,
@DealerFee FLOAT = NULL,
@TagString nvarchar(1000) = NULL
AS
BEGIN
	DECLARE @ProviderID UNIQUEIDENTIFIER
	SELECT @ProviderID = [Guid] FROM [finance].[Providers] WHERE Name = @Name

	IF @ProviderID IS NULL
	BEGIN
		INSERT INTO [finance].[PlanDefinitions]
		(Guid, ProviderID, [Type], IsDisabled, [Name], TenantId, [DateCreated], Version, IsDeleted, TermInYears, TagString, DealerFee, Apr, IntegrationProvider)
		VALUES
		(NEWID(), (SELECT [Guid] FROM [finance].[Providers] WHERE Name = @ProviderName), @Type, 0, @Name, 0, GETUTCDATE(), 1, 0, @Years, @TagString, @DealerFee, @PlanRate, @IntegrationProvider)
		RETURN 1
	END
	RETURN 0
END
GO

BEGIN TRAN

DECLARE @ProviderName VARCHAR(250) = 'LoanPal'
declare @providerID uniqueidentifier = (select Guid from finance.Providers where Name = @ProviderName)

IF (@ProviderID IS NULL)
BEGIN
	SET @providerID = NEWID()

	insert into finance.Providers(Guid,ImageUrl,Name,TenantID,DateCreated,Version,IsDeleted, ProposalFlowType)
	values (@providerID,'http://loanpal.com/resources/img/loanpal-2017.png',@ProviderName,0,GETDATE(),0,0,0)

END

DECLARE @Response INT
DECLARE @PlanName VARCHAR(250)
DECLARE @PlanRate FLOAT
DECLARE @DealerFee FLOAT
DECLARE @TermInYears INT
DECLARE @MainPeriodMonths INT


SET @PlanName = 'LoanPal 20 / 3.99%'
SET @PlanRate = 3.99
SET @TermInYears = 20
SET @DealerFee = NULL
EXEC @Response = #tempInsertPlanDefinitions @ProviderName, 2, @PlanName, @TermInYears, @PlanRate, 1, @DealerFee

SET @PlanName = 'LoanPal 20 / 4.99%'
SET @PlanRate = 4.99
EXEC @Response = #tempInsertPlanDefinitions @ProviderName, 2, @PlanName, @TermInYears, @PlanRate, 1, @DealerFee


SET @PlanName = 'LoanPal 20 / 5.99%'
SET @PlanRate = 5.99
EXEC @Response = #tempInsertPlanDefinitions @ProviderName, 2, @PlanName, @TermInYears, @PlanRate, 1, @DealerFee

SET @PlanName = 'LoanPal 10 / 2.99%'
SET @TermInYears = 10
SET @PlanRate = 2.99
EXEC @Response = #tempInsertPlanDefinitions @ProviderName, 2, @PlanName, @TermInYears, @PlanRate, 1, @DealerFee

SET @PlanName = 'LoanPal 10 / 3.99%'
SET @PlanRate = 3.99
EXEC @Response = #tempInsertPlanDefinitions @ProviderName, 2, @PlanName, @TermInYears, @PlanRate, 1, @DealerFee


COMMIT TRAN

DROP PROC #tempInsertPlanDefinitions
