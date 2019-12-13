using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    public enum AdderItemCalculatedPerType
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        PerRoofPlane,

        [EnumMember]
        PerPanel
    }
}
