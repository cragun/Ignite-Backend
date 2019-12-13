using DataReef.Integrations.Core.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using RestSharp.Serializers;

namespace DataReef.Integrations.Core
{
    public class IntegrationProviderBase
    {
        protected string serviceUrl;
        protected string sessionID;
        protected string pricingQuoteId;
        protected Guid OUID;
        protected string sunEdCustId = String.Empty;
        protected const string applicationJsonContentType = "application/json";
        protected readonly string oktaSessionIdHeader = "x-okta-session-id";
        protected string serviceCallErrorMessage = "Service call to address '{0}{1}' failed with the following error: {2}";
        private string PPAandLoanResource = @"/api/finprog_v2";
        protected readonly string contractResource = @"/api/contract";
        protected readonly string docuSignResource = @"/api/docusign";
        protected readonly string createProposalResource = @"/api/proposal_v2";

        public string ErrorMessage { get; private set; }

        public bool IsAuthenticated { get; set; }

        public string Contract { get; set; }

        public string ContractID { get; set; }


        public IntegrationProviderBase(string serviceUrl)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentNullException("serviceUrl");
            }

            this.serviceUrl = serviceUrl;
            this.ErrorMessage = String.Empty;
            this.sessionID = String.Empty;
            this.pricingQuoteId = String.Empty;
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="userName">Username.</param>
        /// <param name="password">Password.</param>
        /// <returns>The session Id.</returns>
        public string Authenticate(string userName, string password, string loginResource)
        {

            sessionID = String.Empty;

            Credential credentials = new Credential()
            {
                username = userName,
                password = password
            };

            AuthenticationToken token = MakeRequest<AuthenticationToken>(serviceUrl, loginResource, Method.POST, payload: credentials, userName: userName, password: password);

            if (token != null && !string.IsNullOrEmpty(token.id))
            {
                sessionID = token.id;
                this.IsAuthenticated = true;
            }

            return sessionID;

        }

        /// <summary>
        /// Gets the signed contract to that particular homeowner using SunEdCustomerId.
        /// </summary>
        /// <param name="request">The DocuSignRequest request object.</param>
        /// <param name="oktaSessionId">The session Id.</param>
        /// <returns>DocuSignResponse response object.</returns>
        public DocuSignResponse DocuSign(DocuSignRequest request = null, string oktaSessionId = "")
        {
            if (request == null)
            {
                request = new DocuSignRequest
                {
                    PricingQuoteId = pricingQuoteId
                };
            }

            Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
                };


            DocuSignResponse response = MakeRequest<DocuSignResponse>(serviceUrl, docuSignResource, Method.POST, headers, request);

            this.ContractID = response != null ? response.envelopeId : String.Empty;

            return response;
        }


        /// <summary>
        /// It's used to create a Contract Document for the homeowner.
        /// </summary>
        /// <param name="request">ContractRequest request object.</param>
        /// <param name="oktaSessionId">The session Id.</param>
        /// <returns>ContractResponse response object.</returns>
        public ContractResponse GetContract(ContractRequest request = null, string oktaSessionId = "")
        {
            if (request == null)
            {
                request = new ContractRequest
                {
                    SunEdCustId = sunEdCustId,
                    PricingQuoteId = pricingQuoteId
                };
            }

            Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
                };

            ContractResponse response = MakeRequest<ContractResponse>(serviceUrl, contractResource,
                                        Method.POST, headers, request, contentType: applicationJsonContentType);

            this.Contract = response != null ? response.Contract : String.Empty;

            return response;
        }

        /// <summary>
        /// Creates a PDF document called as proposal for selected finance option.
        /// </summary>
        /// <param name="request">CreateProposalRequest request object.</param>
        /// <param name="oktaSessionId">The session Id.</param>
        /// <returns>The proposal.</returns>
        public CreateProposalResponse CreateProposal(CreateProposalRequest request = null, string oktaSessionId = "")
        {
            if (request == null)
            {
                request = new CreateProposalRequest
                {
                    PricingQuoteId = pricingQuoteId,
                    ProposalBytes = String.Empty
                };
            }

            Dictionary<string, string> headers = new Dictionary<string, string>
                {
                    { oktaSessionIdHeader, String.IsNullOrWhiteSpace(oktaSessionId) ? sessionID : oktaSessionId }
                };

            string response = MakeRequest(serviceUrl, createProposalResource, Method.PUT, headers, request);

            return new CreateProposalResponse
            {
                proposal = response
            };
        }

        /// <summary>
        /// Makes a REST request.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="resource">The endpoint address.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="headers">Header elements for the request.</param>
        /// <param name="payload">The payload of the request.</param>
        /// <param name="parameters">List of parameters for the request.</param>
        /// <param name="contentType">Content Type of the request.</param>
        /// <returns>Returns the content of the response object.</returns>
        public string MakeRequest(string serviceUrl, string resource, Method method, Dictionary<string, string> headers = null, object payload = null,
                                    List<Parameter> parameters = null, bool replaceNullStringFieldValues = true, string contentType = applicationJsonContentType,
                                    string userName = null, string password = null, ISerializer serializer = null)
        {
            var client = new RestClient(serviceUrl);
            if (userName != null && password != null)
            {
                client.Authenticator = new HttpBasicAuthenticator(userName, password);
            }
            RestRequest request = BuildRequest(resource, method, headers, payload, parameters, replaceNullStringFieldValues, contentType, serializer);

            var response = client.Execute(request);

            CheckForErrors((int)response.StatusCode, response.ErrorMessage, response.Content, resource, serviceUrl);

            return response.Content;
        }

        /// <summary>
        /// Makes a REST request with generic response object.
        /// </summary>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="resource">The endpoint address.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="headers">Header elements for the request.</param>
        /// <param name="payload">The payload of the request.</param>
        /// <param name="parameters">List of parameters for the request.</param>
        /// <param name="contentType">Content Type of the request.</param>
        /// <returns>Returns the content of the response object.</returns>
        public T MakeRequest<T>(string serviceUrl, string resource, Method method, Dictionary<string, string> headers = null, object payload = null,
                                  List<Parameter> parameters = null, bool replaceNullStringFieldValues = true, string contentType = applicationJsonContentType,
                                  string userName = null, string password = null, ISerializer serializer = null) where T : class, new()
        {
            var client = new RestClient(serviceUrl);
            if (userName != null && password != null)
            {
                client.Authenticator = new HttpBasicAuthenticator(userName, password);
            }
            RestRequest request = BuildRequest(resource, method, headers, payload, parameters, replaceNullStringFieldValues, contentType, serializer);

            //Dirty code, to be refactored; for Mosaic Loan and NetSuite PPA request objects
            if (resource.Equals(PPAandLoanResource))
            {
                request.Parameters[1].Value = request.Parameters[1].Value.ToString()
                    .Replace("installer_client_name", "installer.client.name")
                    .Replace("calcMap_dealerName", "calcMap.dealerName")
                    .Replace("calcMap_currentDate", "calcMap.currentDate")
                    .Replace("installer_client_phone", "installer.client.phone");
            }

            try
            {
                var response = client.Execute<T>(request);
                CheckForErrors((int)response.StatusCode, response.ErrorMessage, response.Content, resource, serviceUrl);

                return response.Data;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Builder for the REST request object.
        /// </summary>
        /// <param name="resource">The endpoint address.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="headers">Header elements for the request.</param>
        /// <param name="payload">The payload of the request.</param>
        /// <param name="parameters">List of parameters for the request.</param>
        /// <param name="contentType">Content Type of the request.</param>
        /// <param name="serializer"></param>
        /// <returns>Returns a RestRequest object.</returns>
        public RestRequest BuildRequest(string resource, Method method, Dictionary<string, string> headers = null, object payload = null,
                                  List<Parameter> parameters = null, bool replaceNullStringFieldValues = true, string contentType = applicationJsonContentType, ISerializer serializer = null)
        {
            RestRequest request = new RestRequest(resource, method);
            request.Timeout = 1000 * 60 * 5;
            request.RequestFormat = DataFormat.Json;
            request.OnBeforeDeserialization = resp => { resp.ContentType = contentType; };
            if (serializer != null) request.JsonSerializer = serializer;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (payload != null)
            {
                if (replaceNullStringFieldValues) payload = ReplaceNullStringFieldValues(payload);

                request.AddBody(payload);
            }

            if (parameters != null)
            {
                request.Parameters.AddRange(parameters);
            }

            return request;
        }

        /// <summary>
        /// Handler for a REST response object's error message.
        /// </summary>
        /// <param name="responseStatusCode">The status code of the response.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="content">The content of the response.</param>
        /// <param name="resource">The endpoint address.</param>
        /// <param name="resource">The service URL.</param>
        public void CheckForErrors(int responseStatusCode, string errorMessage, string content, string resource, string serviceUrl)
        {

            if (responseStatusCode == 0 && errorMessage.ToLower().Contains("timed out"))
            {
                this.ErrorMessage = "DataReef Server Timeout";
            }
            else if (responseStatusCode >= (int)HttpStatusCode.BadRequest)
            {
                // known error http code, error is in the content
                this.ErrorMessage = content;
            }
            else if (!string.IsNullOrWhiteSpace(errorMessage) && responseStatusCode != 200)
            {
                // exception message returned
                this.ErrorMessage = string.Format(serviceCallErrorMessage, serviceUrl, resource, errorMessage);
            }

        }

        /// <summary>
        /// Replaces the values of null string fields with an empty string.
        /// </summary>
        /// <param name="obj">The object to be modified.</param>
        /// <returns>The modified object.</returns>
        public object ReplaceNullStringFieldValues(object obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                object value = property.GetValue(obj);
                if (property.PropertyType == typeof(string) && value == null)
                {
                    property.SetValue(obj, String.Empty);
                }
            }

            return obj;
        }
    }
}
