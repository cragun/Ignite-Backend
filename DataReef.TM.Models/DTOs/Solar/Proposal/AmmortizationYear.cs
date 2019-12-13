using DataReef.TM.Models.DTOs.Solar.Finance;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Proposal
{
    public class AmortizationYear : AmortizationRow
    {
        public static List<AmortizationYear> CreateYears(LoanRequestSunEdison request, AmortizationTable table)
        {
            List<AmortizationYear> ret = new List<AmortizationYear>();

            int startYear = table.Rows.First().Year;
            int endYear = table.Rows.Last().Year;

            for (int year = startYear; year <= endYear; year++)
            {
                AmortizationYear ay = new AmortizationYear();
                ay.Year = year;
                ay.Production = table.Rows.Where(r => r.Year == year).Sum(r => r.Production);
                ay.Consumption = table.Rows.Where(r => r.Year == year).Sum(r => r.Consumption);
                ay.CostWithoutSolar = table.Rows.Where(r => r.Year == year).Sum(r => r.CostWithoutSolar);
                ay.SolarOnlyCosts = table.Rows.Where(r => r.Year == year).Sum(r => r.SolarOnlyCosts);
                ay.CostsWithSolarAndElectric = table.Rows.Where(r => r.Year == year).Sum(r => r.CostsWithSolarAndElectric);
                ay.EndingBalance = table.Rows.Where(r => r.Year == year).Last().EndingBalance;
                ay.BeginningBalance = table.Rows.Where(r => r.Year == year).First().BeginningBalance;
                ay.Payment = table.Rows.Where(r => r.Year == year).Sum(r => r.Payment);
                ay.Interest = table.Rows.Where(r => r.Year == year).Sum(r => r.Interest);
                ay.Principal = table.Rows.Where(r => r.Year == year).Sum(r => r.Principal);
                ay.PBI = table.Rows.Where(r => r.Year == year).Sum(r => r.PBI);
                ay.SREC = table.Rows.Where(r => r.Year == year).Sum(r => r.SREC);
                ay.ITC = table.Rows.Where(r => r.Year == year).Sum(r => r.ITC);
                ay.ReinvestedIncentives = table.Rows.Where(r => r.Year == year).Sum(r => r.ReinvestedIncentives);
                ay.TotalPaymentsAndReinvestments = table.Rows.Where(r => r.Year == year).Sum(r => r.TotalPaymentsAndReinvestments);
                ay.TotalCost = table.Rows.Where(r => r.Year == year).Sum(r => r.TotalCost);
                ay.CumulativeCostOfSolar = table.Rows.Where(r => r.Year == year).Last().CumulativeCostOfSolar;
                ret.Add(ay);
            }

            return ret;
        }
    }
}
