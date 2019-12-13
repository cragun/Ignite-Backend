using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    [DataContract]
    [NotMapped]
    public class Thumbnail
    {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public ThumbnailType Type { get; set; }
    }

    public enum ThumbnailType
    {
        Small,
        Medium,
        Large
    }
}
