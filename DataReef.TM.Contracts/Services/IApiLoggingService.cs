using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IApiLoggingService
    {
        [OperationContract]
        [AnonymousAccess]
        void LogToDatabase(ApiLogEntry apiLogEntry);
    }
}