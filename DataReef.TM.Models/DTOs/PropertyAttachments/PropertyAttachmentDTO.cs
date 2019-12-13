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
    [KnownType(typeof(ExtendedPropertyAttachmentDTO))]
    [NotMapped]
    public class PropertyAttachmentDTO
    {
        [DataMember]
        public Guid Guid { get; set; }

        /// <summary>
        /// ID of the Attachment option found at Legion.Features.Property.Attachments.Options
        /// </summary>
        [DataMember]
        public int AttachmentTypeID { get; set; }

        [DataMember]
        public Header Header { get; set; }

        [DataMember]
        public List<AuditItem> Audit { get; set; }


        [DataMember]
        public IEnumerable<PropertyAttachmentItemDTO> Items { get; set; }

        public PropertyAttachmentDTO(PropertyAttachment propertyAttachment)
        {
            if(propertyAttachment != null)
            {
                Guid = propertyAttachment.Guid;
                Header = new Header
                {
                    SystemTypeID = propertyAttachment.SystemTypeID,
                    CreatedDate = propertyAttachment.DateCreated,
                    UpdateDate = propertyAttachment.DateLastModified,
                    Status = propertyAttachment.Status,
                    Customer =
                    propertyAttachment.Property == null
                    ? null
                    : new Customer(propertyAttachment.Property)
                };
                AttachmentTypeID = propertyAttachment.AttachmentTypeID;
                Items = propertyAttachment.Items?.Select(x => new PropertyAttachmentItemDTO(x));
                Audit = propertyAttachment.GetAudit();
            }
        }        
            
    }
}
