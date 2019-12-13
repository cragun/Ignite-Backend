using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.Auth.Core.Models;

namespace DataReef.Auth.Core.Services
{
    /// <summary>
    /// Interface for administrating users of the system: creating, disabling, changing / reseting their passwords
    /// </summary>
    [ServiceContract]
    public interface IUserAdministrationService
    {
        [OperationContract]
        void CreateUser(string username, CredentialType credentialType, Guid userId, Guid ouId, string ouName, int tenantId, bool requirePasswordChange = true);

        [OperationContract]
        void EnableUser(Guid userId);
        [OperationContract]
        void EnableUsers(IList<Guid> userIds);
        [OperationContract]
        void DisableUser(Guid userId);
        [OperationContract]
        void DisableUsers(IList<Guid> userIds);

        [OperationContract]
        void ResetPassword(Guid userId, CredentialType credentialType = CredentialType.All);
        [OperationContract]
        void ResetPasswords(IList<Guid> userIds, CredentialType credentialType = CredentialType.All);

        [OperationContract]
        void RequestPasswordChange(Guid userId, CredentialType credentialType = CredentialType.All);
        [OperationContract]
        void RequestPasswordsChange(IList<Guid> userIds, CredentialType credentialType = CredentialType.All);

        [OperationContract]
        void ChangeUsername(Guid userId, string newUsername, CredentialType credentialType);
    }
}
