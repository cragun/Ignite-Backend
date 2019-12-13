using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class MortgageAnalysisRequest
    {
        /// <summary>
        /// From ElasticSearch AVM ( not in there yet )
        /// </summary>
        public decimal HomeValue { get; set; }

        /// <summary>
        /// Reserved for Future Use
        /// </summary>
        public decimal ClosingCostRate { get; set; }

        /// <summary>
        /// The current MOrtgage.  From DEED Index in ES
        /// </summary>
        public MortgageDetails CurrentMortgage { get; set; }

        /// <summary>
        /// Proposed Mortgages.  From IOS, Mortage Calculator
        /// </summary>
        public List<MortgageDetails> ProposedMortgages { get; set; }



    }
}
