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
    public class ExtendedPropertyAttachmentItemDTO : PropertyAttachmentItemDTO
    {
        [DataMember]
        public string SectionName { get; set; }

        [DataMember]
        public string ItemName { get; set; }

        public ExtendedPropertyAttachmentItemDTO(PropertyAttachmentItem item, PhotosDefinitionSettingModel definition) : base(item)
        {
            SectionName = definition?.Data?.Sections?.FirstOrDefault(x => x.Id.Equals(SectionID))?.Name;
            ItemName = definition?.Data?.Tasks?.FirstOrDefault(x => x.Id.Equals(ItemID))?.Name;
        }

        
    }
}
