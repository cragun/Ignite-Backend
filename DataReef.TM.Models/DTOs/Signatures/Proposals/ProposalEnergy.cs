using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures.Proposals
{
    public class ProposalEnergy
    {
        public double TotalProduction { get; set; }
        public double TotalConsumption { get; set; }

        /// <summary>
        /// Production and Consumption data for 12 months
        /// </summary>
        public List<ProposalMonthValue> EnergyData { get; set; }

        public ProposalEnergy() { }

        public ProposalEnergy(FinancePlan financePlan, LoanRequest request, LoanResponse response)
        {

            EnergyData = request?
                            .MonthlyPower?
                            .Select(mp => new ProposalMonthValue
                            {
                                MonthIndex = mp.Month,
                                EnergyConsumption = (double)mp.PostAddersConsumptionOrConsumption,
                                EnergyProduction = (double)mp.Production,
                                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(mp.Month),
                                UtilityBillCostPostAdders = mp.PostSolarCost
                            })
                            .ToList();

            var firstYear = response?.Years?.FirstOrDefault();

            if (firstYear != null)
            {
                TotalProduction = firstYear.SystemProduction;
                TotalConsumption = firstYear.NewConsumption;
            }
        }
    }

    public class ProposalMonthValue
    {
        public int MonthIndex { get; set; }
        public string MonthName { get; set; }
        public double EnergyProduction { get; set; }
        public double EnergyConsumption { get; set; }
        public decimal UtilityBillCostPostAdders { get; set; }

    }
}
