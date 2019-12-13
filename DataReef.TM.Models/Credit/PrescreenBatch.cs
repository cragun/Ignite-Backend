using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.Credit
{
    /// <summary>
    /// The batch object for running prequals.  
    /// </summary>
    public class PrescreenBatch : EntityBase
    {
        [DataMember]
        public PrescreenSource Source { get; set; }

        /// <summary>
        /// The guid of the person who ran ( and is accountable ) for the batch
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        /// <summary>
        /// All batches will begin with a pending process, then will move into Processing once the qualification server is contacted
        /// </summary>
        [DataMember]
        public PrescreenStatus Status { get; set; }

        /// <summary>
        /// The guid of the territory to prescreen
        /// </summary>
        [DataMember]
        public Guid TerritoryID { get; set; }

      
        /// <summary>
        /// The datetime that the process completed
        /// </summary>
        [DataMember]
        public DateTime? CompletionDate { get; set; }

        /// <summary>
        /// If the processed errored out ... the service should populate this error string
        /// </summary>
        [DataMember]
        public string ErrorString { get; set; }

        #region Navigation

        /// <summary>
        /// The system will populate the details.  The client only submits the Batch object  with the TerritoryID
        /// </summary>
        [DataMember]
        [InverseProperty("Batch")]
        public ICollection<PrescreenDetail> Details { get; set; }

        [ForeignKey("TerritoryID")]
        [DataMember]
        public Territory Territory { get; set; }

        #endregion




    }
}
