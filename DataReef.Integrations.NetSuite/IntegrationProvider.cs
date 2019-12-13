using DataReef.Integrations.Core;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.Integrations.NetSuite
{
    	public class IntegrationProvider : IntegrationProviderBase
		{
			private string sunEdCustId                                   = String.Empty;
			private readonly string partnerId                            = "partnerid";
			private readonly string loginResource                        = @"/api/login";
			private readonly string createHomeOwnersResource             = @"/api/homeowners";
			private readonly string getHomeOwnersByPartnerIdResource     = @"/api/homeowners/";
			private readonly string updateHomeOwnerResource              = @"/api/homeowners/";
			private readonly string ppaResource                          = @"/api/finprog_v2";
            
  		    public string ContractID { get; set; }

			public string Contract { get; set; }

            public IntegrationProvider(string serviceUrl)
                : base(serviceUrl)
            {

            }

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
			/// If there is no homeowner with the passed email, one is created, otherwise the existing one is updated.
			/// </summary>
			/// <param name="owner">The homeowner.</param>
			/// <param name="oktaSessionId">The session id.</param>
			/// <returns>The homeowner with his SunEdCustId.</returns>
			public HomeOwner SaveOrUpdateHomeOwner(HomeOwner owner, string oktaSessionId = "")
			{
				oktaSessionId = String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId;

				if (!String.IsNullOrWhiteSpace(owner.SunEdCustId))
				{
					UpdateHomeOwner(owner, oktaSessionId);
				}
				else
				{
					CreateHomeOwnerResponse response = SaveHomeOwner(owner, oktaSessionId);
					owner.SunEdCustId                = response.SunEdCustId;
				}

				this.sunEdCustId = owner.SunEdCustId;

				return owner;
			}

			/// <summary>
			/// Save homeowner.
			/// </summary>
			/// <param name="owner">The homeowner.</param>
			/// <param name="oktaSessionId">The session id.</param>
			/// <returns>The homeowner with his SunEdCustId.</returns>
			public CreateHomeOwnerResponse SaveHomeOwner(HomeOwner owner, string oktaSessionId = "")
			{
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{ oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
				};

				owner.CampaignSubcategory           = CampaignSubCategory.Blackbird.ToString();
				owner.SunEdCustId                   = owner.SunEdCustId ?? String.Empty;
                CreateHomeOwnerResponse response    = MakeRequest<CreateHomeOwnerResponse>(serviceUrl, createHomeOwnersResource,
																									Method.POST, headers, owner);

				this.sunEdCustId = response != null ? response.SunEdCustId : String.Empty;

				return response;

			}

			/// <summary>
			/// Updates an existing homeowner.
			/// </summary>
			/// <param name="owner">The homeowner.</param>
			/// <param name="oktaSessionId">The session id.</param>
			/// <returns>Returns an UpdateHomeOwnerResponse object.</returns>
			public UpdateHomeOwnerResponse UpdateHomeOwner(HomeOwner owner, string oktaSessionId = "")
			{
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
				};

                UpdateHomeOwnerResponse response = MakeRequest<UpdateHomeOwnerResponse>(serviceUrl,
									String.Format("{0}{1}", updateHomeOwnerResource, owner.SunEdCustId), 
                                    Method.PUT, headers, owner);

				return response;
			}


			/// <summary>
			/// Gets the homeowners of a partner.
			/// </summary>
			/// <param name="owner">The homeowner.</param>
			/// <param name="oktaSessionId">The session id.</param>
			/// <returns>List of homeowners.</returns>
			public List<HomeOwner> GetHomeOwnersByPartnerId(HomeOwner owner, string oktaSessionId = "")
			{
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{ oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
				};

				List<Parameter> parameters = new List<Parameter> { new Parameter {
									Type   = ParameterType.HttpHeader,
									Name   = partnerId,
									Value  = owner.PartnerId,
								}
				};


                List<HomeOwner> response = MakeRequest<List<HomeOwner>>(serviceUrl, getHomeOwnersByPartnerIdResource, Method.GET, 
															  headers, parameters: parameters);


				return response;
			}



			/// <summary>
			/// Returns the financial calculations details including monthly payments, rate, term etc. for PPA finance
			/// </summary>
			/// <param name="request">PPAPricingRequest request object.</param>
			/// <param name="oktaSessionId">The session Id.</param>
			/// <returns>PPAPricingResponse response object.</returns>
			public PPAPricingResponse GetPPAPricing(PPAPricingRequest request, string oktaSessionId = "")
			{
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{ oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
				};
				
				request.SunEdCustId         = String.IsNullOrWhiteSpace(request.SunEdCustId) ? sunEdCustId : request.SunEdCustId;
                PPAPricingResponse response = MakeRequest<PPAPricingResponse>(serviceUrl, ppaResource, Method.POST, headers, request);
                if (response != null)
                {
                    pricingQuoteId            = response.PricingQuoteId;
                }

                return response;
			}


			/// <summary>
			/// Gets a valid SalesPersonId for a PartnerId.
			/// </summary>
			/// <param name="partnerId">The PartnerId.</param>
			/// <returns>The SalesPersonId.</returns>
			public string GetValidSalesPersonIdByPartnerId(string partnerId, string oktaSessionId = "")
			{
				List<HomeOwner> owners = GetHomeOwnersByPartnerId(new HomeOwner { PartnerId = partnerId }, oktaSessionId);
				string salesPersonId   = owners.Any() ? owners.First().SalesPersonId : String.Empty;
				return salesPersonId;
			}

		}

}
