using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models
{
    public class OUMediaItem : EntityBase
    {
        #region Properties

        [DataMember]
        public Guid OUID { get; set; }

        [DataMember]
        public Guid MediaID { get; set; }

        /// <summary>
        /// This is the OUMediaItem parent
        /// </summary>
        [DataMember]
        public Guid? ParentId { get; set; }

        [DataMember]
        public int OrderInFolder { get; set; }

        #endregion

        #region Navigation

        [DataMember]
        [ForeignKey("OUID")]
        public OU OU { get; set; }

        [DataMember]
        [ForeignKey("MediaID")]
        public MediaItem MediaItem { get; set; }

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
            MediaItem = FilterEntity(MediaItem, newInclusionPath);

        }
    }
}
