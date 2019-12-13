using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace DataReef.TM.Models
{
    public class Address : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid? OUID { get; set; }

        [DataMember]
        public Guid? PersonID { get; set; }

        /// <summary>
        /// Address number and street 
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [StringLength(50, ErrorMessage = "Address Line 1 cannot be longer than 50 characters")]
        public string Address1 { get; set; }

        /// <summary>
        /// Second line of the addresses.  Not required
        /// </summary>
        /// 
        [Description("This is the description")]
        [DataMember(EmitDefaultValue = false)]
        [StringLength(200)]
        public string Address2 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string City { get; set; }

        /// <summary>
        /// 2 character street abbreviation
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [StringLength(2)]
        public string State { get; set; }

        /// <summary>
        /// 5 digit zip Code
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [StringLength(5), MinLength(5)]
        public string ZipCode { get; set; }

        /// <summary>
        /// Zip 4 part (US Addresses)
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        [StringLength(4), MinLength(4)]
        public string PlusFour { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [StringLength(50)]
        public string County { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float Latitude { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public float Longitude { get; set; }


        [DataMember(EmitDefaultValue = false)]
        [StringLength(400)]
        public string Description { get; set; }

        #endregion

        #region Navigation

        /// <summary>
        /// The Organization Unit that defined this activity
        /// </summary>
        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        /// <summary>
        /// The person of this adress
        /// </summary>
        [DataMember]
        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            OU      =    FilterEntity(OU,        newInclusionPath);
            Person  =    FilterEntity(Person,    newInclusionPath);

        }

    }
}
