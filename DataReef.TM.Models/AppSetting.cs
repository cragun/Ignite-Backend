using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    public class AppSetting : EntityBase
    {
        public AppSetting() : base()
        {
            DefaultExcludedProperties.Add(nameof(VisibleToClients));
        }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool VisibleToClients { get; set; }

    }
}
