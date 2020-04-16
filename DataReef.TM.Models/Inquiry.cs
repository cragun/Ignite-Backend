using DataReef.TM.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    [Table("Inquiries")]
    public class Inquiry : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public ActivityType ActivityType { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? FollowUpDate { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public long ClockDiff { get; set; }

        [StringLength(50)]
        [DataMember]
        public string ClockType { get; set; }

        [DataMember]
        public bool? ShouldIntegrateWithCalendar { get; set; }

        [StringLength(50)]
        [DataMember]
        public string Color { get; set; }

        [StringLength(50)]
        [DataMember]
        public string Annotation { get; set; }

        [DataMember]
        public Guid? OUID { get; set; }

        /// <summary>
        ///     When double knocking, all inquiries are Archived before starting the double knock.  So we know which
        /// </summary>
        [DataMember]
        public bool IsArchive { get; set; }

        /// <summary>
        ///     this person was interested and should be followed
        /// </summary>
        [DataMember]
        public bool IsLead { get; set; }

        /// <summary>
        ///     Longitude at the time of the inquiry
        /// </summary>
        [DataMember]
        public double? Lat { get; set; }

        /// <summary>
        ///     Latitude at the time of the inquiry
        /// </summary>
        [DataMember]
        public double? Lon { get; set; }

        /// <summary>
        /// The new and more flexible disposition/status.
        /// </summary>
        [DataMember]
        [StringLength(50)]
        [Index("IDX_INQUIRY_DISPOSITION")]
        public string Disposition { get; set; }

        [DataMember]
        public Nullable<int> DispositionTypeId { get; set; }


        [DataMember]
        [StringLength(50)]
        public string OldDisposition { get; set; }

        #endregion

        #region Navigation

        [ForeignKey("PropertyID")]
        [DataMember]
        public Property Property { get; set; }

        /// <summary>
        ///     Person who performed the inquiry
        /// </summary>
        [ForeignKey("PersonID")]
        [DataMember]
        public User User { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Property = FilterEntity(Property, newInclusionPath);
            User = FilterEntity(User, newInclusionPath);
        }
    }
}