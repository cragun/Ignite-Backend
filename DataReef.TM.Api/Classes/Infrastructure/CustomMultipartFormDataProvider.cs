using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace DataReef.TM.Api.Classes.Infrastructure
{
    public class CustomMultipartFormDataProvider : MultipartFormDataRemoteStreamProvider
    {
        public override RemoteStreamInfo GetRemoteStream(HttpContent parent, HttpContentHeaders headers)
        {
            return new RemoteStreamInfo(
                remoteStream: new MemoryStream(),
                location: string.Empty,
                fileName: string.Empty);
        }
    }
}