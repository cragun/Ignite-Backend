using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.FinanceAdapters.SST
{
    [DataContract]
    public class SstResponse
    {
        [DataMember]
        public SstResponseMessage Message { get; set; }

        [DataMember]
        [JsonProperty("new_lead")]
        public SstResponseNewLead NewLead { get; set; }

        [JsonProperty("new_appointment")]
        public SSTResponseNewAppointment NewAppointment { get; set; }

        public long? GetNewLeadID()
        {
            if (long.TryParse(NewLead?.Lead_ID, out long result))
            {
                return result;
            }
            return null;
        }
    }

    [DataContract]
    public class SstResponseMessage
    {
        [DataMember]
        public string text { get; set; }

        [DataMember]
        public string type { get; set; }
    }

    [DataContract]
    public class SstResponseNewLead
    {
        [DataMember]
        public string Lead_ID { get; set; }
    }

    public class SSTResponseNewAppointment
    {
        [JsonProperty("appointment_id")]
        public string AppointmentID { get; set; }
    }
}
