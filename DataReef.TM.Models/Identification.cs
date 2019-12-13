using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataReef.TM.Models
{
    [DataContract]
    public enum IdentificationType
    {
        [EnumMember]
        DriverLicense,

        [EnumMember]
        BirthCertificate,

        [EnumMember]
        Passport,

        [EnumMember]
        MilitaryID,

        [EnumMember]
        Other
    }


    [DataContract]
    public class Identification : EntityBase
    {
        [StringLength(50)]
        [DataMember]
        public string IssuedBy { get; set; }

        [StringLength(50)]
        [DataMember]
        public string Number { get; set; }

        [StringLength(50)]
        [DataMember]
        public string Expiration { get; set; }

        [DataMember]
        public IdentificationType Type { get; set; }

        [StringLength(50)]
        [DataMember]
        public string DateIssued { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }


        /// <summary>
        /// if center needed to verify
        /// </summary>
        [DataMember]
        public System.DateTime? DateVerified { get; set; }

        /// <summary>
        /// Guid of person who performed verification
        /// </summary>
        [DataMember]
        public Guid? VerifiedByID { get; set; }


        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        [DataMember]
        public ICollection<Attachment> Attachments { get; set; }

        #endregion


        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Attachments = FilterEntityCollection(Attachments, newInclusionPath);

            Person = FilterEntity(Person, newInclusionPath);

        }


    }
}
