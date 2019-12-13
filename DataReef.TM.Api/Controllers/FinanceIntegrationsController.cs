using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.FinancialIntegration.LoanPal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/financeintegrations")]
    public class FinanceIntegrationsController : ApiController
    {
        private readonly Lazy<ILoanPalAdapter> _loanPalAdapter;
        public FinanceIntegrationsController(Lazy<ILoanPalAdapter> loanPalAdapter)
        {
            _loanPalAdapter = loanPalAdapter;
        }

        [Route("loanpal/submitapplication")]
        [ResponseType(typeof(LoanPalApplicationResponse))]
        [HttpPost]
        public HttpResponseMessage SubmitLoanPalApplication([FromBody]LoanPalApplicationRequest request)
        {
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.Select(m => m.Value.Errors.Select(e => e.ErrorMessage)));

            var response = _loanPalAdapter.Value.SubmitApplication(request);

            return Request.CreateResponse(HttpStatusCode.OK, response);
        }
    }
}