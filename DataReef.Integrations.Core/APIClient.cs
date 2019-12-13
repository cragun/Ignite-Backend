using RestSharp;
using RestSharp.Serializers;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DataReef.Integrations.Core
{
    public class APIClient
    {
        protected const string applicationJsonContentType = "application/json";

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
        public static T MakeRequest<T>(string serviceUrl, string resource, Method method, out string error, Dictionary<string, string> headers = null, object payload = null,
                                  List<Parameter> parameters = null, string contentType = applicationJsonContentType, ISerializer serializer = null) where T : class, new()
        {
            var client = new RestClient(serviceUrl);
            RestRequest request = BuildRequest(resource, method, headers, payload, parameters, contentType, serializer);

            try
            {
                var response = client.Execute<T>(request);
                //CheckForErrors((int)response.StatusCode, response.ErrorMessage, response.Content, resource, serviceUrl);

                var errorHeader = response.Headers.FirstOrDefault(h => h.Name == "X-Error-Detail");

                error = errorHeader?.Value?.ToString();

                return response.Data;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static T MakeRequest<T>(string serviceUrl, string resource, Method method, Dictionary<string, string> headers = null, object payload = null,
                                  List<Parameter> parameters = null, string contentType = applicationJsonContentType, ISerializer serializer = null) where T : class, new()
        {
            string error = null;
            return MakeRequest<T>(serviceUrl, resource, method, out error, headers, payload, parameters, contentType, serializer);
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
        /// <returns>Returns a RestRequest object.</returns>
        public static RestRequest BuildRequest(string resource, Method method, Dictionary<string, string> headers = null, object payload = null,
                                  List<Parameter> parameters = null, string contentType = applicationJsonContentType, ISerializer serializer = null)
        {
            RestRequest request = new RestRequest(resource, method);
            if (serializer != null)
            {
                request.JsonSerializer = serializer;
            }
            request.Timeout = 1000 * 60 * 10; // 10 minutes
            request.RequestFormat = DataFormat.Json;
            request.OnBeforeDeserialization = resp => { resp.ContentType = contentType; };


            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (payload != null)
            {
                request.AddBody(payload);
            }

            if (parameters != null)
            {
                request.Parameters.AddRange(parameters);
            }

            return request;
        }
    }
}
