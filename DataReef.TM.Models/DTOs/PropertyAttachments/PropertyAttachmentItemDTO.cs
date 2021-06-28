using DataReef.TM.Models.Enums.PropertyAttachments;
using DataReef.TM.Models.PropertyAttachments;
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
    [KnownType(typeof(ExtendedPropertyAttachmentItemDTO))]
    public class PropertyAttachmentItemDTO
    {
        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public string ItemID { get; set; }

        [DataMember]
        public string SectionID { get; set; }

        [DataMember]
        public ItemStatus Status { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public DateTime? UpdateDate { get; set; }

        [DataMember]
        public List<Image> Images { get; set; }

        [DataMember]
        public string RejectionMessage { get; set; }

        public PropertyAttachmentItemDTO(PropertyAttachmentItem item)
        {
            Guid = item.Guid;
            CreatedDate = item.DateCreated;
            UpdateDate = item.DateLastModified;
            SectionID = item.SectionID;
            ItemID = item.ItemID;
            RejectionMessage = item.RejectionMessage;
            //Images = item.GetImages();
            Images = item.GetProxifyImages();
            Status = item.Status;
        }
    }
}
