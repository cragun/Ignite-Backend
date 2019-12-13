using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations.Schema;



namespace DataReef.TM.Models
{
    /// <summary>
    /// userd to record the integration of an object to another system
    /// </summary>
    public class DataIntegration:EntityBase
    {
        /// <summary>
        /// Netsuite, Agemni, CommPay
        /// </summary>
        [DataMember]
        [StringLength(50)]
        public string IntegratonType { get; set; }

        /// <summary>
        /// The class of the object represented by the json Payload field
        /// </summary>
        [DataMember]
        [StringLength(100)]    
        public string Class { get; set; }

        /// <summary>
        /// Our object serialized ( as JSON )
        /// </summary>
        [DataMember]
        public string Payload { get; set; }

        /// <summary>
        /// The response returned from foreign system
        /// </summary>
        [DataMember]
        public string Response { get; set; }

        [DataMember]
        [StringLength(50)]
        public string ErrorCode { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// guid of our system, PropertyID for example
        /// </summary>
        [DataMember]
        public Guid PrimaryKey { get; set; }

        /// <summary>
        /// primary key as returned by 
        /// </summary>
        [DataMember]
        public string ForeignSystemPrimaryKey { get; set; }



    }
}
