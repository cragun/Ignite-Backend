using System;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Enums
{

    [DataContract]
    [Flags]
    public enum PermissionType : long
    {
        [EnumMember]
        None = 0,

        [EnumMember]
        Can_Manage_Organization = 1 << 0,

        [EnumMember]
        Can_Manage_User_Settings = 1 << 1,

        [EnumMember]
        Can_Remove_Users = 1 << 2,

        [EnumMember]
        Can_View_Layers = 1 << 3,

        [EnumMember]
        Can_Manage_Organizations_Territories = 1 << 4,

        [EnumMember]
        Can_See_All_CRM = 1 << 5,

        [EnumMember]
        Can_Change_Disposition_CRM = 1 << 6,

        [EnumMember]
        Can_Access_High_Res_Image = 1 << 7,

        [EnumMember]
        Can_Invite_New_Users = 1 << 8,

        [EnumMember]
        Can_Access_Financing = 1 << 9,

        [EnumMember]
        Can_Report_Progress = 1 << 10,

        [EnumMember]
        Can_Complete_Sales = 1 << 11,

        [EnumMember]
        Can_Set_Appointments = 1 << 12,

        [EnumMember]
        Can_Create_Proposals = 1 << 13,

        [EnumMember]
        Portal_View_Survey = 1 << 14,

        [EnumMember]
        Portal_View_Survey_Reposnse = 1 << 15,

        [EnumMember]
        Portal_View_OU = 1 << 16,

        [EnumMember]
        Portal_View_Photos = 1 << 17,

        [EnumMember]
        Portal_View_Photos_Settings = 1 << 18,

        [EnumMember]
        Portal_All_OU_Settings = 1 << 19,

        [EnumMember]
        Portal_All = 1 << 20
    }
}