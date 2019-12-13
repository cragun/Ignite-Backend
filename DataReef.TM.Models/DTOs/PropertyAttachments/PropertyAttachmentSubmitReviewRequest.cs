using DataReef.TM.Models.Enums.PropertyAttachments;
using DataReef.TM.Models.PropertyAttachments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataReef.TM.Models.DTOs.PropertyAttachments
{
    public class PropertyAttachmentSubmitReviewRequest
    {
        public ItemStatus? Status { get; set; }

        public List<ItemReviewModel> Items { get; set; }


        public class ItemReviewModel
        {
            public Guid Guid { get; set; }

            public ItemStatus? Status { get; set; }

            public string Message { get; set; }

            public List<ItemImageReviewModel> Images { get; set; }

            public string UpdateEntity(ref PropertyAttachmentItem entity, PhotosDefinitionSettingModel def)
            {
                string result = null;
                if (entity == null)
                {
                    return result;
                }

                if (Status.HasValue && Status != entity.Status)
                {
                    result += $"Changed status to {Status}. ";
                    entity.Status = Status.Value;
                }

                
                if (!string.IsNullOrEmpty(Message) && Message != entity.RejectionMessage)
                {
                    result += $"Added reject message: {string.Join(", ", Message)}. ";
                    entity.RejectionMessage = Message;
                }
                //clear rejection message if approved
                if(entity.Status == ItemStatus.Approved)
                {
                    entity.RejectionMessage = string.Empty;
                }

                result = string.IsNullOrEmpty(result) ? result : $"Updated task [{def.GetTaskName(entity.ItemID)}] {result}\r\n";

                var entityImages = entity.GetImages();
                Images?.ForEach(img =>
                {
                    var entImg = entityImages?.FirstOrDefault(ei => ei.ID == img.ImageID);
                    result += img.UpdateImage(ref entImg, Images.IndexOf(img));
                });

                entity.SetImages(entityImages);

                return result;
            }
        }

        public class ItemImageReviewModel
        {
            public Guid ImageID { get; set; }
            public ItemStatus? Status { get; set; }
            public string Message { get; set; }

            public string UpdateImage(ref Image image, int idx)
            {
                string result = null;
                if (Status.HasValue && Status != image.Status)
                {
                    result = $"Changed status to {Status}. ";
                    image.Status = Status.Value;
                }

                if (!string.IsNullOrEmpty(Message) && Message != image.RejectionMessage)
                {
                    result += $"Added reject message: {string.Join(", ", Message)}.";
                    image.RejectionMessage = Message;
                }

                //clear rejection message if approved
                if (image.Status == ItemStatus.Approved)
                {
                    image.RejectionMessage = string.Empty;
                }

                result = string.IsNullOrEmpty(result) ? result : $"Image [#{idx + 1}] {result}\r\n";

                return result;
            }
        }

    }



}
