using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.Enums.PropertyAttachments;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.PropertyAttachments
{
    [Table("PropertyAttachmentItems")]
    public class PropertyAttachmentItem : EntityBase
    {
        [DataMember]
        public string ItemID { get; set; }

        [DataMember]
        public string SectionID { get; set; }

        [DataMember]
        public ItemStatus Status { get; set; }

        [DataMember]
        [JsonIgnore]
        public string ImagesJson { get; set; }

        [DataMember]
        [JsonIgnore]
        public string RejectionMessage { get; set; }

        [DataMember]
        public Guid PropertyAttachmentID { get; set; }

        #region Navigation properties

        [ForeignKey("PropertyAttachmentID")]
        [DataMember]
        public virtual PropertyAttachment PropertyAttachment { get; set; }

        #endregion

        #region Helpers

        public List<Image> GetImages()
        {
            if (string.IsNullOrEmpty(ImagesJson))
            {
                return new List<Image>();
            }

            return JsonConvert.DeserializeObject<List<Image>>(ImagesJson);
        }

        public List<Image> GetProxifyImages()
        {  
            return GetImages().Select(c =>
            {
                c.Url = c.Url?.GetAWSProxifyUrl();
                c.Thumbnails = c.Thumbnails.Select(a => { a.Url = a.Url?.GetAWSProxifyUrl(); return a; }).ToList();
                return c;
            }).ToList();
        }

        public void SetImages(IEnumerable<Image> imagesList)
        {
            ImagesJson = imagesList?.Any() == true ? JsonConvert.SerializeObject(imagesList) : null;
        }

        #endregion
    }


}
