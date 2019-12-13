using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace DataReef.TM.Api.Classes.Infrastructure
{
    public class InMemoryMultipartFormDataStreamProvider : MultipartStreamProvider
    {
        private NameValueCollection _formData = new NameValueCollection();
        private List<RequestFile> _fileContents = new List<RequestFile>();

        // Set of indexes of which HttpContents we designate as form data
        private List<RequestPart> _parts = new List<RequestPart>();

        /// <summary>
        /// Gets a <see cref="NameValueCollection"/> of form data passed as part of the multipart form data.
        /// </summary>
        public NameValueCollection FormData
        {
            get { return _formData; }
        }

        /// <summary>
        /// Gets list of <see cref="HttpContent"/>s which contain uploaded files as in-memory representation.
        /// </summary>
        public List<RequestFile> Files
        {
            get { return _fileContents; }
        }


        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            // For form data, Content-Disposition header is a requirement
            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
            if (contentDisposition != null)
            {
                _parts.Add(new RequestPart
                {
                    Name = contentDisposition.Name,
                    FileName = contentDisposition.FileName
                });

                return new MemoryStream();
            }

            // If no Content-Disposition header was present.
            throw new InvalidOperationException("Did not find required 'Content-Disposition' header field in MIME multipart body part..");
        }

        /// <summary>
        /// Read the non-file contents as form data.
        /// </summary>
        /// <returns></returns>
        public override async Task ExecutePostProcessingAsync()
        {
            // Find instances of non-file HttpContents and read them asynchronously
            // to get the string content and then add that as form data
            for (int index = 0; index < Contents.Count; index++)
            {
                var part = _parts[index];
                if (!part.IsFile())
                {
                    HttpContent formContent = Contents[index];
                    // Extract name from Content-Disposition header. We know from earlier that the header is present.
                    ContentDispositionHeaderValue contentDisposition = formContent.Headers.ContentDisposition;
                    string formFieldName = UnquoteToken(contentDisposition.Name) ?? String.Empty;

                    // Read the contents as string data and add to form data
                    string formFieldValue = await formContent.ReadAsStringAsync();
                    FormData.Add(formFieldName, formFieldValue);
                }
                else
                {
                    _fileContents.Add(new RequestFile
                    {
                        Name = UnquoteToken(part.Name),
                        Content = Contents[index]
                    });
                }
            }
        }

        /// <summary>
        /// Remove bounding quotes on a token if present
        /// </summary>
        /// <param name="token">Token to unquote.</param>
        /// <returns>Unquoted token.</returns>
        private static string UnquoteToken(string token)
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }

    public class RequestFile
    {
        public string Name { get; set; }
        public HttpContent Content { get; set; }
    }

    public class RequestPart
    {
        public string Name { get; set; }
        public string FileName { get; set; }

        public bool IsFile()
        {
            return !string.IsNullOrWhiteSpace(FileName);
        }
    }
}