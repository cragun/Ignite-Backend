using DataReef.Core.Attributes;
using DataReef.Integrations.Geo.DataViews;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Integrations.Common.Geo;

namespace DataReef.Integrations
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IGeographyBridge))]

    public class GeographyBridge : IGeographyBridge
    {
        private static readonly string url = System.Configuration.ConfigurationManager.AppSettings["DataReef.Geo.Url"];

        private RestClient client
        {
            get
            {
                return new RestClient(url);
            }
        }

        public ICollection<Property> GetPropertiesForWellKnownText(string wkt, int? requestSize = null, int? requestPage = null, List<string> exludedLocationIds = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ApplicationException("Missing Geo Url in config");
            }

            string resource = "api/v3/data/properties";
            var request = new RestRequest(resource, Method.POST);

            PropertiesRequest pr = new PropertiesRequest();
            pr.AreaWellKnownText = wkt;
            pr.OnlyActive = true;
            pr.RequestSize = requestSize;
            pr.RequestPage = requestPage;
            pr.ExcludedLocationIDs = exludedLocationIds;
            string body = JsonConvert.SerializeObject(pr);

            request.AddParameter("application/json", body, ParameterType.RequestBody);
            request.AddDataReefAuthHeader();

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"GetProperties Failed. {response.Content}");
            }

            var content = response.Content; // raw content as string

            List<Property> ret = JsonConvert.DeserializeObject<List<Property>>(content);

            return ret;

        }

        public ICollection<Property> GetProperties(List<PropertiesRequest> propertiesRequests)
        {
            if (propertiesRequests == null) throw new ArgumentException(nameof(propertiesRequests));

            string resource = "api/v3/data/properties/batch";
            var request = new RestRequest(resource, Method.POST);

            var body = JsonConvert.SerializeObject(propertiesRequests);
            request.AddParameter("application/json", body, ParameterType.RequestBody); 
            request.AddDataReefAuthHeader();

            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException($"GetProperties Failed. {response.Content}");
            }

            var content = response.Content;
            var ret = JsonConvert.DeserializeObject<List<Property>>(content);

            return ret;
        }

        public HighResImage GetHiResImageByLatLon(double lat, double lon)
        {
            var request = new RestRequest("api/v1/imagery/exists", Method.GET);
            request.AddQueryParameter("lat", lat.ToString());
            request.AddQueryParameter("lon", lon.ToString());
            request.AddDataReefAuthHeader();

            var response = client.Execute<HighResImage>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            return null;
        }


        public List<Shape> GetShapesForStates(List<string> states)
        {
            string resource = "api/v2/shapes/states";
            var request = new RestRequest(resource, Method.POST);
            request.AddJsonBody(states);
            request.AddDataReefAuthHeader();

            var response = client.Execute<List<Shape>>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ApplicationException("Get Shapes for States failed");
            }
            return response.Data;
        }

        public HighResImage GetHiResImageById(Guid id)
        {
            var request = new RestRequest($"api/v1/imagery/{id}", Method.GET);
            request.AddDataReefAuthHeader();

            var response = client.Execute<HighResImage>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }
            return null;
        }

        public string SaveHiResImagetest(HighResImage image)
        {
            try
            {
                var request = new RestRequest($"api/v1/imagery", Method.POST);
                request.AddJsonBody(image);
                request.AddDataReefAuthHeader();

                var response = client.Execute(request);
                return response.ResponseStatus.ToString() + "test" + request.JsonSerializer.ToString() ;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
        public void SaveHiResImage(HighResImage image)
        {
            var request = new RestRequest($"api/v1/imagery", Method.POST);
            request.AddJsonBody(image);
            request.AddDataReefAuthHeader();

            var response = client.Execute(request);
        }

        public void MigrateHiResImage(HighResImage image)
        {
            var request = new RestRequest($"api/v1/imagery/migrate", Method.POST);
            request.AddJsonBody(image);
            request.AddDataReefAuthHeader();

            client.Execute(request);
        }

        public void InsertShapes(List<Shape> shapes)
        {
            var request = new RestRequest($"api/v2/shapes/insert/bulk", Method.POST);
            request.AddDataReefAuthHeader();
            request.AddJsonBody(shapes);

            var response = client.Execute<HighResImage>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }
            return;
        }

    }
}
