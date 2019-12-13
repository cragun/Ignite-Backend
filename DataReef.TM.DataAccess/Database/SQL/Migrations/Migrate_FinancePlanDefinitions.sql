-- Spruce 12 years
UPDATE [solar].[FinancePlans]
SET FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'Spruce 12 / 3.99%')
WHERE PeriodInMonths = 144 AND FinanceProvider = 3

-- Spruce 20 years
UPDATE [solar].[FinancePlans]
SET FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'Spruce 20 / 4.99%')
WHERE PeriodInMonths = 240 AND FinanceProvider = 3

-- Mosaic 12
UPDATE [solar].[FinancePlans]
SET FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'Mosaic 12 / 3.99 %')
WHERE PeriodInMonths = 144 AND FinanceProvider = 1

-- Mosaic 20
UPDATE [solar].[FinancePlans]
SET FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'Mosaic 20 / 5.99%')
WHERE PeriodInMonths = 240 AND FinanceProvider = 1

-- GreenSky 12
UPDATE [solar].[FinancePlans]
SET FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'GreenSky 12 / 2.99%')
WHERE PeriodInMonths = 144 AND FinanceProvider = 2

-- GreenSky 20
UPDATE [solar].[FinancePlans]
SET FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'GreenSky 12 / 2.99%')
WHERE PeriodInMonths = 240 AND FinanceProvider = 2

-- Delete PPA and Other old plans
UPDATE [solar].[FinancePlans]
SET IsDeleted = 1,
FinancePlanDefinitionID = (SELECT TOP 1 Guid FROM finance.PlanDefinitions WHERE Name = 'OBSOLETE PLAN (DO NOT USE)')
WHERE [Name] = 'PPA' OR [Name] = 'New FinancePlan'
