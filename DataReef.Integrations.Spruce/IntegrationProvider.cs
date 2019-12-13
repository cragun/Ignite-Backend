using DataReef.Integrations.Core;
using DataReef.Integrations.Spruce.DTOs;
using RestSharp;
using System;
using System.Linq;

namespace DataReef.Integrations.Spruce
{
    public class IntegrationProvider : IntegrationProviderBase
    {
        public IntegrationProvider(string serviceUrl) : base(serviceUrl)
        {
        }

        /// <summary>
        /// Does a hard credit check through the API from Spruce (Kilowatt Financial)
        /// </summary>
        /// <param name="request">The request containing info for the hard credit check</param>
        /// <returns></returns>
        public QuoteResponse HardCreditCheck(QuoteRequest request, string userName, string password)
        {
            var response = MakeRequest<QuoteResponse>(serviceUrl, Endpoints.HardCreditCheck, Method.POST, null, request, null, false, userName: userName, password: password);

            return response;
        }

        /// <summary>
        /// Prescreens a single person through the API from Spruce (Kilowatt Financial)
        /// </summary>
        /// <param name="request">The request containing info about the prescreened person</param>
        /// <returns></returns>
        public PrescreenResponse Prescreen(PrescreenRequest request, string userName, string password)
        {
            var response = MakeRequest<PrescreenResponse>(serviceUrl, Endpoints.Prescreen, Method.POST, null, request, null, false, userName: userName, password: password);

            return response;
        }

        public LoanResponseSpruce GetLoanFinancePlan(LoanRequestSpruce request, decimal amountFinanced, string userName, string password)
        {
            var requestExternal = LoanRequestSpruceExternal.FromRequest(request, amountFinanced);
            var responseExternal = MakeRequest<LoanResponseSpruceExternal>(serviceUrl, Endpoints.LoanFinancePlan, Method.POST, null, requestExternal, null, false, userName: userName, password: password);
            if (responseExternal.Calculator_Errors != null && responseExternal.Calculator_Errors.Any(e => !e.error.Equals("No Errors", StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new Exception(String.Join(Environment.NewLine, responseExternal.Calculator_Errors.Select(e => e.error).ToArray()));
            }
            var response = LoanCalculator.CalculateResponse(request, responseExternal);
            return response;
        }

        public void GenerateDocuments(GenDocsRequest request, string userName, string password)
        {
            MakeRequest(serviceUrl, Endpoints.GenerateDocuments, Method.POST, null, request, null, false, userName: userName, password: password);
        }
    }
}
