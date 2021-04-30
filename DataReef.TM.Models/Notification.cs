using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    [DataContract(IsReference = true)]
    [Versioned]
    public class Notification : EntityBase
    {
        #region Properties
        [DataMember]
        [Index(IsUnique = false, IsClustered = false)]
        public Guid PersonID { get; set; }

        [DataMember]
        [Index(IsUnique = false, IsClustered = false)]
        public IgniteNotificationType NotificationType { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        [Index(IsUnique = false, IsClustered = false)]
        public DateTime? SeenAt { get; set; }

        [DataMember]
        [Index(IsUnique = false, IsClustered = false)]
        public IgniteNotificationSeenStatus Status { get; set; }

        /// <summary>
        /// Holds the Guid of the target entity if one exists
        /// </summary>
        [DataMember]
        public Guid? Value { get; set; }

        [DataMember]
        public string SmartBoardID { get; set; }

        [NotMapped]
        public Guid NoteID { get; set; } 

        [DataMember]
        [Index(IsUnique = false, IsClustered = false)]
        public Guid PropertyID { get; set; } 

        #endregion

        #region Navigation Properties
        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        [ForeignKey("PropertyID")]
        [DataMember]
        public Property Property { get; set; }

        #endregion
    }
}
