
using DataReef.TM.Models.PropertyAttachments;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    [DataContract]
    public class ExtendedPropertyAttachmentDTO : PropertyAttachmentDTO
    {
        /// <summary>
        /// Name of the System found at SystemTypeID
        /// </summary>
        [DataMember]
        public string SystemTypeName { get; set; }

        /// <summary>
        /// Name of the property for this attachment
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }

        /// <summary>
        /// Organization name for the property of this attachment
        /// </summary>
        [DataMember]
        public string OUName { get; set; }


        /// <summary>
        /// Territory name for the property of this attachment
        /// </summary>
        [DataMember]
        public string TerritoryName { get; set; }

        /// <summary>
        /// Name of the Attachment type matching AttachmentTypeID
        /// </summary>
        [DataMember]
        public string AttachmentTypeName { get; set; }

        /// <summary>
        /// Name of the person identified with CreatedByID
        /// </summary>
        [DataMember]
        public string SalesAgent { get; set; }

        [DataMember(Name = "ExtendedItems")]
        public new IEnumerable<ExtendedPropertyAttachmentItemDTO> Items { get; set; }

        public ExtendedPropertyAttachmentDTO(PropertyAttachment propertyAttachment, PhotosDefinitionSettingModel definition, AttachmentOptionDefinition optionsDefinition, Person createdBy = null) : base(propertyAttachment)
        {
            if (propertyAttachment != null)
            {
                SystemTypeName = definition?.Data?.SystemTypes?.FirstOrDefault(x => x.Id.Equals(Header.SystemTypeID))?.Name;
                PropertyName = propertyAttachment.Property?.Name;
                OUName = propertyAttachment.Property?.Territory?.OU?.Name;
                TerritoryName = propertyAttachment.Property?.Territory?.Name;
                AttachmentTypeName = optionsDefinition?.Name;
                SalesAgent = createdBy?.Name;
                Items = propertyAttachment.Items?.Select(x => new ExtendedPropertyAttachmentItemDTO(x, definition));
            }
        }
    }
}
