using DataReef.TM.Models.DataViews.Geo;
using DataReef.TM.Models.Enums.PropertyAttachments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    [DataContract]
    [NotMapped]
    public class Image
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public GeoPoint Location { get; set; }

        [DataMember]
        public List<Thumbnail> Thumbnails { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public string RejectionMessage { get; set; }

        [DataMember]
        public ItemStatus? Status { get; set; }
    }
}
