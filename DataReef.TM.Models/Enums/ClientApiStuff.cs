using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{
    [DataContract]
    [Flags]
    public enum EventDomain
    {
        [EnumMember]
        Any =               0,

        [EnumMember]
        User =              1 << 0,

        [EnumMember]
        Organization =      1 << 1,

        [EnumMember]
        Territory =         1 << 2,

        [EnumMember]
        Disposition =       1 << 3,

        [EnumMember]
        Proposal =          1 << 4,

        [EnumMember]
        PrescreenBatch=     1 << 5,

        [EnumMember]
        TokenTransfer =     1 << 6,

        [EnumMember]
        Reminder =          1 << 7,

        [EnumMember]
        Association=        1 << 8,

        [EnumMember]
        Assignment =        1 << 9,


    }

    [DataContract]
    public enum ApiObjectType
    {
        [EnumMember]
        Organization,

        [EnumMember]
        Customer
    }

    [DataContract]
    public enum EventAction
    {
        [EnumMember]
        Created = 0,

        [EnumMember]
        Changed = 1,

        [EnumMember]
        Removed = 2,

    }

}
