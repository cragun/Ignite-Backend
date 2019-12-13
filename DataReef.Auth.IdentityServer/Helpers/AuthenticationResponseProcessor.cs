using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;
using DataReef.Auth.IdentityServer.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataReef.Auth.IdentityServer.Helpers
{
    /// <summary>
    /// Updates the Http status code for failed authentications to include more details (400 => 401, 412)
    /// </summary>
    internal class AuthenticationResponseProcessor : OwinMiddleware
    {
        public AuthenticationResponseProcessor(OwinMiddleware next) :
            base(next)
        {

        }

        public override async Task Invoke(IOwinContext context)
        {
            // Buffer the response
            Stream stream = context.Response.Body;
            MemoryStream buffer = new MemoryStream();
            context.Response.Body = buffer;

            // Add a continuation to the previous step in the pipeline
            await Next.Invoke(context).ContinueWith(async task =>
            {
                if (context.Environment.ContainsKey(AuthConstants.AuthenticationReponseKey))
                {
                    AuthenticationResponse authResponse = context.Get<AuthenticationResponse>(AuthConstants.AuthenticationReponseKey);
                    context.Response.StatusCode = (int)authResponse.StatusCode;

                    // Build the JSON content
                    IDictionary<string, object> content = new Dictionary<string, object>();
                    if (!string.IsNullOrEmpty(authResponse.Message))
                    {
                        content.Add("Message", authResponse.Message);
                    }

                    if (authResponse.Data != null)
                    {
                        content.Add("Data", authResponse.Data);
                    }

                    string serializedContent = JsonConvert.SerializeObject(content);
                    byte[] contentBytes = Encoding.UTF8.GetBytes(serializedContent);

                    // Replace the default response content with the authentication response details
                    context.Response.ContentLength = contentBytes.Length;
                    buffer.Seek(0, SeekOrigin.Begin);
                    await buffer.WriteAsync(contentBytes, 0, contentBytes.Length);
                }

                // Flush the buffer into the stream
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(stream);
            });
        }
    }
}