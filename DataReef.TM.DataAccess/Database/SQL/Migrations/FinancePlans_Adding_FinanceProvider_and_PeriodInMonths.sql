
UPDATE solar.FinancePlans
SET RequestJSON = COALESCE(NULLIF(LoanRequestJSON,''), PPARequestJSON)

UPDATE solar.FinancePlans
SET ResponseJSON = COALESCE(NULLIF(LoanResponseJSON,''), PPAResponseJSON)


-- PPA (1)
UPDATE solar.FinancePlans
SET FinancePlanType = 1,
	FinanceProvider = 0,
	PeriodInMonths = 0
WHERE FinancePlanType = 1

-- LoanMosaic12Year (11)
UPDATE solar.FinancePlans
SET FinancePlanType = 2,
	FinanceProvider = 1,
	PeriodInMonths = 144
WHERE FinancePlanType = 11

-- LoanMosaic20Year (12)
UPDATE solar.FinancePlans
SET FinancePlanType = 2,
	FinanceProvider = 1,
	PeriodInMonths = 240
WHERE FinancePlanType = 12

-- LoanGreenSky12Year (13)
UPDATE solar.FinancePlans
SET FinancePlanType = 2,
	FinanceProvider = 2,
	PeriodInMonths = 144
WHERE FinancePlanType = 13

-- LoanGreenSky20Year (14)
UPDATE solar.FinancePlans
SET FinancePlanType = 2,
	FinanceProvider = 2,
	PeriodInMonths = 240
WHERE FinancePlanType = 14

-- LoanSpurce12Year (15)
UPDATE solar.FinancePlans
SET FinancePlanType = 2,
	FinanceProvider = 3,
	PeriodInMonths = 144
WHERE FinancePlanType = 15

-- LoanSpruce20Year (16)
UPDATE solar.FinancePlans
SET FinancePlanType = 2,
	FinanceProvider = 3,
	PeriodInMonths = 240
WHERE FinancePlanType = 16

