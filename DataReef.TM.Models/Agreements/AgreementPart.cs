using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{

    [DataContract]
    public enum AgreementPartType
    {
        [EnumMember]
        Signature,

        [EnumMember]
        Date,

        [EnumMember]
        Initials,

        [EnumMember]
        Checkbox

    }

    public class AgreementPart : EntityBase
    {

        #region Properties

        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid AgreementID { get; set; }

        [StringLength(50)]
        [DataMember(EmitDefaultValue = false)]
        public string LocationString { get; set; }

        [StringLength(50)]
        [DataMember(EmitDefaultValue = false)]
        public string SizeString { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public int PageNumber { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public bool IsRequired { get; set; }

        [StringLength(50)]
        [DataMember(EmitDefaultValue = false)]
        public string OptionGroupID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public AgreementPartType Type { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        #endregion

        #region Navigation

        [ForeignKey("AgreementID")]
        public Agreement Agreement { get; set; }

        #endregion


    }
}
