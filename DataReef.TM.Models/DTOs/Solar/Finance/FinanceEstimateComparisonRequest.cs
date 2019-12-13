using DataReef.TM.Models.Finance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.Solar.Finance
{
    public class FinanceEstimateComparisonRequest : LoanRequest
    {
        /// <summary>
        /// List of FinancePlanDefinitionGuids that the user wishes to compare
        /// </summary>
        [Obsolete("Please use: FinancePlanDefinitionData. This will be removed!")]
        public List<Guid> FinancePlanDefinitionGuids { get; set; }

        public List<ComparisonFinancePlanRequestData> FinancePlanDefinitionData { get; set; }

        /// <summary>
        /// This property will trigger results filter and order by FinancePlanDefinitionGuids
        /// </summary>
        public bool SortAndFilterResponse { get; set; }

        public FinanceEstimateComparisonRequest()
        {
            SortAndFilterResponse = false;
        }

        private List<Guid> _financePlanGuids;
        public List<Guid> FinancePlanGuids
        {
            get
            {
                if (_financePlanGuids == null)
                {
                    _financePlanGuids = FinancePlanDefinitionData?.Select(fpd => fpd.Guid)?.ToList() ?? FinancePlanDefinitionGuids;
                }
                return _financePlanGuids;
            }
        }

        public decimal GetDealerFee(Guid planId)
        {
            return FinancePlanDefinitionData?.FirstOrDefault(fpd => fpd.Guid == planId)?.DealerFee ?? 0;
        }

        public string GetPlanData(Guid planId)
        {
            return FinancePlanDefinitionData?.FirstOrDefault(fpd => fpd.Guid == planId)?.Data;
        }
    }
}
