using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Dynamic;
using DataReef.TM.Models.DataViews.Solar.Proposal;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.Proposals;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class ProposalUnformattedDataView
    {

        public ProposalUnformattedDataView()
        {
            this.Years = new List<ProposalYearDataView>();
            this.Adders = new List<AdderDataView>();
            this.UserDefined = new ExpandoObject();
            this.KeyValues = new List<KeyValueDataView>();
        }

        //public ProposalUnformattedDataView(Proposal proposal, FinancePlan financePlan, ICollection<KeyValue> keyValues = null, List<OUSetting> settings = null)
        public ProposalUnformattedDataView(ProposalUnformattedDataViewConstructor data)
        {
            Years = new List<ProposalYearDataView>();
            this.Adders = new List<AdderDataView>();
            this.UserDefined = new ExpandoObject();
            this.KeyValues = new List<KeyValueDataView>();


            var negativeCurrencyMinusCulture = CultureInfo.CreateSpecificCulture("en-US");
            negativeCurrencyMinusCulture.NumberFormat.CurrencyNegativePattern = 1;
            // this culture displays negative currency numbers as -$#.## instead of ($#.##)

            var mainOccupant = data?.Proposal.Property?.GetMainOccupant();

            if (mainOccupant != null)
            {
                FirstName = mainOccupant.FirstName;
                LastName = mainOccupant.LastName;
            }

            try { PropertyID = data?.Proposal.Property.Guid.ToString().ToUpper(); } catch { }
            PropertyLocatorID = data?.Proposal.Property.ExternalID;
            CustomerFullName = data?.Proposal.NameOfOwner;
            City = data?.Proposal.City;
            State = data?.Proposal.State;
            ZipCode = data?.Proposal.ZipCode;
            Address1 = data?.Proposal.Address;
            Address2 = data?.Proposal.Address2;
            OrganizationName = data?.Proposal.Property.Territory.OU.Name;
            OrganizationId = data.Proposal.Property.Territory.OUID;

            if ((data.FinancePlan?.FinancePlanType == Enums.FinancePlanType.Loan ||
                 data.FinancePlan?.FinancePlanType == Enums.FinancePlanType.Cash)
                && !string.IsNullOrEmpty(data.FinancePlan?.RequestJSON) && !string.IsNullOrEmpty(data.FinancePlan?.ResponseJSON))
            {
                var loanRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanRequest>(data.FinancePlan.RequestJSON);
                var loanResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<LoanResponse>(data.FinancePlan.ResponseJSON);
                var firstYear = loanResponse.Years.First();

                PricePerWatt = loanRequest.PricePerWattASP;
                CurrentMonthlyBill = Math.Round((firstYear.ElectricityBillWithoutSolar / 12), 2);
                MonthlyBillIn25Years = Math.Round((loanResponse.Years.Last().ElectricityBillWithoutSolar / 12), 2);
                TotalElectricityBillWithSolar = Math.Round(loanResponse.TotalElectricityBillWithSolar, 2);
                TotalElectricityBillWithoutSolar = Math.Round(loanResponse.TotalElectricityBillWithoutSolar, 2);
                UtilityInflationRate = (loanRequest.UtilityInflationRate * 1);
                FirstYearProduction = (long)firstYear.SystemProduction;
                TotalProduction = loanResponse.TotalSystemProduction;
                TotalConsumption = loanResponse.TotalConsumption;
                int solarPowerPercentage =
                    (int)
                        Math.Round((data.Proposal.SolarSystem.SystemProduction.Production /
                                    data.Proposal.SolarSystem.SystemProduction.Consumption) * 100);
                SolarPowerPercentage = solarPowerPercentage;
                UtilityPowerPercentage = (100 - solarPowerPercentage);
                TotalSolarPaymentsCost = Math.Round(loanResponse.TotalSolarPaymentsCost, 2);
                TotalBenefitsAndIncentives = Math.Round(loanResponse.TotalBenefitsAndIncentives, 2);
                var totalPaidByUser =
                    Math.Round(
                        loanResponse.TotalSolarPaymentsCost - loanResponse.TotalBenefitsAndIncentives +
                        loanResponse.TotalUpfrontRebate, 2);
                // check if we should add the DownPayment to TotalPayedByUser
                SalesPersonId = data.Proposal.PersonID;
                TotalPaidByUser = totalPaidByUser;
                TotalSavings = Math.Round(loanResponse.TotalSavings, 2);
                YearlySavings = Math.Round((loanResponse.TotalSavings / loanRequest.ScenarioTermInYears), 2);
                var solarElectricityRate = totalPaidByUser / loanResponse.TotalSystemProduction;
                var consumptionOver25Years = loanRequest.ScenarioTermInYears *
                                             loanRequest.MonthlyPower.Sum(mp => mp.Consumption);
                var solarUtilityElectricityRate =
                    Math.Round(
                        loanResponse.TotalElectricityBillWithSolar /
                        (consumptionOver25Years - loanResponse.TotalSystemProduction), 3);
                var averageSolarElectricityRate = consumptionOver25Years == 0 ? 0 : Math.Round((solarElectricityRate * loanResponse.TotalSystemProduction + solarUtilityElectricityRate * (consumptionOver25Years - loanResponse.TotalSystemProduction)) / consumptionOver25Years, 3);
                var electricityRateWithoutSolar = consumptionOver25Years == 0 ? 0 : loanResponse.TotalElectricityBillWithoutSolar / consumptionOver25Years;
                UtilityPowerCost = Math.Round(electricityRateWithoutSolar, 2);
                SolarPowerCost = Math.Round(averageSolarElectricityRate, 2);
                IntroMonthlyPayment = Math.Round(loanResponse.IntroMonthlyPayment, 2);
                MonthlyPayment = Math.Round(loanResponse.MonthlyPayment, 2);
                SolarSystemCost = Math.Round(loanResponse.SolarSystemCost, 2);
                TotalFederalInvestmentTaxCredit = loanResponse.TotalFederalTaxIncentive;
                TotalStateTaxIncentive = loanResponse.TotalStateTaxIncentive;
                TotalPBI = loanResponse.TotalPBI;
                TotalSREC = loanResponse.TotalSREC;
                TotalUpfrontRebate = loanResponse.TotalUpfrontRebate;
                InterestRate = loanResponse.StatedApr;
                LoanTermInYears = (int)Math.Round(loanResponse.FinancingPeriodInMonths / (double)12);
                ScenarioTermInYears = loanRequest.ScenarioTermInYears;
                UtilityName = data.Proposal.Tariff.UtilityName;
                TariffName = data.Proposal.Tariff.Name;
                CurrentAnnualBill = Math.Round(firstYear.ElectricityBillWithoutSolar, 2);
                SystemSize = (loanRequest.SystemSize);
                AnnualSystemDegradation = (loanRequest.Derate);
                DownPayment = loanRequest.DownPayment;
                TotalExtraCosts = loanResponse.TotalAddersCosts;
                AddersPaidByRep = loanResponse.AddersPaidByRep;
                AmountFinanced = loanResponse.AmountFinanced;
                PanelCount = data.Proposal.SolarSystem.PanelCount;
                CustomerPhoneNumber = data.Proposal.Property.GetMainPhoneNumber();
                CustomerEmailAddress = data.Proposal.Property.GetMainEmailAddress();
                EquipmentInfo = data.Proposal.SolarSystem.GetEquipmentInfo();
                Date = data.Proposal.DateCreated;

                if (loanResponse.Years != null)
                {
                    foreach (var year in loanResponse.Years.OrderBy(mm => mm.Year))
                    {
                        ProposalYearDataView mdv = new ProposalYearDataView();
                        mdv.BenefitsAndIncentives = year.TotalBenefitsAndIncentives;
                        mdv.ElectricityBillWithoutSolar = Math.Round(year.ElectricityBillWithoutSolar, 2);
                        mdv.ElectricityBillWithSolar = Math.Round(year.ElectricityBillWithSolar, 2);
                        mdv.FederalTaxIncentive = year.FederalTaxIncentive;
                        mdv.InterestCharge = Math.Round(year.InterestCharge, 2);
                        mdv.PaymentAmount = Math.Round(year.PaymentAmount, 2);
                        mdv.PaymentBalance = Math.Round(year.PaymentBalance, 2);
                        mdv.PBI = year.PBI;
                        mdv.Principal = Math.Round(year.Principal, 2);
                        mdv.Savings = Math.Round(year.Savings, 2);
                        mdv.SREC = Math.Round(year.SREC, 2);
                        mdv.StateTaxIncentive = Math.Round(year.StateTaxIncentive, 2);
                        mdv.SystemProduction = (decimal)year.SystemProduction;
                        mdv.Consumption = (decimal)year.Consumption;
                        mdv.UnpaidInternalBalance = Math.Round(year.UnpaidInternalBalance, 2);
                        mdv.Year = year.Year;
                        Years.Add(mdv);
                    }
                }
            }

            if (data.Proposal.SolarSystem?.RoofPlanes != null && data.Proposal.SolarSystem.RoofPlanes.Any())
            {
                RoofPlanes = data.Proposal.SolarSystem.RoofPlanes.Select(rp => new RoofPlaneDataView
                {
                    RoofPlaneID = rp.Guid,
                    Label = rp.Name,
                    Azimuth = rp.Azimuth,
                    CenterX = rp.CenterX,
                    CenterY = rp.CenterY,
                    CenterLatitude = rp.CenterLatitude,
                    CenterLongitude = rp.CenterLongitude,
                    GenabilitySolarProviderProfileID = rp.GenabilitySolarProviderProfileID,
                    IsManuallyEntered = rp.IsManuallyEntered,
                    ManuallyEnteredPanelsCount = rp.ManuallyEnteredPanelsCount,
                    ModuleSpacing = rp.ModuleSpacing,
                    Pitch = rp.Pitch,
                    Racking = rp.Racking,
                    RowSpacing = rp.RowSpacing,
                    Shading = rp.Shading,
                    Tilt = rp.Tilt,
                    Points = rp.Points.Select(p => new RoofPlanePointDataView(p)).ToList(),
                    Edges = rp.Edges.Select(e => new RoofPlaneEdgeDataView(e)).ToList(),
                    Panels = rp.Panels.Select(p => new RoofPlanePanelDataView(p)).ToList(),
                    Obstructions = rp.Obstructions.Select(o => new RoofPlaneObstructionDataView(o)).ToList(),
                    SolarPanel = new SolarPanelDataView(rp.SolarPanel),
                    Inverter = new InverterDataView(rp.Inverter)
                }).ToList();
            }



            if (data.KeyValues != null && data.KeyValues.Any())
            {
                foreach (var kv in data.KeyValues)
                {

                    ((IDictionary<String, Object>)this.UserDefined)[kv.Key] = kv.Value;
                    //this.UserDefined[kv.Key] = kv.Value;

                    var dataView = KeyValueDataView.FromDbModel(kv);
                    this.KeyValues.Add(dataView);

                }
            }

            if (data.Proposal.SolarSystem?.AdderItems != null && data.Proposal.SolarSystem.AdderItems.Any())
            {
                var adders = data.Proposal.SolarSystem.AdderItems.Where(ai => !ai.IsDeleted).ToList();

                foreach (var adder in adders)
                {
                    var dataView = AdderDataView.FromDbModel(adder);
                    this.Adders.Add(dataView);
                }
            }

            Attachments = data.Proposal?
                            .MediaItems?
                            .Where(mi => !mi.IsDeleted)?
                            .Select(mi => new ProposalMediaItemDataView(mi))?
                            .ToList();

            Tags = data.Proposal.Tags;

            PowerConsumption = data.Proposal?
                                .SolarSystem?
                                .PowerConsumption?
                                .Where(pc => !pc.IsDeleted)?
                                .Select(pc => new PowerConsumptionDataView(pc))?
                                .ToList();

            PowerMetaData = data.Proposal?.SolarSystem?.GetPowerMetaData();

            UtilityID = data.Proposal?.Tariff?.UtilityID;

            Losses = data.Settings.GetValue<List<OrgSettingDataView>>(OUSetting.Solar_Losses);

            PVWattData = data
                            .IntegrationAudits?
                            .Select(a => new GenabilityIntegrationAuditDataView(a))?
                            .ToList();
        }

        public string PropertyID { get; set; }

        public string PropertyLocatorID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CustomerFullName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string OrganizationName { get; set; }

        public Guid OrganizationId { get; set; }

        public string SalesPersonName { get; set; }

        public Guid SalesPersonId { get; set; }

        /// <example>$5.00</example>
        public decimal PricePerWatt { get; set; }

        public int PanelCount { get; set; }

        /// <example>$1,242.05</example>
        public decimal CurrentMonthlyBill { get; set; }

        /// <example>$1,242.05</example>
        public decimal MonthlyBillIn25Years { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalElectricityBillWithSolar { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalElectricityBillWithoutSolar { get; set; }

        /// <example>4.90%</example>
        public double UtilityInflationRate { get; set; }

        /// <example>11,066 kWh</example>
        public long FirstYearProduction { get; set; }

        /// <example>11,066 kWh</example>
        public long TotalProduction { get; set; }

        public double TotalConsumption { get; set; }

        /// <example>89%</example>
        public float SolarPowerPercentage { get; set; }

        /// <summary>100 - SolarPowerPercentage</summary>
        /// <example>11%</example>
        public float UtilityPowerPercentage { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalSolarPaymentsCost { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalBenefitsAndIncentives { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalPaidByUser { get; set; }

        public decimal TotalSavings { get; set; }

        /// <example>$1,242.05</example>
        public decimal YearlySavings { get; set; }

        /// <example>$0.441</example>
        public decimal UtilityPowerCost { get; set; }

        /// <example>$0.323</example>
        public decimal SolarPowerCost { get; set; }

        /// <example>$1,242.05</example>
        public decimal IntroMonthlyPayment { get; set; }

        /// <example>$1,242.05</example>
        public decimal MonthlyPayment { get; set; }

        /// <example>$1,242.05</example>
        public decimal SolarSystemCost { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalFederalInvestmentTaxCredit { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalStateTaxIncentive { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalPBI { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalSREC { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalUpfrontRebate { get; set; }

        /// <example>4.99%</example>
        public decimal InterestRate { get; set; }

        /// <example>20 years</example>
        public int LoanTermInYears { get; set; }

        /// <example>25 years</example>
        public int ScenarioTermInYears { get; set; }

        public string UtilityName { get; set; }

        public string TariffName { get; set; }

        /// <example>$1,242.05</example>
        public decimal CurrentAnnualBill { get; set; }

        ///price in watts
        /// <example>6910</example>
        public long SystemSize { get; set; }

        /// <example>0.5%</example>
        public double AnnualSystemDegradation { get; set; }

        /// <example>$1,242.05</example>
        public decimal DownPayment { get; set; }

        /// <example>$1,242.05</example>
        public decimal TotalExtraCosts { get; set; }

        public decimal AddersPaidByRep { get; set; }

        /// <example>$1,242.05</example>
        public decimal AmountFinanced { get; set; }

        /// <example>
        /// 3 Sunniva 285 W, 22 Sunniva 275 W
        /// SolarEdge Inverter w/ power optimizers
        /// </example>
        public string EquipmentInfo { get; set; }

        /// <summary>MM/dd/yyyy</summary>
        /// <example>08/18/2016</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// Consumption / bill data entered by the sales rep
        /// </summary>
        public PowerMetaDataDataView PowerMetaData { get; set; }

        public string UtilityID { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public string CustomerEmailAddress { get; set; }

        public dynamic UserDefined { get; set; }

        public ICollection<ProposalYearDataView> Years { get; set; }

        public ICollection<RoofPlaneDataView> RoofPlanes { get; set; }

        public ICollection<KeyValueDataView> KeyValues { get; set; }

        public ICollection<AdderDataView> Adders { get; set; }

        public ICollection<ProposalMediaItemDataView> Attachments { get; set; }

        public ICollection<string> Tags { get; set; }

        public ICollection<PowerConsumptionDataView> PowerConsumption { get; set; }

        /// <summary>
        /// Name options: [Solar.SystemLosses.Soiling, Solar.SystemLosses.Snow, Solar.SystemLosses.Mismatch, Solar.SystemLosses.Wiring, Solar.SystemLosses.Connections, Solar.SystemLosses.LightInductedDegradation, Solar.SystemLosses.NamePlateRating, Solar.SystemLosses.Age, Solar.SystemLosses.Availability]
        /// </summary>        
        public ICollection<OrgSettingDataView> Losses { get; set; }

        /// <summary>
        /// Name options: [Genability.CreateAccount, Genability.ChangeAccountProperty, Genability.CalculateSavingAnalysis, Genability.CalculateCost, Genability.GetPriceResult, Genability.GetRecommendedPrice, Genability.GetTarrifs, Genability.GetZipCodeTariffs, Genability.GetUsageProfile, Genability.GetMonthPresolarConsumption, Genability.GetLSEs, Genability.UpsertSolarProfileWithIntegratedPVWatts, Genability.UpsertUsageProfile]
        /// </summary>
        public ICollection<GenabilityIntegrationAuditDataView> PVWattData { get; set; }
    }
}
