using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IAssignmentService : IDataService<Assignment>
    {
        [OperationContract]
        Task SendTerritoryAssignmentNotification(IEnumerable<Assignment> assignments);

        [OperationContract]
        List<KeyValuePair<Guid, string>> ValidatePeopleOUs(List<KeyValuePair<Guid, Guid>> data);
    }
}