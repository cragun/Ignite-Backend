using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Enums.PropertyAttachments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace DataReef.TM.Models.PropertyAttachments
{
    [Table("PropertyAttachments")]
    public class PropertyAttachment : EntityBase
    {
        /// <summary>
        /// ID of the Attachment option found at Legion.Features.Property.Attachments.Options
        /// </summary>
        [DataMember]
        public int AttachmentTypeID { get; set; }

        [DataMember]
        public string SystemTypeID { get; set; }

        [DataMember]
        public Guid PropertyID { get; set; }

        [DataMember]
        public ItemStatus Status { get; set; }

        [DataMember]
        [JsonIgnore]
        public string AuditJSON { get; set; }

        #region Navigation properties
        [ForeignKey("PropertyID")]
        [DataMember]
        public virtual Property Property { get; set; }

        [DataMember]
        public virtual ICollection<PropertyAttachmentItem> Items { get; set; }
        #endregion

        #region Helpers

        public List<AuditItem> GetAudit()
        {
            return string.IsNullOrEmpty(AuditJSON) ? new List<AuditItem>() : JsonConvert.DeserializeObject<List<AuditItem>>(AuditJSON);
        }

        public void SetAudit(IEnumerable<AuditItem> value)
        {
            AuditJSON = value?.Any() == true ? JsonConvert.SerializeObject(value) : null;
        }

        public void AddAudit(AuditItem newAudit)
        {
            var audit = GetAudit();
            audit.Add(newAudit);
            SetAudit(audit);
        }

        public bool AllHaveImages(PhotosDefinitionSettingModel definition)
        {
            var systemTypeDef = definition?
                                    .Data?
                                    .Definitions?
                                    .FirstOrDefault(d => d.SystemTypeID == SystemTypeID);
            var result = true;
            systemTypeDef?
                    .Sections?
                    .ForEach(section =>
                    {
                        result = result && (Items?
                                    .Where(i => i.SectionID == section.SectionId
                                                && section.TaskIDs?.Contains(i.ItemID) == true
                                                && definition?.IsTaskOptional(i.ItemID) != true)?
                                    .Any(i => i.GetImages()?.Any() != true) != true);
                    });
            return result;
        }

        public bool AllWithinSectionHaveImages(PhotosDefinitionSettingModel definition, string sectionId)
        {
            var systemTypeDef = definition?
                                    .Data?
                                    .Definitions?
                                    .FirstOrDefault(d => d.SystemTypeID == SystemTypeID);
            var section = systemTypeDef.Sections.FirstOrDefault(x => x.SectionId == sectionId);
            if(section != null)
            {
                return (Items?
                            .Where(i => i.SectionID == section.SectionId
                                        && section.TaskIDs?.Contains(i.ItemID) == true
                                        && definition?.IsTaskOptional(i.ItemID) != true)?
                            .Any(i => i.GetImages()?.Any() != true) != true);
            }
            return false;
        }

        public bool AllAreApproved(PhotosDefinitionSettingModel definition)
        {
            var systemTypeDef = definition?
                                    .Data?
                                    .Definitions?
                                    .FirstOrDefault(d => d.SystemTypeID == SystemTypeID);
            var result = true;
            systemTypeDef?
                    .Sections?
                    .ForEach(section =>
                    {
                        result = result && Items?
                                    .Any(item => item.SectionID == section.SectionId
                                                && section.TaskIDs?.Contains(item.ItemID) == true
                                                && item.Status != ItemStatus.Approved) != true;
                    });
            return result;
        }


        #endregion
    }
}
