using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.TM.Api.Classes.Requests
{
    public class TestAPNPushRequest
    {
        public string DeviceToken { get; set; }
        public string Payload { get; set; }

    }
}