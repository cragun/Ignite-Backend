using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;


namespace DataReef.TM.Models
{
    [DataContract]
    public enum AssignmentStatus
    {
        [EnumMember] Open,

        [EnumMember] Closed,
    }

    /// <summary>
    /// Assignments are Territory Associations.  The many to many between a person and a Territory
    /// </summary>
    public class Assignment : EntityBase
    {
        #region Properties

        /// <summary>
        /// Guid of the person assigned a territory
        /// </summary>
        [DataMember]
        public Guid PersonID { get; set; }

        [DataMember]
        public Guid TerritoryID { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime? DateClosed { get; set; }

        [DataMember(EmitDefaultValue = true)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime? DateAvailable { get; set; }

        /// <summary>
        ///     typically a rep will want to close out a territory so that it can be reflected into their ui
        /// </summary>
        [DataMember]
        public AssignmentStatus Status { get; set; }

        [DataMember]
        public string Notes { get; set; }

     

        #endregion

        #region Navigation

        [ForeignKey("PersonID")]
        [DataMember]
        public Person Person { get; set; }

        [ForeignKey("TerritoryID")]
        [DataMember]
        public Territory Territory { get; set; }


     

        #endregion


        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Person      = FilterEntity(Person,      newInclusionPath);
            Territory   = FilterEntity(Territory,   newInclusionPath);

        }

    }
}