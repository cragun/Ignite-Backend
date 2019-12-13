using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class MortgageCalculationResponse
    {
        public decimal MonthlyPayment { get; set; }

        public decimal TotalInterest { get; set; }

    }
}
