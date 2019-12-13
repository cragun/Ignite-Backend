using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract]
    public enum PhoneType
    {
        [EnumMember]
        Mobile = 1,

        [EnumMember]
        Business = 2,

        [EnumMember]
        Home = 3,

        [EnumMember]
        Fax = 4,

        [EnumMember]
        Other = 5
    }

    public class PhoneNumber : EntityBase
    {
        [DataMember(EmitDefaultValue = false)]
        public Guid? PersonID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid? OUID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        [Phone]
        public string Number { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(10)]
        public string Extension { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [NotMapped]
        public string AreaCode { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool IsPrimary { get; set; }

        [DataMember(EmitDefaultValue = true)]
        public PhoneType PhoneType { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Person  = FilterEntity(Person,    newInclusionPath);
            OU      = FilterEntity(OU,        newInclusionPath);

        }

    }
}
