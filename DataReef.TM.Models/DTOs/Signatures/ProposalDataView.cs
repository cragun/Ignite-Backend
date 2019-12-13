using DataReef.TM.Models.DTOs.Charting;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Tables;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DataReef.Core.Extensions;
using Newtonsoft.Json;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class ProposalDataView
    {
        [MergeAlias("Property_ID")]
        public string PropertyID { get; set; }

        [MergeAlias("Property_Locator_ID")]
        public string PropertyLocatorID { get; set; }


        [MergeAlias("Proposal_Number")]
        public long ProposalNumber { get; set; }

        [MergeAlias("First_Name")]
        public string FirstName { get; set; }

        [MergeAlias("Last_Name")]
        public string LastName { get; set; }

        [MergeAlias("Customer_Full_Name")]
        public string CustomerFullName { get; set; }

        [MergeAlias("Address_Line_1")]
        public string Address1 { get; set; }

        [MergeAlias("Address_Line_2")]
        public string Address2 { get; set; }

        [MergeAlias("Formatted_Address")]
        public string FormattedAddress { get; set; }

        [MergeAlias("City")]
        public string City { get; set; }

        [MergeAlias("State")]
        public string State { get; set; }

        [MergeAlias("Zip_Code")]
        public string ZipCode { get; set; }

        [MergeAlias("Sales_Rep_Name")]
        public string SalesRepName { get; set; }

        [MergeAlias("Sales_Rep_PhoneNumber")]
        public string SalesRepPhoneNumber { get; set; }

        [MergeAlias("Sales_Rep_Email")]
        public string SalesRepEmail { get; set; }

        [MergeAlias("Dealer_Name")]
        public string DealerName { get; set; }

        /// <example>$5.00</example>
        [MergeAlias("Price_Per_Watt")]
        public string PricePerWatt { get; set; }

        [MergeAlias("Panel_Count")]
        public string PanelCount { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Current_Monthly_Bill")]
        public string CurrentMonthlyBill { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Monthly_Bill_In_25_Years")]
        public string MonthlyBillIn25Years { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Electricity_Bill_With_Solar")]
        public string TotalElectricityBillWithSolar { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Electricity_Bill_Without_Solar")]
        public string TotalElectricityBillWithoutSolar { get; set; }

        /// <example>4.90%</example>
        [MergeAlias("Utility_Inflation_Rate")]
        public string UtilityInflationRate { get; set; }

        /// <example>11,066 kWh</example>
        [MergeAlias("First_Year_Production")]
        public string FirstYearProduction { get; set; }

        /// <example>11,066 kWh</example>
        [MergeAlias("Total_Production")]
        public string TotalProduction { get; set; }

        /// <example>89%</example>
        [MergeAlias("Solar_Power_Percentage")]
        public string SolarPowerPercentage { get; set; }

        /// <summary>100 - SolarPowerPercentage</summary>
        /// <example>11%</example>
        [MergeAlias("Utility_Power_Percentage")]
        public string UtilityPowerPercentage { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Solar_Payments_Cost")]
        public string TotalSolarPaymentsCost { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Benefits_And_Incentives")]
        public string TotalBenefitsAndIncentives { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Paid_By_User")]
        public string TotalPaidByUser { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Savings")]
        public string TotalSavings { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Yearly_Savings")]
        public string YearlySavings { get; set; }

        [MergeAlias("First_Year_Savings")]
        public string FirstYearSavings { get; set; }

        /// <example>$0.441</example>
        [MergeAlias("Utility_Power_Cost")]
        public string UtilityPowerCost { get; set; }

        /// <example>$0.323</example>
        [MergeAlias("Solar_Power_Cost")]
        public string SolarPowerCost { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Intro_Monthly_Payment")]
        public string IntroMonthlyPayment { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Monthly_Payment")]
        public string MonthlyPayment { get; set; }

        [MergeAlias("Total_System_Cost")]
        public string TotalSystemCost { get; set; }

        [MergeAlias("Tax_Rate")]
        public string TaxRate { get; set; }

        [MergeAlias("Gross_System_Cost")]
        public string GrossSystemCost { get; set; }

        /// <example>$1,242.05</example>; SystemCost with tax
        [MergeAlias("Solar_System_Cost")]
        public string SolarSystemCost { get; set; }

        [MergeAlias("Net_System_Cost")]
        public string NetSystemCost { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Extra_Costs")]
        public string TotalExtraCosts { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Federal_Investment_Tax_Credit")]
        public string TotalFederalInvestmentTaxCredit { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_State_Tax_Incentive")]
        public string TotalStateTaxIncentive { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_PBI")]
        public string TotalPBI { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_SREC")]
        public string TotalSREC { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Total_Upfront_Rebate")]
        public string TotalUpfrontRebate { get; set; }

        /// <example>4.99%</example>
        [MergeAlias("Interest_Rate")]
        public string InterestRate { get; set; }

        /// <example>20 years</example>
        [MergeAlias("Loan_Term_in_Years")]
        public string LoanTermInYears { get; set; }

        [MergeAlias("Loan_Term_in_Months")]
        public string LoanTermInMonths { get; set; }

        /// <example>25 years</example>
        [MergeAlias("Scenario_Term_in_Years")]
        public string ScenarioTermInYears { get; set; }

        [MergeAlias("Intro_Period_Months")]
        public string IntroPeriodMonths { get; set; }

        [MergeAlias("Main_Loan_Starting_Month")]
        public string MainLoanStartingMonth { get; set; }

        [MergeAlias("Utility_Name")]
        public string UtilityName { get; set; }

        [MergeAlias("Tariff_Name")]
        public string TariffName { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Current_Annual_Bill")]
        public string CurrentAnnualBill { get; set; }

        /// <example>6.91 kW</example>
        [MergeAlias("System_Size")]
        public string SystemSize { get; set; }

        /// <example>0.5%</example>
        [MergeAlias("Annual_System_Degradation")]
        public string AnnualSystemDegradation { get; set; }

        [MergeAlias("Annual_Estimate_Production")]
        public string AnnualEstimatedProduction { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Down_Payment")]
        public string DownPayment { get; set; }

        public string AddersPaidByRep { get; set; }

        /// <example>$1,242.05</example>
        [MergeAlias("Amount_Financed")]
        public string AmountFinanced { get; set; }

        [MergeAlias("First_Year_Monthly_Bill_Without_Solar")]
        public string FirstYearMonthlyElectricityBillWithoutSolar { get; set; }

        [MergeAlias("Avg_Monthly_Bill_Without_Solar")]
        public string AvgMonthlyElectrityBillWithoutSolar { get; set; }

        [MergeAlias("First_Year_Monthly_Bill_With_Solar")]
        public string FirstYearMonthlyElectricityBillWithSolar { get; set; }

        [MergeAlias("Avg_Monthly_Bill_With_Solar")]
        public string AvgMonthlyElectrityBillWithSolar { get; set; }

        [MergeAlias("OrientationSlope")]
        public string OrientationSlope { get; set; }

        [MergeAlias("Shading")]
        public string Shading { get; set; }

        /// <example>
        /// 3 Sunniva 285 W, 22 Sunniva 275 W
        /// SolarEdge Inverter w/ power optimizers
        /// </example>
        [MergeAlias("Equipment_Info")]
        public string EquipmentInfo { get; set; }

        /// <summary>MM/dd/yyyy</summary>
        /// <example>08/18/2016</example>
        [MergeAlias("Date")]
        public string Date { get; set; }

        [MergeAlias("Customer_Phone_Number")]
        public string CustomerPhoneNumber { get; set; }

        [MergeAlias("Customer_Email_Address")]
        public string CustomerEmailAddress { get; set; }

        [MergeAlias("Pie_Chart_1")]
        public string PieChart1 { get; set; }

        [MergeAlias("Pie_Chart_Solcius")]
        public string PieChartSolcius { get; set; }

        [MergeAlias("Bar_Chart_1")]
        public string BarChart1 { get; set; }

        [MergeAlias("Bar_Chart_2")]
        public string BarChart2 { get; set; }

        [MergeAlias("Bar_Chart_3")]
        public string BarChart3 { get; set; }

        [MergeAlias("Table_1")]
        public string Table1 { get; set; }

        [MergeAlias("Table_CostsOverTime_Generic")]
        public string TableCostsOverTimeGeneric { get; set; }

        [MergeAlias("webhook_default_payload")]
        public string WebhookDefaultPayload { get; set; }

        [MergeAlias("webhook_request_payload")]
        public string WebhookRequestPayload { get; set; }

        [MergeAlias("webhook_solcius_payload")]
        public string WebhookSolciusPayload { get; set; }

        public static ProposalDataView FromCoreModel(Proposal proposal, FinancePlan financePlan, string contractorID, DateTime deviceDate)
        {
            ProposalDataView ret = new ProposalDataView();

            var negativeCurrencyMinusCulture = CultureInfo.CreateSpecificCulture("en-US");
            negativeCurrencyMinusCulture.NumberFormat.CurrencyNegativePattern = 1; // this culture displays negative currency numbers as -$#.## instead of ($#.##)

            ret.ProposalNumber = proposal.Id;

            var mainOccupant = proposal.Property.GetMainOccupant();
            if (mainOccupant != null)
            {
                ret.FirstName = mainOccupant.FirstName;
                ret.LastName = mainOccupant.LastName;
            }

            try { ret.PropertyID = proposal.PropertyID.ToString().ToUpper(); } catch { }
            ret.PropertyLocatorID = proposal.ExternalID;

            ret.CustomerFullName = proposal.NameOfOwner;
            ret.City = proposal.City;
            ret.State = proposal.State;
            ret.ZipCode = proposal.ZipCode;
            ret.Address1 = proposal.Address;
            ret.Address2 = proposal.Address2;
            ret.FormattedAddress = $"{proposal.Address} | {proposal.City}, {proposal.State} {proposal.ZipCode}";
            ret.DealerName = contractorID;

            if (proposal.SalesRep != null)
            {
                ret.SalesRepName = $"{proposal.SalesRep.FirstName} {proposal.SalesRep.MiddleName} {proposal.SalesRep.LastName}".Replace("  ", " ");
                ret.SalesRepPhoneNumber = proposal.SalesRep.PhoneNumbers != null && proposal.SalesRep.PhoneNumbers.Any()
                    ? proposal.SalesRep.PhoneNumbers.ElementAt(0).Number
                    : string.Empty;
                ret.SalesRepEmail = proposal.SalesRep.EmailAddresses != null && proposal.SalesRep.EmailAddresses.Any()
                    ? proposal.SalesRep.EmailAddresses[0]
                    : string.Empty;
            }

            if ((financePlan.FinancePlanType == Enums.FinancePlanType.Loan || financePlan.FinancePlanType == Enums.FinancePlanType.Cash) && !String.IsNullOrEmpty(financePlan.RequestJSON) && !String.IsNullOrEmpty(financePlan.ResponseJSON))
            {
                var loanRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanRequest>(financePlan.RequestJSON);
                var loanResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanResponse>(financePlan.ResponseJSON);
                var firstYear = loanResponse.Years.First();

                ret.PricePerWatt = loanRequest.PricePerWattASP.ToString("C", negativeCurrencyMinusCulture);
                ret.CurrentMonthlyBill = (firstYear.ElectricityBillWithoutSolar / 12).ToString("C", negativeCurrencyMinusCulture);
                ret.MonthlyBillIn25Years = (loanResponse.Years.Last().ElectricityBillWithoutSolar / 12).ToString("C", negativeCurrencyMinusCulture);
                ret.TotalElectricityBillWithSolar = loanResponse.TotalElectricityBillWithSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalElectricityBillWithoutSolar = loanResponse.TotalElectricityBillWithoutSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.UtilityInflationRate = (loanRequest.UtilityInflationRate * 100) + "%";
                ret.FirstYearProduction = firstYear.SystemProduction.ToString("N0") + " kWh";
                ret.TotalProduction = loanResponse.TotalSystemProduction.ToString("N0") + " kWh";
                int solarPowerPercentage = (int)Math.Round((proposal.SolarSystem.SystemProduction.Production / proposal.SolarSystem.SystemProduction.Consumption) * 100);
                ret.SolarPowerPercentage = solarPowerPercentage + "%";
                ret.UtilityPowerPercentage = (100 - solarPowerPercentage) + "%";
                ret.TotalSolarPaymentsCost = loanResponse.TotalSolarPaymentsCost.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalBenefitsAndIncentives = loanResponse.TotalBenefitsAndIncentives.ToString("C", negativeCurrencyMinusCulture);
                var totalPaidByUser = loanResponse.TotalSolarPaymentsCost - loanResponse.TotalBenefitsAndIncentives + loanResponse.TotalUpfrontRebate;

                // check if we should add the DownPayment to TotalPayedByUser
                ret.TotalPaidByUser = totalPaidByUser.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalSavings = loanResponse.TotalSavings.ToString("C", negativeCurrencyMinusCulture);

                var yearlySavings = loanRequest.ScenarioTermInYears == 0 ? 0 : (loanResponse.TotalSavings / loanRequest.ScenarioTermInYears);
                ret.YearlySavings = yearlySavings.ToString("C", negativeCurrencyMinusCulture);
                ret.FirstYearSavings = loanResponse.Years[0].Savings.ToString("C", negativeCurrencyMinusCulture);

                var solarElectricityRate = totalPaidByUser / loanResponse.TotalSystemProduction;
                var consumptionOver25Years = loanRequest.ScenarioTermInYears * loanRequest.MonthlyPower.Sum(mp => mp.Consumption);
                var solarUtilityElectricityRate = loanResponse.TotalElectricityBillWithSolar / (consumptionOver25Years - loanResponse.TotalSystemProduction);
                var averageSolarElectricityRate = consumptionOver25Years == 0 ? 0 : (solarElectricityRate * loanResponse.TotalSystemProduction + solarUtilityElectricityRate * (consumptionOver25Years - loanResponse.TotalSystemProduction)) / consumptionOver25Years;
                var electricityRateWithoutSolar = consumptionOver25Years == 0 ? 0 : loanResponse.TotalElectricityBillWithoutSolar / consumptionOver25Years;
                ret.UtilityPowerCost = electricityRateWithoutSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.SolarPowerCost = averageSolarElectricityRate.ToString("C", negativeCurrencyMinusCulture);
                ret.IntroMonthlyPayment = loanResponse.IntroMonthlyPayment.ToString("C", negativeCurrencyMinusCulture);
                ret.MonthlyPayment = loanResponse.MonthlyPayment.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalFederalInvestmentTaxCredit = loanResponse.TotalFederalTaxIncentive.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalStateTaxIncentive = loanResponse.TotalStateTaxIncentive.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalPBI = loanResponse.TotalPBI.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalSREC = loanResponse.TotalSREC.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalUpfrontRebate = loanResponse.TotalUpfrontRebate.ToString("C", negativeCurrencyMinusCulture);
                ret.InterestRate = (loanResponse.StatedApr / 100).ToString("P2"); //loanRequest.Apr + "%";
                //ret.LoanTermInYears = loanRequest.TermInYears + " years"; 
                ret.LoanTermInYears = $"{Math.Round(loanResponse.FinancingPeriodInMonths / (double)12)} years";
                ret.LoanTermInMonths = loanResponse.FinancingPeriodInMonths.ToString();
                ret.ScenarioTermInYears = loanRequest.ScenarioTermInYears + " years";
                ret.IntroPeriodMonths = loanResponse.IntroPeriodInMonths.ToString();
                ret.MainLoanStartingMonth = (loanResponse.IntroPeriodInMonths + 1).ToString();
                ret.UtilityName = proposal.Tariff.UtilityName;
                ret.TariffName = proposal.Tariff.Name;
                ret.CurrentAnnualBill = firstYear.ElectricityBillWithoutSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.SystemSize = ((double)loanRequest.SystemSize / 1000).ToString("N2") + " kW";
                ret.AnnualSystemDegradation = (loanRequest.Derate * 100) + "%";

                var annualEstimatedProduction = loanRequest.ScenarioTermInYears == 0 ? 0 : (loanResponse.TotalSystemProduction / loanRequest.ScenarioTermInYears);
                ret.AnnualEstimatedProduction = annualEstimatedProduction.ToString("0.##");
                ret.DownPayment = loanRequest.DownPayment.ToString("C", negativeCurrencyMinusCulture);
                ret.TaxRate = loanRequest.TaxRate.ToString("C", negativeCurrencyMinusCulture);
                ret.GrossSystemCost = loanRequest.GrossSystemCost.ToString("C", negativeCurrencyMinusCulture);
                ret.SolarSystemCost = loanRequest.GrossSystemCostWithTax.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalExtraCosts = loanRequest.ExtraCostsWithTax.ToString("C", negativeCurrencyMinusCulture);
                ret.TotalSystemCost = (loanRequest.GrossSystemCostWithTax + loanRequest.ExtraCostsWithTax).ToString("C", negativeCurrencyMinusCulture);
                ret.NetSystemCost = (loanRequest.GrossSystemCostWithTax + loanRequest.ExtraCostsWithTax - loanResponse.TotalBenefitsAndIncentives).ToString("C", negativeCurrencyMinusCulture);
                ret.AddersPaidByRep = loanResponse.AddersPaidByRep.ToString("C", negativeCurrencyMinusCulture);
                ret.AmountFinanced = loanResponse.AmountFinanced.ToString("C", negativeCurrencyMinusCulture);
                ret.PanelCount = proposal.SolarSystem.PanelCount.ToString("N");
                ret.CustomerPhoneNumber = proposal.Property.GetMainPhoneNumber();
                ret.CustomerEmailAddress = proposal.Property.GetMainEmailAddress();
                ret.EquipmentInfo = proposal.SolarSystem.GetEquipmentInfo();
                ret.Date = deviceDate.ToString("MM/dd/yyyy");
                ret.FirstYearMonthlyElectricityBillWithoutSolar = loanResponse.FirstYearMonthlyElectricityBillWithoutSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.AvgMonthlyElectrityBillWithoutSolar = loanResponse.AvgMonthlyElectrityBillWithoutSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.FirstYearMonthlyElectricityBillWithSolar = loanResponse.FirstYearMonthlyElectricityBillWithSolar.ToString("C", negativeCurrencyMinusCulture);
                ret.AvgMonthlyElectrityBillWithSolar = loanResponse.AvgMonthlyElectrityBillWithSolar.ToString("C", negativeCurrencyMinusCulture);

                ret.WebhookDefaultPayload = financePlan.ResponseJSON;
                ret.WebhookRequestPayload = financePlan.RequestJSON;
                ret.WebhookSolciusPayload = JsonConvert.SerializeObject(new SolciusPayload(proposal, financePlan));

                if (proposal.SolarSystem.RoofPlanes != null && proposal.SolarSystem.RoofPlanes.Any())
                {
                    foreach (var roofPlane in proposal.SolarSystem.RoofPlanes)
                    {
                        ret.Shading = string.IsNullOrEmpty(ret.Shading) ? $"{roofPlane.Shading}%" : $"{ret.Shading};{roofPlane.Shading}%";
                        ret.OrientationSlope = string.IsNullOrEmpty(ret.OrientationSlope) ? $"{roofPlane.Azimuth}°/{roofPlane.Tilt.RoundValue()}°" : $"{ret.OrientationSlope};{roofPlane.Azimuth}°/{roofPlane.Tilt.RoundValue()}°";
                    }
                }

                ret.PieChart1 = new Chart
                {
                    Type = ChartType.Pie,
                    Series = new List<Series>
                                {
                                    new Series
                                    {
                                        Points = new List<Point> {
                                            new Point { Label = "Solar Power "  + ret.SolarPowerPercentage, HexColor = "0xfffddb33", Value = solarPowerPercentage, LabelHexColor = "0xffa5a191" },
                                            new Point { Label = "Utility Power "  + ret.UtilityPowerPercentage, HexColor = "0xffbdbdbd", Value = 100 - solarPowerPercentage, LabelHexColor = "0xffa5a191" }
                                        }
                                    }
                                }
                }.ToString();
                ret.PieChartSolcius = new Chart
                {
                    Type = ChartType.Pie,
                    Series = new List<Series>
                    {
                        new Series
                        {
                            Points = new List<Point>
                            {
                                new Point { Label = $"Production {ret.SolarPowerPercentage} From Solar", HexColor = "0xffbdbdbd", Value = solarPowerPercentage, LabelHexColor = "0xffbdbdbd" },
                                new Point { Label = $"{ret.UtilityPowerPercentage} From Grid", HexColor = "0xfffddb33", Value = 100 - solarPowerPercentage, LabelHexColor = "0xfffddb33" }
                            }
                        }
                    },
                    Legend = new Legend { Alignment = Alignment.Near, Docking = Docking.Left }
                }.ToString();
                ret.BarChart1 = new Chart
                {
                    Type = ChartType.StackedColumn,
                    ValueLabelFormat = "C2",
                    Series = new List<Series>
                                {
                                    new Series {
                                        Name = "Utility",
                                        LabelAlignment  = "Center",
                                        Points = new List<Point> {
                                            new Point { Label = "Cost of doing nothing", HexColor = "0xffbdbdbd", Value = loanResponse.TotalElectricityBillWithoutSolar, LabelHexColor = "0xffffffff" },
                                            new Point { Label = "Solar savings", HexColor = "0xffbdbdbd", Value = loanResponse.TotalElectricityBillWithSolar, LabelHexColor = "0xffffffff" }
                                        }
                                    },
                                    new Series {
                                        Name = "Solar",
                                        LabelAlignment  = "Center",
                                        Points = new List<Point> {
                                            new Point { Label = "Cost of doing nothing", HexColor = "0xfffddb33", Value = null },
                                            new Point { Label = "Solar savings", HexColor = "0xfffddb33", Value = loanResponse.TotalSolarPaymentsCost - loanResponse.TotalBenefitsAndIncentives, LabelHexColor = "0xffffffff" }
                                        }
                                    },
                                    new Series {
                                        Name = "Savings",
                                        LabelAlignment  = "Center",
                                        Points = new List<Point> {
                                            new Point { Label = "Cost of doing nothing", HexColor = "0xfffdee88", Value = null },
                                            new Point { Label = "Solar savings", HexColor = "0xfffff4be", Value = loanResponse.TotalSavings, LabelHexColor = "0xffa5a191" }
                                        }
                                    }
                                }
                }.ToString();
                ret.BarChart2 = new Chart
                {
                    Type = ChartType.Column,
                    DrawZeroVisible = true,
                    ValueLabelFormat = "C2",
                    Series = new List<Series>
                                {
                                    new Series {
                                        LabelAlignment = "Top",
                                        Points = new List<Point> {
                                            new Point { Label = "Upfront payment", HexColor = "0xffbdbdbd", Value = loanRequest.DownPayment, LabelHexColor = "0xffa5a191" },
                                            new Point { Label = "25 Years savings", HexColor = "0xffbdbdbd", Value = loanResponse.TotalSavings, LabelHexColor = "0xffa5a191" }
                                        }
                                    }
                                }
                }.ToString();
                ret.BarChart3 = new Chart
                {
                    Type = ChartType.Column,
                    ValueLabelFormat = "C3",
                    Series = new List<Series>
                                {
                                    new Series {
                                        LabelAlignment = "Top",
                                        Points = new List<Point> {
                                            new Point { Label = "Utility Power", HexColor = "0xffbdbdbd", Value = electricityRateWithoutSolar, LabelHexColor = "0xffa5a191" },
                                            new Point { Label = "Solar Power", HexColor = "0xfffddb33", Value = averageSolarElectricityRate, LabelHexColor = "0xffa5a191" }
                                        }
                                    }
                                }
                }.ToString();

                var table1 = new Table
                {
                    Rows = new List<Row>
                                {
                                    new Row(new Cell("Year", VisibleBorders.Bottom),
                                            new Cell("Utility Bill Savings from Solar", VisibleBorders.Bottom | VisibleBorders.Right),
                                            new Cell("Loan Payment", VisibleBorders.Bottom),
                                            new Cell("Benefits and Incentives", VisibleBorders.Bottom),
                                            new Cell("Annual Savings", VisibleBorders.Bottom)) { IsHeader = true }
                                }
                };

                foreach (var year in loanResponse.Years.OrderBy(y => y.Year))
                {
                    table1.Rows.Add(new Row(
                        new Cell(year.Year.ToString()),
                        new Cell((year.ElectricityBillWithoutSolar - year.ElectricityBillWithSolar).ToString("C2", negativeCurrencyMinusCulture), VisibleBorders.Right),
                        new Cell(year.PaymentAmount.ToString("C2", negativeCurrencyMinusCulture)),
                        new Cell(year.TotalBenefitsAndIncentives.ToString("C2", negativeCurrencyMinusCulture)),
                        new Cell(year.Savings.ToString("C2", negativeCurrencyMinusCulture))
                        ));
                }

                table1.Rows.Add(new Row(
                        new Cell("Total", VisibleBorders.Top),
                        new Cell((loanResponse.TotalElectricityBillWithoutSolar - loanResponse.TotalElectricityBillWithSolar).ToString("C2", negativeCurrencyMinusCulture), VisibleBorders.Top),
                        new Cell(loanResponse.SolarSystemCost.ToString("C2", negativeCurrencyMinusCulture), VisibleBorders.Top),
                        new Cell(loanResponse.TotalBenefitsAndIncentives.ToString("C2", negativeCurrencyMinusCulture), VisibleBorders.Top),
                        new Cell(loanResponse.TotalSavings.ToString("C2", negativeCurrencyMinusCulture), VisibleBorders.Top)
                        ));

                ret.Table1 = table1.ToString();

                var tableCostsOverTimeGeneric = new Table
                {
                    Rows = new List<Row>
                    {
                        new Row(
                            new Cell("Year", VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                            new Cell("Production in kWh", VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                            new Cell("Estimated Utility Bill Post Solar Only", VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                            new Cell("Estimated Lease Payments Only", VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                            new Cell("Estimated Annual Savings", VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                            new Cell("Estimated Cumulative Annual Savings", VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right))
                    }
                };

                decimal cummulativeAnnualSavings = 0;
                foreach (var year in loanResponse.Years.OrderBy(y => y.Year))
                {
                    cummulativeAnnualSavings += year.Savings;

                    tableCostsOverTimeGeneric.Rows.Add(new Row(
                        new Cell(year.Year.ToString(), VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                        new Cell(year.SystemProduction.ToString("0.##"), VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                        new Cell(year.ElectricityBillWithSolar.ToString("0.##"), VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                        new Cell(year.PaymentAmount.ToString("0.##"), VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                        new Cell(year.Savings.ToString("0.##"), VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right),
                        new Cell(cummulativeAnnualSavings.ToString("0.##"), VisibleBorders.Top | VisibleBorders.Bottom | VisibleBorders.Left | VisibleBorders.Right)));
                }

                ret.TableCostsOverTimeGeneric = tableCostsOverTimeGeneric.ToString();

            }// todo: handle PPA, morgage etc.

            return ret;
        }

        public List<MergeField> ToMergeDictionary(List<string> mergeFields)
        {
            mergeFields.Add("webhook_default_payload");
            mergeFields.Add("webhook_request_payload");
            mergeFields.Add("webhook_solcius_payload");

            List<MergeField> ret = new List<MergeField>();

            PropertyInfo[] props = this.GetType().GetProperties();

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttribute<MergeAliasAttribute>();
                if (attr != null && mergeFields.Contains(attr.Alias))
                {
                    var value = prop.GetValue(this);
                    ret.Add(new MergeField() { ContentType = ContentType.String, Name = attr.Alias, Value = value != null ? value.ToString() : "" });
                }
            }

            return ret;
        }
    }
}