using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.DTOs.Solar.Finance;

namespace DataReef.TM.Models.Finance
{
    public partial class FinancePlanDefinition
    {
        public void SetMortgageDetails(MortgageDetails mortgage)
        {
            var detail = new FinanceDetail()
            {
                Apr = (float)mortgage.Apr,
                Months = mortgage.Term,
                Name = "Mortgage Principal and Interest",
                PrincipalType = PrincipalType.Loan,
                InterestType = InterestType.Regular,
                ApplyReductionAfterPeriod = ReductionType.TakeHome
            };
            Details = new List<FinanceDetail> { detail };
            TermInYears = mortgage.Term / 12;
        }

        public static FinancePlanDefinition FromCashDetails()
        {
            FinancePlanDefinition def = new FinancePlanDefinition();
            def.Name = "Cash";
            def.Details = new List<FinanceDetail>();
            def.Type = Enums.FinancePlanType.Cash;
            return def;

        }

    }

}
