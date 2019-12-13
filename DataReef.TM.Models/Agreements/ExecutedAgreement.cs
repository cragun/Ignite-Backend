using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    public class ExecutedAgreement : EntityBase
    {

        #region Properties

        [Required]
        [DataMember(EmitDefaultValue = false)]
        public Guid AgreementID { get; set; }

        [Required]
        [DataMember(EmitDefaultValue = false)]
        public Guid AccountID { get; set; }

        /// <summary>
        /// person ID that the agreement belongs to (ususally a child)
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// PersonID of the person who is signing
        /// </summary>
        [DataMember]
        public Guid? SignerID { get; set; }


        /// <summary>
        /// used for sending in base64 encoded blob data.  will be stored via the blob service
        /// </summary>
        [NotMapped]
        [DataMember(EmitDefaultValue = false)]
        public string BlobData { get; set; }


        #endregion


        #region Navigation

        [DataMember]
        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        [DataMember]
        [ForeignKey("SignerID")]
        public Person Signer { get; set; }

        [DataMember(EmitDefaultValue = false)]
        [ForeignKey("AgreementID")]
        public Agreement Agreement { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public ICollection<ExecutedAgreementPart> Parts { get; set; }

        #endregion



    }
}
