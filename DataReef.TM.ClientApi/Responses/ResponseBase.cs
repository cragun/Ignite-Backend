using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.ClientApi.Responses
{
    public abstract class ResponseBase
    {

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }


    }
}