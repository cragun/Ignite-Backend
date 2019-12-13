using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{

    [DataContract]
    public enum VisibilityFlags
    {
        [EnumMember]
        Private,

        [EnumMember]
        Internal,

        [EnumMember]
        Public

    }

    /// <summary>
    /// Todo.  What to do with this
    /// </summary>

    [DataContract]
    public class Note : EntityBase
    {
        [DataMember(EmitDefaultValue = false)]
        public Guid PersonID { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Content { get; set; }

        /// <summary>
        /// Object this belongs to
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Guid OwnerID { get; set; }

        /// <summary>
        /// If private.. only is visible to the UserID
        /// </summary>
        [DataMember]
        public VisibilityFlags VisibilityFlag { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("PersonID")]
        public Person Person { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Person = FilterEntity(Person, newInclusionPath);

        }
    }
}