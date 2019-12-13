using DataReef.Integrations.Spruce.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.Integrations.Spruce
{
    public class LoanCalculator
    {
        public static LoanResponseSpruce CalculateResponse(LoanRequestSpruce request, LoanResponseSpruceExternal responseExternal)
        {
            var response = new LoanResponseSpruce
            {
                AmountFinanced = responseExternal.Amount_Financed,
                PromoMonthlyPayment = responseExternal.Promo_Monthly_Payment,
                MonthlyPaymentAfterPromoNoQualifiedPrepayment = responseExternal.Monthly_Payment_After_Promo_NO_QUALIFIED_PREPAYMENT,
                MonthlyPaymentAfterPromoWithQualifiedPrepayment = responseExternal.Monthly_Payment_After_Promo_WITH_QUALIFIED_PREPAYMENT,
                PaymentOptions = responseExternal.Payment_Options != null ? responseExternal.Payment_Options.Select(po => new PaymentOption { Term = po.a_Term, MonthlyPayment = po.b_MonthlyPayment }).ToList() : null,
                Disclaimer = responseExternal.Calculator_Disclaimer,
                OffsetMonths = request.DaysDeferred <= 0 ? 0 : (request.DaysDeferred / 30) - 1, // we approximate a month with 30 days, it doesn't matter here whether it has 28, 29, 30 or 31
                Key = request.Key
            };

            var termMonths = request.Term * 12;
            var scenarioTermMonths = request.ScenarioTerm * 12;

            response.FederalTaxIncentive = response.AmountFinanced * request.FederalTaxIncentivePercentage;
            response.StateTaxIncentive = request.StateTaxIncentive;
            var prepayment = response.FederalTaxIncentive + response.StateTaxIncentive;

            var months = new List<PaymentMonth>();

            decimal previousUnpaidInternalBalance = response.PromoMonthlyPayment * response.OffsetMonths;
            decimal previousPaymentBalance = response.AmountFinanced;


            //for (int idxMonth = 1; idxMonth <= termMonths; idxMonth++)
            for (int idxMonth = 1; idxMonth <= scenarioTermMonths; idxMonth++)
            {
                var month = new PaymentMonth
                {
                    Year = (int)((idxMonth - 1) / 12) + 1,
                    Month = idxMonth,
                };

                if (idxMonth < request.IntroTerm - response.OffsetMonths)
                {
                    month.PaymentAmount = response.PromoMonthlyPayment;
                    month.InterestCharge = response.PromoMonthlyPayment;
                    month.Principal = 0;
                    month.BenefitsAndIncentives = 0;
                }
                else if (idxMonth == request.IntroTerm - response.OffsetMonths) // the month of prepayment
                {
                    if (prepayment > 0)
                    {
                        month.PaymentAmount = prepayment;
                    }
                    else
                    {
                        month.PaymentAmount = response.PromoMonthlyPayment;
                    }
                    month.InterestCharge = response.PromoMonthlyPayment;
                    month.Principal = month.PaymentAmount - month.InterestCharge - previousUnpaidInternalBalance;
                    month.BenefitsAndIncentives = prepayment;
                }
                else
                {
                    month.InterestCharge = previousPaymentBalance * (request.Apr / (100 /*percent*/ * 12 /*months*/));
                    if (idxMonth >= termMonths)
                    {
                        month.PaymentAmount = previousPaymentBalance + month.InterestCharge;
                    }
                    else
                    {
                        if (prepayment > 0)
                        {
                            month.PaymentAmount = response.MonthlyPaymentAfterPromoWithQualifiedPrepayment;
                        }
                        else
                        {
                            month.PaymentAmount = response.MonthlyPaymentAfterPromoNoQualifiedPrepayment;
                        }
                    }
                    month.Principal = month.PaymentAmount - month.InterestCharge - previousUnpaidInternalBalance;
                    month.BenefitsAndIncentives = 0;
                }

                if (month.Principal < 0) month.Principal = 0;

                month.PaymentBalance = previousPaymentBalance - month.Principal;

                if (previousUnpaidInternalBalance > 0 && (month.PaymentAmount - month.InterestCharge > 0))
                {
                    month.UnpaidInternalBalance = previousUnpaidInternalBalance - (month.PaymentAmount - month.InterestCharge);
                    if (month.UnpaidInternalBalance < 0) month.UnpaidInternalBalance = 0;
                }
                else
                {
                    month.UnpaidInternalBalance = previousUnpaidInternalBalance;
                }

                previousUnpaidInternalBalance = month.UnpaidInternalBalance;
                previousPaymentBalance = month.PaymentBalance;

                var currentMonthValues = request.Consumption[idxMonth % 12];
                var monthConsumption = currentMonthValues.ConsumptionInKwh;
                var monthProduction = GetValueForMonth(idxMonth, request.FirstYearAnnualProduction, (decimal)request.Derate, goingUp: false);
                var monthPrice = GetValueForMonth(idxMonth, currentMonthValues.Price * 12, (decimal)request.UtilityInflationRate, goingUp: true);

                month.SystemProduction = monthProduction;
                month.ElectricityBillWithoutSolar = monthPrice;
                month.ElectricityBillWithSolar = ((decimal)(monthConsumption - monthProduction) * monthPrice / monthConsumption);
                month.Savings = month.ElectricityBillWithoutSolar + month.BenefitsAndIncentives - (month.ElectricityBillWithSolar + month.PaymentAmount);

                months.Add(month);
            }
            response.Months = request.IncludeMonthsInResponse ? months.Where(m => m.Year <= request.Term).ToList() : null; // only add Term months to response.

            var years = months.GroupBy(m => m.Year).Select(g => new PaymentYear
            {
                Year = g.Key,
                PaymentAmount = g.Key <= request.Term ? g.Sum(m => m.PaymentAmount) : 0,
                InterestCharge = g.Key <= request.Term ? g.Sum(m => m.InterestCharge) : 0,
                Principal = g.Key <= request.Term ? g.Sum(m => m.Principal) : 0,
                PaymentBalance = g.Key <= request.Term ? g.Where(m => m.Month == g.Max(mm => mm.Month)).Single().PaymentBalance : 0,
                UnpaidInternalBalance = g.Key <= request.Term ? g.Where(m => m.Month == g.Max(mm => mm.Month)).Single().UnpaidInternalBalance : 0,
                SystemProduction = (long)g.Sum(m => m.SystemProduction),
                ElectricityBillWithoutSolar = g.Sum(m => m.ElectricityBillWithoutSolar),
                ElectricityBillWithSolar = g.Sum(m => m.ElectricityBillWithSolar),
                BenefitsAndIncentives = g.Key <= request.Term ? g.Sum(m => m.BenefitsAndIncentives) : 0,
                Savings = g.Sum(m => m.Savings)
            }).ToList();

            response.Years = years;

            response.TotalSystemProduction = years.Sum(y => y.SystemProduction);
            response.TotalElectricityBillWithoutSolar = years.Sum(y => y.ElectricityBillWithoutSolar);
            response.TotalElectricityBillWithSolar = years.Sum(y => y.ElectricityBillWithSolar);
            response.TotalSavings = years.Sum(y => y.Savings);
            response.TotalSolarPaymentsCost = years.Sum(y => y.PaymentAmount);

            return response;
        }

        private static decimal GetValueForYear(int year, decimal firstYearAnnualValue, decimal variationRate, bool goingUp)
        {
            if (year <= 0) return 0;
            if (year == 1) return firstYearAnnualValue;

            decimal lastProduction = firstYearAnnualValue;
            decimal annualProduction = lastProduction;

            for (int x = 2; x <= year; x++)
            {
                annualProduction = lastProduction * (1 + (goingUp ? 1 : -1) * variationRate);
                lastProduction = annualProduction;
            }

            return annualProduction;
        }

        private static decimal GetValueForMonth(int month, decimal firstYearAnnualValue, decimal variationRate, bool goingUp)
        {
            int year = (int)Math.Ceiling((double)month / 12);
            return GetValueForYear(year, firstYearAnnualValue, variationRate, goingUp) / 12.0M;
        }

    }
}
