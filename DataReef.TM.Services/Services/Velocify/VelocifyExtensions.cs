using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DataReef.TM.Services.Services
{
    public static class VelocifyExtensions
    {
        public static HttpResponseMessage PostFormUrlEncodedAsync<T>(this HttpClient httpClient, string url, T obj)
        {
            var data = obj.GetType().GetProperties()
                .Where(p => p.Name != "DefaultExcludedProperties")
                .Select(pi => new { pi.Name, Value = pi.GetValue(obj, null) })
                .Select(pi => new KeyValuePair<string, string>(pi.Name, pi.Value?.ToString() ?? string.Empty))
                .ToList();

            using (var content = new FormUrlEncodedContent(data))
            {
                var response = httpClient.PostAsync(url, content).Result;

                return response;
            }
        }
    }
}
