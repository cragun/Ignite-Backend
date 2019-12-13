using DataReef.TM.Models.DataViews.Geo;
using DataReef.TM.Models.Enums.PropertyAttachments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    [DataContract]
    [NotMapped]
    public class SBAttachmentDataDTO
    {
        public SBAttachmentDataDTO(PhotosDefinitionSettingModel defitinion, PropertyAttachmentDTO attachment, List<Person> people, AttachmentOptionDefinition attOptDef)
        {
            if (defitinion == null || attachment == null)
            {
                return;
            }
            AttachmentTypeName = attOptDef.Name;
            AttachmentTypeID = attachment.AttachmentTypeID;
            Header = new SBAttachmentDataHeaderDTO(defitinion, attachment);

            var sectionIDs = attachment
                                .Items?
                                .Select(i => i.SectionID)?
                                .Distinct();
            Sections = sectionIDs?
                            .Select(id => new SBAttachmentDataSectionDTO(id, defitinion, attachment, people))?
                            .ToList();
        }


        [DataMember]
        public string AttachmentTypeName { get; set; }

        [DataMember]
        public int AttachmentTypeID { get; set; }

        [DataMember]
        public SBAttachmentDataHeaderDTO Header { get; set; }

        [DataMember]
        public List<SBAttachmentDataSectionDTO> Sections { get; set; }
    }

    [DataContract]
    [NotMapped]
    public class SBAttachmentDataHeaderDTO
    {
        public SBAttachmentDataHeaderDTO(PhotosDefinitionSettingModel defitinion, PropertyAttachmentDTO attachment)
        {
            var header = attachment?.Header;
            SystemType = defitinion?.Data?.SystemTypes?.FirstOrDefault(st => st.Id == header?.SystemTypeID)?.Name;
            Status = header?.Status;
            CreatedDate = header?.CreatedDate;
            UpdateDate = header?.UpdateDate;
        }

        [DataMember]
        public string SystemType { get; set; }

        [DataMember]
        public ItemStatus? Status { get; set; }

        [DataMember]
        public DateTime? CreatedDate { get; set; }

        [DataMember]
        public DateTime? UpdateDate { get; set; }
    }

    [DataContract]
    [NotMapped]
    public class SBAttachmentDataSectionDTO
    {
        public SBAttachmentDataSectionDTO(string id, PhotosDefinitionSettingModel defitinion, PropertyAttachmentDTO attachment, List<Person> people)
        {
            Name = defitinion?
                        .Data?
                        .Sections?
                        .FirstOrDefault(s => s.Id == id)?
                        .Name;

            var items = attachment?
                        .Items?
                        .Where(i => i.SectionID == id);

            Tasks = items?
                        .Select(itm => new SBAttachmentDataTaskDTO(itm, defitinion, people))?
                        .ToList();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public List<SBAttachmentDataTaskDTO> Tasks { get; set; }
    }

    [DataContract]
    [NotMapped]
    public class SBAttachmentDataTaskDTO
    {
        public SBAttachmentDataTaskDTO(PropertyAttachmentItemDTO item, PhotosDefinitionSettingModel defitinion, List<Person> people)
        {
            var taskDefinition = defitinion?
                                .Data?
                                .Tasks?
                                .FirstOrDefault(t => t.Id == item.ItemID);

            Name = taskDefinition?.Name;
            IsOptional = taskDefinition?.IsOptional;

            Status = item?.Status;
            CreatedDate = item?.CreatedDate;
            UpdateDate = item?.UpdateDate;
            RejectionMessage = item?.RejectionMessage;
            Images = item?
                        .Images?
                        .Select(img => new SBAttachmentDataTaskImageDTO(img, people))?
                        .ToList();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool? IsOptional { get; set; }

        [DataMember]
        public ItemStatus? Status { get; set; }

        [DataMember]
        public DateTime? CreatedDate { get; set; }

        [DataMember]
        public DateTime? UpdateDate { get; set; }

        [DataMember]
        public List<SBAttachmentDataTaskImageDTO> Images { get; set; }

        [DataMember]
        public string RejectionMessage { get; set; }

    }

    [DataContract]
    [NotMapped]
    public class SBAttachmentDataTaskImageDTO
    {
        public SBAttachmentDataTaskImageDTO(Image image, List<Person> people)
        {
            UserName = people?.FirstOrDefault(p => p.Guid == image.UserID)?.FullName;
            Name = image?.Name;
            Url = image?.Url;
            Location = image?.Location;
            Thumbnails = image?.Thumbnails;
            RejectionMessage = image?.RejectionMessage;
            Status = image?.Status;
        }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public GeoPoint Location { get; set; }

        [DataMember]
        public List<Thumbnail> Thumbnails { get; set; }

        [DataMember]
        public string RejectionMessage { get; set; }

        [DataMember]
        public ItemStatus? Status { get; set; }
    }
}
