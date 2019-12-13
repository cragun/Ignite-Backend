using DataReef.Core.Attributes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models.DataViews.Geo;
using DataReef.TM.Services.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using DataReef.Auth.Helpers;
using DataReef.TM.Models.DTOs.Mortgage;
using Microsoft.Practices.ServiceLocation;

namespace DataReef.TM.Services.InternalServices.Geo
{
    [Service(typeof(IGeoProvider))]
    public class DataReefGeoProvider : IGeoProvider
    {
        private readonly IAppSettingService _settingsService;
        private readonly string kBaseAddress = ConfigurationManager.AppSettings["DataReef.Geo.Url"] ?? "https://tm-geo-live.datareef.com/";
        private const string ClientSecret = "asdfjkl;qweruiop";
        //private readonly string kBaseAddress = " http://localhost:8427/";

        public DataReefGeoProvider()
        {
            _settingsService = ServiceLocator.Current.GetInstance<IAppSettingService>();
        }

        public long PropertiesCountInShapeIds(List<Guid> shapeIds)
        {
            if (shapeIds == null || shapeIds.Count == 0) return 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(kBaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AddAuthorizationHeaders(client);

                    var request = new ShapeCollectionRequest { ShapeIDs = shapeIds };

                    HttpResponseMessage response = client.PostAsJsonAsync("api/v2/data/properties/count", request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsAsync<ShapeSummary>().Result.ResidentCount;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("ERROR: {0}", ex.Message));
                return 0;
            }
        }

        public long PropertiesCountForWKT(string wkt)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(kBaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AddAuthorizationHeaders(client);

                    var request = new { WellKnownText = wkt };

                    HttpResponseMessage response = client.PostAsJsonAsync("api/v2/data/wkt/properties/count", request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsAsync<ShapeSummary>().Result.ResidentCount;
                    }

                    return 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("ERROR: {0}", ex.Message));
                return 0;
            }
        }

        public List<ShapeSummary> BulkPropertiesCount(ICollection<ShapeCollectionRequest> req)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(kBaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    AddAuthorizationHeaders(client);

                    HttpResponseMessage response = client.PostAsJsonAsync("api/v2/data/properties/countbulk", req).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsAsync<List<ShapeSummary>>().Result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("ERROR: {0}", ex.Message));
                return null;
            }
        }

        /// <summary>
        /// This method calls a Geo WebService to verify if any of the shapeIds are in the Geo children of the <paramref name="shapeId"/>
        /// </summary>
        /// <param name="shapeId"></param>
        /// <param name="shapeIds"></param>
        /// <returns>True if the shape can be deleted, otherwise false</returns>
        public bool CanDeleteShape(Guid shapeId, List<Guid> shapeIds)
        {
            if (shapeIds == null || shapeIds.Count == 0) return true;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(kBaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                AddAuthorizationHeaders(client);

                var request = new { shapeId = shapeId, ChildShapeIds = shapeIds };

                HttpResponseMessage response = client.PostAsJsonAsync("api/v2/shapes/candelete", request).Result;

                if (!response.IsSuccessStatusCode) return false;
                return response.Content.ReadAsAsync<bool>().Result;
            }
        }

        public MortgageSource GetMortgageDetails(MortgageRequest mortgageRequest)
        {
            if (mortgageRequest == null)
                throw new ArgumentNullException(nameof(mortgageRequest));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(kBaseAddress);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                AddAuthorizationHeaders(client);

                var response = client.PostAsJsonAsync("api/v1/mortgage", mortgageRequest).Result;
                if (!response.IsSuccessStatusCode) return null;

                return response.Content.ReadAsAsync<MortgageSource>().Result;
            }
        }

        private static void AddAuthorizationHeaders(HttpClient client)
        {
            var salt = CryptographyHelper.GenerateSalt();
            var keyString = $"{ClientSecret}{salt}";
            var computedHashKey = Sha256(keyString);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", computedHashKey);
            client.DefaultRequestHeaders.Add("DataView", salt);
        }

        static string Sha256(string token)
        {
            var crypt = new SHA256Managed();
            string hash = string.Empty;
            var crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(token), 0, Encoding.ASCII.GetByteCount(token));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
    }
}