using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataReef.TM.Models
{

    public class Attachment : EntityBase
    {
        #region Properties

        [DataMember]
        [StringLength(50)]
        public string MimeType { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public int? SortOrder { get; set; }

        [DataMember]
        public Guid OwnerID { get; set; }

     

        #endregion


    }
}
