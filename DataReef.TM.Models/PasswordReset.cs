using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    public class PasswordReset : EntityBase
    {
        [DataMember]
        public string UserName { get; set; }

        [EmailAddress]
        [DataMember]
        public string EmailAddress { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public DateTime? ExpirationDate { get; set; }

        [DataMember]
        public bool PasswordWasReset { get; set; }

        [DataMember]
        public DateTime? DateReset { get; set; }
    }
}