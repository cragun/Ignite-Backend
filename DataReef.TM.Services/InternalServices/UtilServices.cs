using DataReef.Core;
using DataReef.Core.Attributes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DataReef.TM.Services.InternalServices
{
    [Service(typeof(IUtilServices))]
    public class UtilServices : IUtilServices
    {
        public byte[] GetPDF(string url)
        {
            // add pdf=true to the query string.
            // we use this method to make sure it won't brake if in the future we'll use query strings.
            var pdfUrl = GetPDFUrl(url);
            var uri = new Uri(pdfUrl);

            var client = new RestClient(uri.GetLeftPart(UriPartial.Authority));
            var req = new RestRequest(uri.PathAndQuery, Method.GET);
            var resp = client.Get(req);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                return resp.RawBytes;
            }
            return null;
        }

        public string GetPDFUrl(string url)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["pdf"] = "true";
            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();

            var path = HttpUtility.UrlEncode(url);
            var pdfUrl = $"{Constants.UtilsAPIUrl}/api/v1/converter/generate/pdf?url={path}";

            return pdfUrl;
        }
    }
}
