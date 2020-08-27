using System;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{

    [DataContract]
    [Flags]
    public enum PropertyBagName
    {
        //[EnumMember]
        //Own or Rent = 0,

        //[EnumMember]
        //Length of Residence = 1 ,

        [EnumMember]
        Manager = 2,

        [EnumMember]
        Member = 3,

        [EnumMember]
        Installer = 4,

        [EnumMember]
        PhotosManager = 5,

        [EnumMember]
        FranchiseManager = 6,

        [EnumMember]
        SuperAdmin = 7,

        [EnumMember]
        PhotosAdmin = 8
    }
}