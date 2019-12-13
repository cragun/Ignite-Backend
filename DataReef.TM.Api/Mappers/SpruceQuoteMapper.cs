
using System;
namespace DataReef.TM.Api.Mappers
{
    public class SpruceQuoteMapper
    {
        public static DataReef.Integrations.Spruce.DTOs.QuoteRequest MapRequestToDTO(DataReef.TM.Models.Spruce.QuoteRequest entity)
        {
            var setupInfoDTO = new Integrations.Spruce.DTOs.Init
            {
                ContractorId = entity.SetupInfo.ContractorId,
                Product = entity.SetupInfo.Product,
                Partner = entity.SetupInfo.Partner,
                Program = entity.SetupInfo.Program,
                ContractorQuoteId1 = entity.SetupInfo.ContractorQuoteId1,
                ContractorQuoteId2 = entity.SetupInfo.ContractorQuoteId2,
                RateType = entity.SetupInfo.RateType,
                FinOptId = entity.SetupInfo.FinOptId,
                TermInMonths = entity.SetupInfo.TermInMonths,
                CashSalesPrice = entity.SetupInfo.CashSalesPrice,
                DownPayment = entity.SetupInfo.DownPayment,
                AmountFinanced = entity.SetupInfo.AmountFinanced,
                PrefAgreementTypeId = entity.SetupInfo.PrefAgreementTypeId,
                DeliveryMethodId = entity.SetupInfo.DeliveryMethodId
            };

            var appInfoDTO = new Integrations.Spruce.DTOs.Applicant
            {
                NamePrefix = entity.AppInfo.NamePrefix,
                NameSuffix = entity.AppInfo.NameSuffix,
                FirstName = entity.AppInfo.FirstName,
                MiddleName = entity.AppInfo.MiddleName,
                LastName = entity.AppInfo.LastName,
                VerifyLicense = entity.AppInfo.VerifyLicense,
                BirthDate = entity.AppInfo.BirthDate,
                SSN = entity.AppInfo.SSN.Replace("-", ""),
                InstallationAddress = entity.AppInfo.InstallationAddress,
                InstallationCity = entity.AppInfo.InstallationCity,
                InstallationState = entity.AppInfo.InstallationState,
                InstallationZipCode = entity.AppInfo.InstallationZipCode,
                IsMailingDifferentInstall = entity.AppInfo.IsMailingDifferentInstall,
                MailingAddressLine1 = entity.AppInfo.MailingAddressLine1,
                MailingAddressLine2 = entity.AppInfo.MailingAddressLine2,
                MailingCity = entity.AppInfo.MailingCity,
                MailingState = entity.AppInfo.MailingState,
                MailingZipCode = entity.AppInfo.MailingZipCode,
                Email = entity.AppInfo.Email,
                HomePhone = entity.AppInfo.HomePhone.Replace("-", ""),
                CellPhone = entity.AppInfo.CellPhone.Replace("-", ""),
                HasCoApplicant = entity.AppInfo.HasCoApplicant
            };

            Integrations.Spruce.DTOs.CoApplicant coAppInfoDTO = null;
            if (entity.CoAppInfo != null)
            {
                coAppInfoDTO = new Integrations.Spruce.DTOs.CoApplicant
                {
                    NamePrefix  = entity.CoAppInfo.NamePrefix,
                    NameSuffix  = entity.CoAppInfo.NameSuffix,
                    FirstName  = entity.CoAppInfo.FirstName,
                    MiddleName  = entity.CoAppInfo.MiddleName,
                    LastName  = entity.CoAppInfo.LastName,
                    VerifyLicense  = entity.CoAppInfo.VerifyLicense,
                    BirthDate  = entity.CoAppInfo.BirthDate,
                    SSN = entity.CoAppInfo.SSN.Replace("-", ""),
                    IsCoAppDifferentMailing  = entity.CoAppInfo.IsCoAppDifferentMailing,
                    MailingAddressLine1  = entity.CoAppInfo.MailingAddressLine1,
                    MailingAddressLine2  = entity.CoAppInfo.MailingAddressLine2,
                    MailingCity  = entity.CoAppInfo.MailingCity,
                    MailingState  = entity.CoAppInfo.MailingState,
                    MailingZipCode  = entity.CoAppInfo.MailingZipCode,
                    Email  = entity.CoAppInfo.Email,
                    HomePhone = entity.CoAppInfo.HomePhone.Replace("-", ""),
                    CellPhone = entity.CoAppInfo.CellPhone.Replace("-", "")
                };
            }

            Integrations.Spruce.DTOs.Employment appEmploymentDTO = null;

            if (entity.AppEmployment != null)
            {
                appEmploymentDTO = new Integrations.Spruce.DTOs.Employment
                {
                    EmployedSince = entity.AppEmployment.EmployedSince,
                    EmploymentStatus = entity.AppEmployment.EmploymentStatus
                };
            }            

            Integrations.Spruce.DTOs.Employment coAppEmploymentDTO = null;
            if (entity.CoAppEmployment != null)
            {
                coAppEmploymentDTO = new Integrations.Spruce.DTOs.Employment
                {
                    EmployedSince = entity.CoAppEmployment.EmployedSince,
                    EmploymentStatus = entity.CoAppEmployment.EmploymentStatus
                };
            }

            Integrations.Spruce.DTOs.IncomeDebt appIncomeDebtDTO = null; 
            if (entity.AppIncomeDebt != null)
            {
                appIncomeDebtDTO = new Integrations.Spruce.DTOs.IncomeDebt
                {
                    AnnualIncome = entity.AppIncomeDebt.AnnualIncome
                };
            }            

            Integrations.Spruce.DTOs.IncomeDebt coAppIncomeDebtDTO = null;
            if (entity.CoAppIncomeDebt != null)
            {
                coAppIncomeDebtDTO = new Integrations.Spruce.DTOs.IncomeDebt
                {
                    AnnualIncome = entity.CoAppIncomeDebt.AnnualIncome
                };
            }

            Integrations.Spruce.DTOs.SpruceProperty propertyDTO = null;

            if (entity.Property != null)
            {
                propertyDTO = new Integrations.Spruce.DTOs.SpruceProperty
                {
                    MonthlyMortgagePayment = entity.Property.MonthlyMortgagePayment,
                    TitleHolder = entity.Property.TitleHolder
                };
            }            

            var quoteRequestDTO = new Integrations.Spruce.DTOs.QuoteRequest
            {
                SetupInfo = setupInfoDTO,
                AppInfo = appInfoDTO,
                CoAppInfo = coAppInfoDTO,
                AppEmployment = appEmploymentDTO,
                CoAppEmployment = coAppEmploymentDTO,
                AppIncomeDebt = appIncomeDebtDTO,
                CoAppIncomeDebt = coAppIncomeDebtDTO,
                Property = propertyDTO
            };

            return quoteRequestDTO;
        }

        internal static Models.Spruce.QuoteResponse MapDTOToResponse(Integrations.Spruce.DTOs.QuoteResponse dto, Guid requestId)
        {
            var response = new Models.Spruce.QuoteResponse
            {
                Guid = requestId,
                QuoteNumber = dto.QuoteNo != 0 ? dto.QuoteNo : dto.QuoteNumber,
                Decision = dto.Decision,
                CreditResponse = dto.CreditResponse,
                DecisionDateTime = dto.DecisionDateTime,
                AmountFinanced = dto.AmountFinanced,
                LoanRate = dto.LoanRate,
                Term = dto.Term,
                IntroRatePayment = dto.IntroRatePayment,
                IntroTerm = dto.IntroTerm,
                MonthlyPayment = dto.MonthlyPayment,
                GotoTerm = dto.GotoTerm,
                StipulationText = dto.StipulationText,
                MaxApproved = dto.MaxApproved
            };

            return response;
        }

        internal static string MapHardCreditCheckCallbackRequestDecisionToQuoteResponseDecision(string decision)
        {
            switch(decision) 
            {
                case "APPR":
                    return "A";
                case "DENY":
                    return "F";
                default:
                    return "P";
            }
        }
    }
}