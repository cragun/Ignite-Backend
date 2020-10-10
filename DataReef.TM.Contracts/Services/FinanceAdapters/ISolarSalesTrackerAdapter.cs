using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard;
using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISolarSalesTrackerAdapter
    {
        [OperationContract]
        SstResponse SubmitSolarData(Guid financePlanID);

        [OperationContract]
        SstResponse SubmitLead(Guid propertyID, Guid? overrideEc = null, bool disposeRepo = true, bool IsdispositionChanged = false);

        [OperationContract]
        SBIntegrationLoginModel GetSBToken(Guid ouid);

        [OperationContract]
        SstResponse AttachProposal(Proposal proposal, Guid proposalDataId, SignedDocumentDTO proposalDoc);

        [OperationContract]
        void SignAgreement(Proposal proposal,string documentTypeId, SignedDocumentDTO proposalDoc);

        [OperationContract]
        void UploadDocumentItem(Property property, string documentTypeId, ProposalMediaItem proposalDoc);

        [OperationContract]
        SBGetDocument GetProposalDocuments(Property property);

        [OperationContract]
        SBGetDocument GetOuDocumentType(Guid ouid);
        
        [OperationContract]
        void SBActiveDeactiveUser(bool IsActive, string sbid);

        [OperationContract]
        void SetSSTSettings(SMARTBoardIntegrationOptionData sstSettings);

        [OperationContract]
        SBProposalDataModel BuildProposalDataModel(Guid proposalDataId);

        [OperationContract]
        SBProposalDataModel BuildProposalDataModelFromProposalData(ProposalData proposalData);

        [OperationContract]
        string AddUserTaggingNotification(Property property, Guid createdByID, Guid taggedID);

        [OperationContract]
        string DismissNotification(Guid ouid, string smartboardNotificationID);

        [OperationContract]
        void SBUpdateactivityUser(string SmartBoardID, string ActivityName, string BuildVersion, DateTime? LastActivityDate, Guid prsnid);
    }
}
