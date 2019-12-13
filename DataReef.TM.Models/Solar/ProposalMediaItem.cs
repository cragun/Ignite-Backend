using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.Solar
{
    [Table("ProposalMediaItems", Schema = "solar")]
    public class ProposalMediaItem : EntityBase
    {
        [DataMember]
        public Guid ProposalID { get; set; }

        [DataMember]
        [JsonIgnore]
        [StringLength(2048)]
        public string Url { get; set; }

        [DataMember]
        [JsonIgnore]
        [StringLength(2048)]
        public string ThumbUrl { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        [JsonIgnore]
        [StringLength(128)]
        public string MimeType { get; set; }

        [DataMember]
        public ProposalMediaItemType MediaItemType { get; set; }

        #region Navigation properties

        [ForeignKey(nameof(ProposalID))]
        [DataMember]
        public Proposal Proposal { get; set; }


        #endregion

        public string BuildUrl()
        {
            return $"proposal-data/{ProposalID}/media-items/{Guid}";
        }

        public string BuildThumbUrl()
        {
            return $"{BuildUrl()}_thumb";
        }

        public string GetAWSFileName(bool thumb = false)
        {
            var path = thumb ? ThumbUrl : Url;
            if (path?.StartsWith("https://") == true)
            {
                path = path.GetAWSPath();
            }
            return path;
        }
    }
}
