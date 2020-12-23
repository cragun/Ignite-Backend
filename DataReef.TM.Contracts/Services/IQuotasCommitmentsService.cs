using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.QuotasCommitments;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IQuotasCommitmentsService : IDataService<QuotasCommitment>
    {
        [OperationContract]
        AdminQuotas GetQuotasType();

        [OperationContract]
        IEnumerable<UserInvitation> GetUsersFromRoleType(Guid roleid);

        [OperationContract]
        QuotasCommitment InsertQuotas(QuotasCommitment request); 

        [OperationContract]
        List<List<object>> GetQuotasReport();

        [OperationContract]
        List<List<object>> GetQuotasReportByPerson(QuotasCommitment req);  

        [OperationContract]
        List<List<object>> GetQuotasCommitementsReport(QuotasCommitment request);

        [OperationContract]
        QuotasCommitment InsertCommitments(QuotasCommitment request);
    }
}