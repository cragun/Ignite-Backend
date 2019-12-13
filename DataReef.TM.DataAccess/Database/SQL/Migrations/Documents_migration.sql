-- *************************************************
-- Case 01, Doc 01: Proposal for SunEdison & PPA
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,					   FinancePlanID, DocumentType,	  SignedURL,		   UnsignedURL,	SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), P.Guid), FP.Guid,		  1 /*Proposal*/, P.SignedProposalURL, NULL,		P.DateSigned, 0,		P.DateCreated, 0,		 0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 1) -- PPA
	AND FP.SolarProviderType = 1 -- SunEdison

-- Case 01, Doc 02: PPA Contracts for SunEdison & PPA
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,						  FinancePlanID, DocumentType,		SignedURL,			 UnsignedURL,			SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), FP.[Guid]), FP.Guid,		 3 /*PPAContract*/, P.SignedContractURL, P.UnsignedContractURL, P.DateSigned, 0,		P.DateCreated, 0,		 0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 1) -- PPA
	AND FP.SolarProviderType = 1 -- SunEdison

-- *************************************************
-- Case 02, Doc 01: Mosaic Proposals for SunEdison & (Mosaic12 || Mosaic20)
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,					   FinancePlanID, DocumentType,	  SignedURL,		   UnsignedURL,	SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), P.Guid), FP.Guid,		  1 /*Proposal*/, P.SignedProposalURL, NULL,		P.DateSigned, 0,		P.DateCreated, 0,		 0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 11 OR FP.FinancePlanType = 12) -- Mosaic12 || Mosaic20
	AND FP.SolarProviderType = 1 -- SunEdison

-- Case 02, Doc 02: Mosaic Contracts for SunEdison & (Mosaic12 || Mosaic20)
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,						  FinancePlanID, DocumentType,		 SignedURL,			  UnsignedURL,			 SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), FP.[Guid]), FP.Guid,		 4 /*LoanContract*/, P.SignedContractURL, P.UnsignedContractURL, P.DateSigned, 0,		 P.DateCreated, 0,		  0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 11 OR FP.FinancePlanType = 12) -- Mosaic12 || Mosaic20
	AND FP.SolarProviderType = 1 -- SunEdison

-- *************************************************
-- Case 03, Doc 01: Greensky Proposals for SunEdison & (GreenSky12 || GreenSky20)
-- similar to the above, but separate in case something
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,					   FinancePlanID, DocumentType,	  SignedURL,		   UnsignedURL,	SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), P.Guid), FP.Guid,		  1 /*Proposal*/, P.SignedProposalURL, NULL,		P.DateSigned, 0,		P.DateCreated, 0,		 0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 13 OR FP.FinancePlanType = 14) -- GreenSky12 || GreenSky20
	AND FP.SolarProviderType = 1 -- SunEdison

-- Case 03, Doc 02: Greensky Contracts for SunEdison & (GreenSky12 || GreenSky20)
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,						  FinancePlanID, DocumentType,		 SignedURL,			  UnsignedURL,			 SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), FP.[Guid]), FP.Guid,		 4 /*LoanContract*/, P.SignedContractURL, P.UnsignedContractURL, P.DateSigned, 0,		 P.DateCreated, 0,		  0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 13 OR FP.FinancePlanType = 14) -- GreenSky12 || GreenSky20
	AND FP.SolarProviderType = 1 -- SunEdison

-- *************************************************
-- Case 04, Doc 01: Spruce Proposals for Solcius & (Spruce12 || Spruce20)
-- similar to the above, but separate in case something
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,					   FinancePlanID, DocumentType,	  SignedURL,		   UnsignedURL,	SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), P.Guid), FP.Guid,		  1 /*Proposal*/, P.SignedProposalURL, NULL,		P.DateSigned, 0,		P.DateCreated, 0,		 0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 15 OR FP.FinancePlanType = 16) -- GreenSky12 || GreenSky20
	AND FP.SolarProviderType = 2 -- Solcius

-- Case 03, Doc 02: Spruce Agreement for Solcius & (Spruce12 || Spruce20)
INSERT INTO solar.ProposalDocuments
	 ([Guid],	DocumentID,						  FinancePlanID, DocumentType,	  SignedURL,		   UnsignedURL,			 SignedDate,   TenantID, DateCreated,  [Version], IsDeleted)
SELECT NEWID(), convert(nvarchar(50), FP.[Guid]), FP.Guid,		 3 /*Agreement*/, P.SignedContractURL, P.UnsignedContractURL, P.DateSigned, 0,		 P.DateCreated, 0,		  0
FROM solar.Proposals P
	INNER JOIN solar.FinancePlans FP
	ON P.Guid = FP.SolarSystemID
WHERE 
	P.SignedProposalURL IS NOT NULL
	AND FP.ContractID IS NOT NULL
	AND FP.IsDeleted = 0 
	AND P.IsDeleted = 0
	AND p.Status <> 0
	AND (FP.FinancePlanType = 15 OR FP.FinancePlanType = 16) -- GreenSky12 || GreenSky20
	AND FP.SolarProviderType = 2 -- Solcius
