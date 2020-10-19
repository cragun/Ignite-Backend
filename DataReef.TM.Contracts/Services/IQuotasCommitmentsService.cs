using DataReef.TM.Models;
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
        IEnumerable<UserInvitation> GetUsersFromRoleType(Guid roleid,Guid ouID);

        [OperationContract]
        QuotasCommitment InsertQuotas(QuotasCommitment request);
        
    }
}