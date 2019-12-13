using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace DataReef.Auth.IdentityServer.Models
{
    class AuthenticationResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}