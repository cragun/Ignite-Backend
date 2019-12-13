using Microsoft.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Thinktecture.IdentityServer.Core.Logging;

namespace DataReef.Auth.IdentityServer.Helpers
{
    /// <summary>
    /// Detects whether the client requesting an access token is using JSON in the request body instead of form url encoded values
    /// In case JSON is used, a conversion to form url encoded values is made so that the call to token enpoint to be successful
    /// </summary>
    internal class JsonTokenClientDetector : OwinMiddleware
    {
        ILog _logger;

        public JsonTokenClientDetector(OwinMiddleware next) :
            base(next) 
        {
            _logger = LogProvider.GetLogger("SmartLogger");
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value != "/connect/token")
            {
                await Next.Invoke(context);
                return;
            }
            if (context == null || context.Request == null || context.Request.ContentType == null)
            {
                await Next.Invoke(context);
                return;
            }

            if (context.Request.ContentType.Contains("application/json"))
            {
                try
                {
                    byte[] bodyContent = StreamHelper.ReadStream(context.Request.Body);

                    string jsonContent = UTF8Encoding.Default.GetString(bodyContent);
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                    string formUrlEncodedContent = String.Join("&", dictionary.Select(v => string.Concat(v.Key, "=", v.Value)));

                    context.Request.ContentType = "application/x-www-form-urlencoded";
                    context.Request.Body = new MemoryStream(UTF8Encoding.Default.GetBytes(formUrlEncodedContent));
                }
                catch (Exception ex)
                {
                    _logger.ErrorException("Failed to process JSON request body.", ex);
                }
            }

            await Next.Invoke(context);
        }
    }
}