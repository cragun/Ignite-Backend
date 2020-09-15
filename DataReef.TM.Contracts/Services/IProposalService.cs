using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.DTOs.Blobs;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DataViews.ClientAPI;
using DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IProposalService : IDataService<Proposal>
    {
        void DeleteDocuments(List<Guid> financePlanIds, object dataContextObj, FinanceDocumentType? documentType = null);

        [OperationContract]
        ICollection<ProposalLite> GetProposalsLite(Guid ouID, DateTime startDate, DateTime endDate, bool deep = true, int pageNumber = 1, int pageSize = 1000);

        [OperationContract]
        Proposal2DataView GetProposalDataView(Guid proposalDataId, double? utilityInflationRate, bool roundAmounts = false);

        [OperationContract]
        CreateProposalDataResponse CreateProposalData(DocumentSignRequest request);

        [OperationContract]
        Proposal SignProposal(Guid proposalDataId, DocumentSignRequest request);

        //[OperationContract]
        //List<ProposalMediaItem> UploadProposalDocumentItem(Guid proposalId,string DocId, List<ProposalMediaUploadRequest> request);

        [OperationContract]
        void UpdateProposalDataJSON(Guid proposalDataId, string proposalDataJSON);

        [OperationContract]
        List<DocumentDataLink> GetProposalDocuments(Guid proposalID);

        [OperationContract]
        SBGetDocument GetOuDocumentType(Guid proposalID);

        [OperationContract]
        SBGetDocument GetDocuments(Guid propertID, int typeid);

        [OperationContract]
        List<DocumentDataLink> GetAllProposalDocuments(Guid proposalID);

        [OperationContract]
        List<ProposalMediaItem> GetProposalMediaItems(Guid proposalID);

        [OperationContract]
        List<KeyValue> GetProposalMediaItemsAsShareableLinks(Guid proposalID);

        [OperationContract]
        BlobModel GetProposalMediaItemContent(Guid proposalMediaID, bool thumb = false);

        [OperationContract]
        string UploadProposalDoc(Guid propertyID, string DocId, ProposalMediaUploadRequest request);

        [OperationContract]
        List<ProposalMediaItem> UploadProposalMediaItem(Guid proposalID, List<ProposalMediaUploadRequest> request);

        [OperationContract]
        void CopyProposalMediaItems(Guid sourceProposalID, Guid destinationProposalID);

        [OperationContract]
        void UpdateProposal(Guid proposalId, ProposalUpdateRequest request);

        [OperationContract]
        LoanResponse ReCalculateFinancing(FinancePlan financePlan);

        [OperationContract]
        void CloneProposal(ProposalCloneCoreParams req);

        [OperationContract]
        string GetAgreementForProposal(Guid proposalID);

        [OperationContract]
        List<DocType> GetDocumentType();

        [OperationContract]
        Proposal SignAgreement(Guid proposalDataId, DocumentSignRequest request);

        [OperationContract]
        List<KeyValue> GetAddersIncentives(Guid ProposalID);

        [OperationContract]
        SystemCostItem AddAddersIncentives(AdderItem adderItem, Guid ProposalID);

        [OperationContract]
        SystemCostItem UpdateQuantityAddersIncentives(AdderItem adderItem, Guid ProposalID);

        [OperationContract]
        void UpdateExcludeProposalData(string excludeProposalJSON, Guid ProposalID);

        [OperationContract]
        void DeleteAddersIncentives(Guid adderID, Guid ProposalID);

        [OperationContract]
        void UpdateProposalFinancePlan(Guid ProposalID, FinancePlan financePlan);

        [OperationContract]
        int GetProposalCount(Guid PropertyID);

        [OperationContract]
        string Getproposalpdftest(string s);

    }
}