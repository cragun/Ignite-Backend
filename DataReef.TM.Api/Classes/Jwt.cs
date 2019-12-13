using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes
{
    public class Jwt
    {
        public long Expiration { get; set; }
        public string Token { get; set; }
    }
}