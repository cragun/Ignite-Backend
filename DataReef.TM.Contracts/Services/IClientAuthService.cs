using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [AnonymousAccess]
    [ServiceContract]
    public interface IClientAuthService
    {
        [OperationContract]
        ApiToken Authenticate(string apiKey, long timestamp, string signature);

        [OperationContract]
        Task<IDictionary<string, string>> ValidateToken(Guid token);

        [OperationContract]
        string ValidateIntegrationToken(Guid tokenId);
    }
}
