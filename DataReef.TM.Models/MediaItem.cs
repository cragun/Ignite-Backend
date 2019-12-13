using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models
{

    [DataContract]
    public enum MediaType
    {
        [EnumMember]
        Sales = 0,

        [EnumMember]
        Training = 1
    }

    public class MediaItem : EntityBase
    {

        /// <summary>
        /// simple flag to identify a folder
        /// </summary>
        [DataMember]
        public bool IsFolder { get; set; }

        /// <summary>
        /// Guid of the parent folder, or containing folder ( for a non older)
        /// </summary>
        [DataMember]
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Guid of the Top Level OU - Clear for example
        /// </summary>
        [DataMember]
        public Guid OUID { get; set; }

        /// <summary>
        /// RFC3778 Mime Types.   application/pdf, application/png, video/mp4
        /// </summary>
        [DataMember]
        [StringLength(100)]
        public string MimeType { get; set; }

        /// <summary>
        /// URL to find the resource ( on YouTube, or Amazon, or Something )
        /// </summary>
        [DataMember]
        [StringLength(255)]
        public string Url { get; set; }

        /// <summary>
        /// Size in Bytes of Resource
        /// </summary>
        [DataMember]
        public long Size { get; set; }


        /// <summary>
        /// token needed to access resource ( Amazon for example )
        /// </summary>
        [DataMember]
        [StringLength(255)]
        public string AuthenticationnToken { get; set; }

        [DataMember]
        public MediaType MediaType { get; set; }

        [NotMapped]
        public int OrderInFolder { get; set; }

        #region Navigation

        [DataMember]
        [ForeignKey("ParentID")]
        public MediaItem Parent { get; set; }

        [DataMember]
        public ICollection<MediaItem> Children { get; set; }

        [DataMember]
        [InverseProperty("MediaItem")]
        public ICollection<OUMediaItem> OUAssociations { get; set; }

        #endregion

        public override void FilterCollections<T>(string inclusionPath = "")
        {
            bool alreadyProcessed;
            string newInclusionPath = InclusionPathBuilder(inclusionPath, GetType().Name, out alreadyProcessed);
            if (alreadyProcessed)
            {
                return;
            }

            Children        = FilterEntityCollection(Children,          newInclusionPath);
            OUAssociations  = FilterEntityCollection(OUAssociations,    newInclusionPath);

            Parent = FilterEntity(Parent, newInclusionPath);

        }

    }
}
