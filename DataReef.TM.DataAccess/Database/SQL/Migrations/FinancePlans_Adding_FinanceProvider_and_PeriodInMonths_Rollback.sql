-- PPA (1)
UPDATE solar.FinancePlans
SET FinancePlanType = 1	
WHERE FinancePlanType = 1 
	AND FinanceProvider = 0
	AND PeriodInMonths = 0

-- LoanMosaic12Year (11)
UPDATE solar.FinancePlans
SET FinancePlanType = 11
WHERE FinancePlanType = 2
	AND FinanceProvider = 1
	AND PeriodInMonths = 144

-- LoanMosaic20Year (12)
UPDATE solar.FinancePlans
SET FinancePlanType = 12	
WHERE FinancePlanType = 2
	AND FinanceProvider = 1
	AND PeriodInMonths = 240

-- LoanGreenSky12Year (13)
UPDATE solar.FinancePlans
SET FinancePlanType = 13	
WHERE FinancePlanType = 2
	AND FinanceProvider = 2
	AND PeriodInMonths = 144

-- LoanGreenSky20Year (14)
UPDATE solar.FinancePlans
SET FinancePlanType = 14	
WHERE FinancePlanType = 2
	AND FinanceProvider = 2
	AND PeriodInMonths = 240

-- LoanSpurce12Year (15)
UPDATE solar.FinancePlans
SET FinancePlanType = 15	
WHERE FinancePlanType = 2
	AND FinanceProvider = 3
	AND PeriodInMonths = 144

-- LoanSpruce20Year (16)
UPDATE solar.FinancePlans
SET FinancePlanType = 16	
WHERE FinancePlanType = 2
	AND FinanceProvider = 3
	AND PeriodInMonths = 240

