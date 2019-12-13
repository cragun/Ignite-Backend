using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISignatureService
    {
        [OperationContract]
        List<SignDocumentResponse> SignDocument(SignDocumentRequest request, Guid ouid, Guid financePlanDefinitionId, FinancePlanType financePlanType, FinanceDocumentType documentType);

        [OperationContract]
        SignDocumentResponseHancock SignDocumentHancock(SignDocumentRequestHancock request, Guid ouid, FinanceDocumentType documentType);

        [OperationContract]
        [AnonymousAccess]
        void RightSignatureContractCallback(Callback callback);

        [OperationContract]
        void SendEmailReminder(string contractID);

        [OperationContract]
        void SendSignedDocumentEmail(FinanceDocument documents);

        [OperationContract]
        void SendPricingRequestEmail(FinanceDocument document);

        [OperationContract]
        void SendDocumentsEmail(List<FinanceDocument> documents, string bodyFormat);
    }
}