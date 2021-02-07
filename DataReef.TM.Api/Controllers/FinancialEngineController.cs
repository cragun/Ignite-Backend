using DataReef.Engines.FinancialEngine.Loan;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/v1/financial")]
    public class FinancialEngineController : ApiController
    {
        private readonly IFinancingCalculator loanCalculator;
        private readonly IFinancePlanDefinitionService financePlanDefinitionService;
        private readonly Lazy<IFinancialService> _financialService;

        public FinancialEngineController(IFinancingCalculator loanCalculator,
                                         IFinancePlanDefinitionService financePlanDefinitionService,
                                         Lazy<IFinancialService> financialService)
        {
            this.loanCalculator = loanCalculator;
            this.financePlanDefinitionService = financePlanDefinitionService;
            _financialService = financialService;
        }


        [Route("{financePlanId:guid}/calculate")]
        [HttpPost]
        [ResponseType(typeof(LoanResponse))]
        public async Task<IHttpActionResult> CalculateLoan(Guid financePlanId, LoanRequest request)
        {
            var data = financePlanDefinitionService.Get(financePlanId, "Details");
            LoanResponse response = null;
            switch (data.Type)
            {
                default:
                case FinancePlanType.Loan:
                    response = loanCalculator.CalculateLoan(request, data);
                    break;
                case FinancePlanType.Lease:
                    response = loanCalculator.CalculateLease(request);
                    break;
            }

            return Ok(response);
        }

        [Route("compare")]
        [HttpPost]
        [ResponseType(typeof(List<FinanceEstimate>))]
        public async Task<IHttpActionResult> CompareFinanceOptions(FinanceEstimateComparisonRequest request)
        {
            // load Plan Definitions data from database
            //var plans = financePlanDefinitionService
            //                .GetPlansForRequest(request)
            //                .ToList();

            //if (request.SortAndFilterResponse)
            //{
            //    plans = plans
            //                .OrderBy(p => request.FinancePlanGuids.IndexOf(p.Guid))
            //                .ToList();
            //}
            //else
            //{
            //    plans = plans
            //            .OrderBy(pd => ComparisonOrder[pd.Type])
            //            .ThenBy(pd => pd.ProviderID)
            //            .ThenBy(pd => pd.TermInYears)
            //            .ToList();
            //}

            //return Ok(loanCalculator.CompareFinancePlans(plans, request));
            return Ok(_financialService.Value.CompareFinancePlans(request));
        }

        [Route("mortgage/termsAndRates")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMortgageMetaData()
        {
            return Ok(MortgageTermsAndRates.Select(t => new { Term = t.Key, Rate = t.Value }));
        }

        /// <summary>
        /// Quick way of sorting results on comparison. Will probably switch to attributes on FinancePlanType elements to be cleaner.
        /// </summary>
        private Dictionary<FinancePlanType, int> ComparisonOrder = new Dictionary<FinancePlanType, int>
        {
            { FinancePlanType.None, 1 },
            { FinancePlanType.Cash, 2 },
            { FinancePlanType.Mortgage, 3 },
            { FinancePlanType.Loan, 4 },
            { FinancePlanType.Lease, 5 },
            { FinancePlanType.PPA, 6 },
            { FinancePlanType.Pace, 7 },
            { FinancePlanType.ReverseMortgage, 8 },
        };

        private static Dictionary<int, double> MortgageTermsAndRates = new Dictionary<int, double>
        {
            { 5, 4.25},
            { 10, 4.35},
            { 15, 4.45},
            { 20, 4.55},
            { 25, 4.65},
            { 28, 4.72 },
            { 30, 4.75}
        };

    }
}
