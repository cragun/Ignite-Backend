using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [DataContract]
    public class Authentication
    {
        [DataMember]
        [Key, Required]
        public Guid Guid { get; set; }

        [DataMember]
        [Index]
        public Guid UserID { get; set; }

        [DataMember]
        public Guid DeviceID { get; set; }

        [DataMember]
        public DateTime DateAuthenticated { get; set; }

        #region Navigation

        [ForeignKey("UserID")]
        [DataMember]
        public User User { get; set; }

        #endregion
    }
}
