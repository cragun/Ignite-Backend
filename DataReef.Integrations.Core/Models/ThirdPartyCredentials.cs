using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Core.Models
{
    public class ThirdPartyCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AppKey { get; set; }
        public string AppID { get; set; }
        public string Url { get; set; }

    }
}
