using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Auth;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.Persons;
using System;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [AnonymousAccess]
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        AuthenticationToken Authenticate(string userName, string password , string fcm_token);

        [OperationContract]
        AuthenticationToken AuthenticateUserBySuperAdmin(Guid personid);

        [OperationContract]
        bool updateUser(bool value,Guid userId);

        [OperationContract]
        bool IsUserActive(Guid userId);

        [OperationContract]
        bool IsUserInvitationValid(Guid invitationGuid);

        [OperationContract]
        UserInvitation GetPendingUserInvitation(Guid invitationGuid);

        [OperationContract]
        AuthenticationToken CreateUser(NewUser newUser, byte[] photo = null, string phoneNumber = null);

        [OperationContract]
        SaveResult InitiatePasswordReset(PasswordReset resetObject);

        [OperationContract]
        AuthenticationToken CompletePasswordReset(Guid resetGuid, string newPassword, string fcm_token);

        [OperationContract]
        AuthenticationToken ChangePassword(string userName, string oldPassword, string newPassword , string fcm_token);

        [OperationContract]
        string GetCurrentUserFullName();

        [OperationContract]
        bool CheckUserExist(string email);

        [OperationContract]
        SaveResult CreateUpdateUserFromSB(CreateUserDTO newUser, string[] apikey = null);
    }
}
