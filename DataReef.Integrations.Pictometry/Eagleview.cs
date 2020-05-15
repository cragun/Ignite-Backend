using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using DataReef.Core.Attributes;

namespace DataReef.Integrations.Pictometry
{
    [Service(typeof(IEagleViewService))]
    public class EagleViewService : IEagleViewService
    {
        public List<SearchResult> GetLinksForLatLon(double lat, double lon)
        {
            string token = GetAuthorizationToken();
            List<SearchResult> results = new List<SearchResult>();

            string url = System.Configuration.ConfigurationManager.AppSettings["EagleView.Url.Search"];

            url = url.Replace("{token}", token);
            url = url.Replace("{content}", $"{lat}, {lon}");

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(url);
                JObject obj = JObject.Parse(json);

                if (obj?["response"]?["statusCode"] != null)
                {
                    string statusCode = obj["response"]["statusCode"].ToString();
                    if (statusCode == "0")
                    {
                        if (obj["response"]["images"] != null)
                        {
                            foreach (JProperty jprop in obj["response"]["images"])
                            {
                                var sr = new SearchResult();

                                try
                                {
                                    sr.Date = jprop.First.Children().First()["date"].ToString();
                                }
                                catch { }

                                try
                                {
                                    sr.OrientationString = jprop.First.Children().First()["orientation"].ToString();
                                    sr.Orientation = (ImageOrientation)Enum.Parse(typeof(ImageOrientation), sr.OrientationString);
                                }
                                catch { }

                                try
                                {
                                    sr.Width = int.Parse(jprop.First.Children().First()["imageWidth"].ToString());
                                    sr.Height = int.Parse(jprop.First.Children().First()["imageHeight"].ToString());

                                }
                                catch { }

                                try
                                {
                                    sr.Url = jprop.First.Children().First()["imageResource"].ToString();
                                }
                                catch { }

                                try
                                {
                                    sr.MetadataUrl = jprop.First.Children().First()["metadataResource"].ToString();
                                }
                                catch { }

                                try
                                {
                                    sr.Resolution = int.Parse(jprop.First.Children().First()["resolution"].ToString());
                                }
                                catch { }

                                try
                                {
                                    int searchX = int.Parse(jprop.First.Children().First()["searchPoint"]["x"].ToString());
                                    int searchY = int.Parse(jprop.First.Children().First()["searchPoint"]["y"].ToString());
                                    sr.SearchPoint = new Point(searchX, searchY);
                                }
                                catch { }

                                results.Add(sr);
                            }
                        }
                    }
                }
            }

            return results;
        }

        public Image GetImageForSearchResult(SearchResult result, int width, int height)
        {
            string token = GetAuthorizationToken();

            Image ret;
            string url = $"{result.Url}/{token}/width:{result.Width};height:{result.Height}";
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);
                using (MemoryStream mem = new MemoryStream(data))
                {
                    ret = Image.FromStream(mem);
                }
            }
            return ret;
        }

        public void PopulateMetaDataForSearchResult(SearchResult result)
        {
            string token = GetAuthorizationToken();
            string url = $"{result.MetadataUrl}/{token}/width:{result.Width};height:{result.Height}";

            using (WebClient webClient = new WebClient())
            {
                var json = webClient.DownloadString(url);
                JObject obj = JObject.Parse(json);

                if (obj?["response"]?["worldFile"] != null)
                {
                    // parsing Word file format https://en.wikipedia.org/wiki/World_file

                    result.MapUnitsPerPixelX = double.Parse(obj["response"]["worldFile"]["a"].ToString()); // A: pixel size in the x-direction in map units / pixel
                    result.SkewX = double.Parse(obj["response"]["worldFile"]["b"].ToString()); // B: rotation about x - axis
                    double x = double.Parse(obj["response"]["worldFile"]["c"].ToString()); // C: x - coordinate of the center of the upper left pixel

                    result.SkewY = double.Parse(obj["response"]["worldFile"]["d"].ToString()); // D: rotation about y - axis
                    result.MapUnitsPerPixelY = double.Parse(obj["response"]["worldFile"]["e"].ToString()); // E: pixel size in the y-direction in map units / pixel, almost always negative
                    double y = double.Parse(obj["response"]["worldFile"]["f"].ToString()); // F: y - coordinate of the center of the upper left pixel

                    // todo: consider the case where xSkew and ySkew are not 0 (the image is rotated)

                    result.Top = y;
                    result.Left = x;

                    result.Bottom = y + (result.Height * result.MapUnitsPerPixelY);
                    result.Right = x + (result.Width * result.MapUnitsPerPixelX);
                }
            }
        }

        public byte[] GetImageBytesForSearchResult(SearchResult result, int width, int height)
        {
            byte[] ret = null;
            string token = GetAuthorizationToken();
            string url = $"{result.Url}/{token}/width:{result.Width};height:{result.Height}";
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);
                ret = data;

            }
            return ret;
        }

        private static string GetAuthorizationToken()
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["EagleView.Url.Auth"];
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["EagleView.ApiKey"];

            TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            long ts = (long)Math.Floor(span.TotalSeconds);

            url = url.Replace("{apikey}", apiKey);
            url = url.Replace("{timestamp}", ts.ToString());
            url = url.Replace("{signature}", GetSignature(ts));

            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(url);
                JObject obj = JObject.Parse(json);

                try
                {
                    var jsonlogfile = System.IO.File.ReadAllText("Content/Images/json.json");
                    var jObject = JObject.Parse(jsonlogfile);
                    JArray nots = (JArray)jObject["NoteList"];
                    Random random = new Random();
                    int randomNumber = random.Next(0, 1000);
                    var newNote = "{ 'Id': " + randomNumber + ", 'url': '" + url + "', 'jsonResponse': '" + json + "'}";
                    nots.Add(JObject.Parse(newNote));
                    jObject["NoteList"] = nots;
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jObject, Newtonsoft.Json.Formatting.Indented);
                    System.IO.File.WriteAllText("wwwroot/JsonFile/Notes.json", output);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }

                if (obj?["response"]?["statusCode"] != null)
                {
                    string statusCode = obj["response"]["statusCode"].ToString();
                    if (statusCode == "0")
                    {
                        string token = obj["response"]["token"].ToString();
                        return token;
                    }
                }
            }

            return null;
        }

        private static string GetSignature(long timeStamp)
        {
            string apiKey = System.Configuration.ConfigurationManager.AppSettings["EagleView.ApiKey"];
            string secretKey = System.Configuration.ConfigurationManager.AppSettings["EagleView.SecretKey"];

            // Generate the hash
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            HMACMD5 hmac = new HMACMD5(encoding.GetBytes(secretKey));
            byte[] hash = hmac.ComputeHash(encoding.GetBytes(apiKey + timeStamp));

            // Convert hash to digital signature string
            string signature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            return signature;
        }
    }
}

