using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.NetSuite
{
    public class Quote
    {
        public string PPARate                   { get; set; }

        public int Year1Production              { get; set; }

        public decimal CurrentUtilityCost       { get; set; }

        public decimal CustomerPrepayment       { get; set; }

        public string PeriodicRentEscalation    { get; set; }

        public string PostSolarTariff           { get; set; }

        public decimal PostSolarUtilityCost     { get; set; }

        public string PreSolarTariff            { get; set; }

        public string ProposalID                { get; set; }

        public string SubstantialCompletionDate { get; set; }

        public string UpfrontRebateAssumptions  { get; set; }
         
        public string UtilityIndex              { get; set; }

        public int YearlyUsage                  { get; set; }

        public decimal Rebate                   { get; set; }

        public string lastYear                  { get; set; }

        public string Credits                   { get; set; }

        public string MasterTariffId            { get; set; }

        //public string EFCostPerWatt { get; set; }

        //public string MasterLeasePaymentEscalationRate { get; set; }

    }
}
