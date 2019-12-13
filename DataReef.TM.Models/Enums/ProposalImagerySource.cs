using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum ProposalImagerySource
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        GoogleMaps = 1,
        [EnumMember]
        Pictometry = 2
    }
}
