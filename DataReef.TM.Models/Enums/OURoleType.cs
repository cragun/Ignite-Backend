using System;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{

    [DataContract]
    [Flags]
    public enum OURoleType
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Owner = 1 << 0,

        [EnumMember]
        Manager = 1 << 1,

        [EnumMember]
        Member = 1 << 2,

        [EnumMember]
        Installer = 1 << 3,

        [EnumMember]
        PhotosManager = 1 << 4,

        [EnumMember]
        FranchiseManager = 1 << 5,

        [EnumMember]
        SuperAdmin = 1 << 6,

        [EnumMember]
        PhotosAdmin = 1 << 7
    }
}