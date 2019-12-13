using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Microsoft.PowerBI.Models
{
    public class PBI_NewUser : PBI_Base
    {
        [JsonProperty("UserID")]
        public string UserID { get; set; }

        [JsonProperty("InviterID")]
        public string InviterID { get; set; }

        [JsonProperty("UserEmail")]
        public string UserEmail { get; set; }

        [JsonProperty("OUID")]
        public string OUID { get; set; }

        [JsonProperty("OUName")]
        public string OUName { get; set; }

        [JsonProperty("RoleID")]
        public string RoleID { get; set; }

        [JsonProperty("InvitationID")]
        public string InvitationID { get; set; }

        [JsonProperty("JoinDate")]
        public DateTime JoinDate { get; set; }

        [JsonProperty("InvitationDate")]
        public DateTime InvitationDate { get; set; }


    }
}
