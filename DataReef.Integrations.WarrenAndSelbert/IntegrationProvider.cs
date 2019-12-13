using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DataReef.Integrations.WarrenAndSelbert
{
    public class IntegrationProvider
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public IntegrationProvider(string apiKey)
        {
            _apiKey = apiKey;
            _baseUrl = "https://rightsignature.com";
        }

        public AmmortizationTableResponse GetAmmortizationTable(AmmortizationTableRequest request)
        {
            return new AmmortizationTableResponse(ParseResponseAsXML(HttpRequest("POST", "ws.pl", request.ToXml())));
        }

        private XDocument ParseResponseAsXML(HttpWebResponse response)
        {
            using (XmlReader xmlReader = XmlReader.Create(response.GetResponseStream()))
            {
                XDocument xdoc = XDocument.Load(xmlReader);
                xmlReader.Close();
                return xdoc;
            }
        }

        private HttpWebResponse HttpRequest(string method, string path, string body = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(_baseUrl + path);

            request.Headers.Add("X-ws-apikey", _apiKey);
            request.Method = method;

            if (method.Equals("POST"))
            {
                request.ContentType = "text/xml";
                if (body != null)
                {
                    request.ContentLength = body.Length;
                    using (Stream writeStream = request.GetRequestStream())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(body);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            var response = (HttpWebResponse)request.GetResponse();
            return response;
        }
    }
}
