using DataReef.Integrations.Core;
using DataReef.Integrations.Mosaic.Models;
using RestSharp;
using System;
using System.Collections.Generic;

namespace DataReef.Integrations.Mosaic
{
    public class IntegrationProvider : IntegrationProviderBase
    {
        private readonly string mosaicHomeOwnersResource        = @"/api/homeowners";
        private readonly string loginResource                   = @"/api/login";
        private readonly string pricingResource                 = @"/api/finprog_v2";
        private readonly string regularFinancingProgram         = "SunEdison Mosaic SCION";
        private string sunEdCustId                              = String.Empty;


        public IntegrationProvider(string serviceUrl)
            : base(serviceUrl)
        {

        }

        public string Contract { get; set; }

        public string ContractID { get; set; }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="userName">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>The session Id.</returns>
        public string Authenticate(string userName, string password)
        {
            return Authenticate(userName, password, loginResource);
        }

        /// <summary>
        /// Updates an existing homeowner.
        /// </summary>
        /// <param name="owner">The homeowner.</param>
        /// <param name="oktaSessionId">The session id.</param>
        /// <returns>Returns an UpdateHomeOwnerResponse object.</returns>
        public UpdateMosaicHomeOwnerResponse UpdateMosaicHomeOwner(MosaicHomeOwner owner, string oktaSessionId = "")
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
		    {
		    	{oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
		    };

            UpdateMosaicHomeOwnerResponse response = MakeRequest<UpdateMosaicHomeOwnerResponse>(serviceUrl,
                                                        String.Format("{0}/{1}", mosaicHomeOwnersResource, owner.SunEdCustId),
                                                        Method.PUT, headers, owner);

            return response;
        }

        /// <summary>
        /// If there is no homeowner with the passed email, one is created, otherwise the existing one is updated.
        /// </summary>
        /// <param name="owner">The homeowner.</param>
        /// <param name="oktaSessionId">The session id.</param>
        /// <returns>The homeowner with his SunEdCustId.</returns>
        public MosaicHomeOwner SaveOrUpdateMosaicHomeOwner(MosaicHomeOwner owner, string oktaSessionId = "")
        {
            oktaSessionId = String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId;

            if (!String.IsNullOrWhiteSpace(owner.SunEdCustId))
            {
                UpdateMosaicHomeOwner(owner, oktaSessionId);
            }
            else
            {
                CreateMosaicHomeOwnerResponse response = SaveMosaicHomeOwner(owner, oktaSessionId);
                owner.SunEdCustId = response.SunEdCustId;
            }

            this.sunEdCustId = owner.SunEdCustId;

            return owner;
        }


        /// <summary>
        /// Save mosaic homeowner.
        /// </summary>
        /// <param name="owner">The homeowner.</param>
        /// <param name="oktaSessionId">The oktaSessionId.</param>
        /// <returns>The mosaic homeowner with his SunEdCustId.</returns>
        public CreateMosaicHomeOwnerResponse SaveMosaicHomeOwner(MosaicHomeOwner owner, string oktaSessionId)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
		    {
		    	{ oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
		    };

            owner.FinancingProgram                  = regularFinancingProgram;
            CreateMosaicHomeOwnerResponse response  = MakeRequest<CreateMosaicHomeOwnerResponse>(serviceUrl, mosaicHomeOwnersResource,
                                                                                                Method.POST, headers, owner);

            this.sunEdCustId = response != null ? response.SunEdCustId : String.Empty;

            return response;

        }


        /// <summary>
        /// Returns the financial calculations details including monthly payments, rate, term etc. for PPA finance
        /// </summary>
        /// <param name="request">PPAPricingRequest request object.</param>
        /// <param name="oktaSessionId">The session Id.</param>
        /// <returns>PPAPricingResponse response object.</returns>
        public LoanPricingResponse GetLoanPricing(LoanPricingRequest request, string oktaSessionId = "")
        {
            Dictionary<string, string> headers = new Dictionary<string, string>
		    {
		    	{ oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
		    };

            request.SunEdCustId             = String.IsNullOrWhiteSpace(request.SunEdCustId) ? sunEdCustId : request.SunEdCustId;
            LoanPricingResponse response    = MakeRequest<LoanPricingResponse>(serviceUrl, pricingResource, Method.POST, headers, request);
            if (response != null)
            {
                pricingQuoteId = response.PricingQuoteId;
            }


            return response;
        }

    }
}
