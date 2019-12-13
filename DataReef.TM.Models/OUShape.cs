using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models
{
    /// <summary>
    /// OUs can be made up of one or more types of shapes from the GeoServer.  This is the collection of shapes that belong to the OU
    /// </summary>
    [DataContract]
    public class OUShape : BaseShape
    {
        #region Properties

        /// <summary>
        /// Guid of the OU that this OUShape belongs to
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        #endregion

        #region Navigation

        [ForeignKey("OUID")]
        [DataMember]
        public OU OU { get; set; }


        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            OU = FilterEntity(OU, newInclusionPath);

        }

    }
}
