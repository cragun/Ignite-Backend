using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using DataReef.CodeGenerator;

namespace DataReef.Core.Classes
{
    public interface ISyncable
    {
        [NotMapped]
        [NotIncludedInClient]
        [DataMember]
        [JsonIgnore]
        string SyncDomainString { get; set; }

        [NotIncludedInClient]
        [NotMapped]
        [DataMember]
        [JsonIgnore]
        List<string> SyncDomains { get; set; }

        void EnsureSyncDomain(string domain);
        
    }
}
