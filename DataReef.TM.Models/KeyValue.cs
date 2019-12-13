using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
 
   /// <summary>
   /// KeyValue is a dictionary entry that can be associated with any entity.  Its primary purpose is for the users of the ClientAPI to decorate the object with their own meta data
   /// </summary>
    public class KeyValue : EntityBase
    {
        #region Properties

        /// <summary>
        /// Guid of the object that these values belong to
        /// </summary>
        [Required]
        [Index("idx_kv_primary",0)]
        [DataMember]
        public Guid ObjectID { get; set; }

        [Required]
        [Index("idx_kv_primary",1)]
        [StringLength(50)]
        [DataMember]
        public string Key { get; set; }

        public string Value { get; set; }

        public string Notes { get; set; }

        #endregion

        #region Navigation

   //None

        #endregion


    }
}