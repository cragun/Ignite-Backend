using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DataReef.TM.Api.CustomResponseTypes
{
    //simple implementation of IHttpActionResult that creates a plain text response
    public class TextResult : IHttpActionResult
    {
        string             _content;
        HttpRequestMessage _request;
        HttpStatusCode     _statusCode;

        public TextResult(string content, HttpRequestMessage request, HttpStatusCode statusCode)
        {
            _content    = content;
            _request    = request;
            _statusCode = statusCode;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_request.CreateResponse(_statusCode, _content));
        }
    }
}