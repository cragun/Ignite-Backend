using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.PRMI
{
    [Table("VelocifyResponses", Schema = "PRMI")]
    [DataContract]
    public class VelocifyResponse : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid VelocifyRequestID { get; set; }

        [DataMember]
        public Guid ReferenceID { get; set; }

        [DataMember]
        public string LeadID { get; set; }

        [DataMember]
        public string Result { get; set; }

        [DataMember]
        public string Message { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        [ForeignKey(nameof(VelocifyRequestID))]
        public VelocifyRequest VelocifyRequest { get; set; }

        #endregion
    }
}
