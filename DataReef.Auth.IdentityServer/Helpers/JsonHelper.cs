using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;

namespace DataReef.Auth.IdentityServer.Helpers
{
    static class JsonHelper
    {
        internal static ResponseMessageResult JsonResult(HttpStatusCode statusCode, object content)
        {
            HttpResponseMessage response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content))
            };

            return new ResponseMessageResult(response);
        }

        internal static string SerializeObject(object o)
        {
            try
            {
                string serializedObject = JsonConvert.SerializeObject(o);
                return serializedObject;
            }
            catch { }

            return null;
        }
    }
}