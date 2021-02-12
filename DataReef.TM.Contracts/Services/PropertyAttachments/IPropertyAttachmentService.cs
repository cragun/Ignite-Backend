using DataReef.TM.Models.DataViews.Geo;
using DataReef.TM.Models.DTOs.PropertyAttachments;
using DataReef.TM.Models.PropertyAttachments;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPropertyAttachmentService : IDataService<PropertyAttachment>
    {
        [OperationContract]
        PropertyAttachmentItemDTO UploadImage(Guid propertyAttachmentID, Guid? propertyAttachmentItemID, string sectionID, string itemID, List<string> images, List<ImageBase64WithNotes> imagesWithNotes, GeoPoint location);


        [OperationContract]
        PropertyAttachmentItemDTO DeleteImage(Guid propertyAttachmentItemID, Guid imageID);

        [OperationContract]
        PropertyAttachmentItemDTO EditImageNotes(Guid propertyAttachmentItemID, Guid imageID, string notes);

        [OperationContract]
        ExtendedPropertyAttachmentDTO GetPropertyAttachmentData(Guid propertyAttachmentID);

        [OperationContract]
        IEnumerable<ExtendedPropertyAttachmentDTO> GetPropertyAttachmentsForProperty(Guid propertyID);

        [OperationContract]
        void UpdatePropertyAttachment(Guid guid, EditPropertyAttachmentRequest request);

        [OperationContract]
        bool SubmitPropertyAttachment(Guid guid);

        [OperationContract]
        bool ReviewPropertyAttachment(Guid guid, PropertyAttachmentSubmitReviewRequest request);

        [OperationContract]
        Task<ICollection<ExtendedPropertyAttachmentDTO>> GetPagedPropertyAttachments(int pageIndex, int pageSize, string query);

        [OperationContract]
        bool SubmitPropertyAttachmentSection(Guid guid, string sectionId);

        [OperationContract]
        bool SubmitPropertyAttachmentTask(Guid guid, string sectionId, string taskId);

        [OperationContract]
        IEnumerable<SBAttachmentDataDTO> GetSBAttachmentData(long propertyID, string apiKey);

        [OperationContract]
        PropertyAttachmentItemDTO UploadUtilityBillImage(Guid PropertyId, UploadImageToPropertyAttachmentRequest uploadImageRequest);
    }
}
