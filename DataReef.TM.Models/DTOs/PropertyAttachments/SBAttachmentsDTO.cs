using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    /// <summary>
    /// Class used to pass property attachment information to SmartBOARD.
    /// It includes both the definitions and the data.
    /// </summary>
    public class SBAttachmentsDTO
    {
        public SBAttachmentsDTO(List<KeyValuePair<PhotosDefinitionSettingModel, PropertyAttachmentDTO>> attachments, List<Person> people, AttachmentOptionDefinition attOptDef)
        {
            Data = attachments?.Select(att => new SBAttachmentDataDTO(att.Key, att.Value, people, attOptDef)).ToList();
        }

        public List<SBAttachmentDataDTO> Data { get; set; }
    }
}
