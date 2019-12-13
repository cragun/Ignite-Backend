using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Signatures
{
    public class ProposalYearDataView
    {
        public int Year { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal InterestCharge { get; set; }
        public decimal Principal { get; set; }
        public decimal PaymentBalance { get; set; }
        public decimal UnpaidInternalBalance { get; set; }

        #region BenefitsAndIncentives

        /// <summary>
        /// The amount of federal money that are being received 
        /// </summary>
        public decimal FederalTaxIncentive { get; set; }

        /// <summary>
        /// The amount of money that are being received from the states that offer this incentive
        /// </summary>
        public decimal StateTaxIncentive { get; set; }

        /// <summary>
        /// Performance-based incentives
        /// </summary>
        public decimal PBI { get; set; }

        /// <summary>
        /// Solar Renewable Energy Credit
        /// </summary>
        public decimal SREC { get; set; }

        public decimal BenefitsAndIncentives { get; set; }

        #endregion BenefitsAndIncentives

        public decimal SystemProduction { get; set; }
        public decimal Consumption { get; set; }
        public decimal ElectricityBillWithoutSolar { get; set; }
        public decimal ElectricityBillWithSolar { get; set; }
        public decimal Savings { get; set; }
    }
}
