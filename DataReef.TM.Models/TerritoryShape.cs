using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    [DataContract]
    public class TerritoryShape : BaseShape
    {
        #region Properties

        /// <summary>
        /// Guid of the Territory that this TerritoryShape belongs to
        /// </summary>
        [DataMember]
        public Guid TerritoryID { get; set; }

        #endregion

        #region Navigation

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

            Territory = FilterEntity(Territory, newInclusionPath);

        }

    }
}
