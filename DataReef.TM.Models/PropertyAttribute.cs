using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// this class is scheduled to be depricated.  PropertyRating will take its place
    /// </summary>
    public class PropertyAttribute : EntityBase
    {
        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        [StringLength(50)]
        public string DisplayType { get; set; }

        [DataMember]
        [StringLength(150)]
        public string Value { get; set; }

        [DataMember]
        [StringLength(50)]
        public string AttributeKey { get; set; }

        [DataMember]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        public Guid? TerritoryID { get; set; }

        [DataMember]
        public Guid? UserID { get; set; }


        #region Navigation

        [ForeignKey("PropertyID")]
        public Property Property { get; set; }

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
        }
    }
}