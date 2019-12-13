using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.Auth.IdentityServer.Helpers
{
    static class AuthConstants
    {
        internal static class ErorrMessages
        {
            public const string InvalidCredentials = "Invalid credentials.";
            public const string PasswordChangeRequired = "Credential password needs to be changed.";
            public const string UserConflict = "User registered within many organizations.";
            public const string UserDoesNotExist = "User does not exist.";
            public const string UserNotRegistered = "User not registered with the requested organization.";
            public const string UserDisabled = "User is disabled.";
            public const string NullUsername = "Username cannot be null.";
            public const string UnknownError = "An error occured while trying to perform the requested operation.";
            public const string InvalidOuId = "Invalid OU id provided.";
            public const string InvalidUserId = "Invalid user id.";
            public const string CredentialNotFound = "User credential not found.";
            public const string UserAlreadyRegistered = "The user is already registered with another organization.";
            public const string CredentialAlreadyExists = "A credential with the same username already exists in the system.";
            public const string UserCredentialAlreadyExists = "A credential of the same type already defined for this user.";
            public const string InvalidPasswordHistory = "Invalid password history.";
            public const string InvalidContent = "Invalid content.";
        }

        internal static class SuccessMessages
        {
            public const string PasswordChanged = "Password successfully changed.";
        }

        public const string AuthenticationReponseKey = "AuthenticationReponse";
        public const string OuIdHeaderKey = "OuId";
        public const string TenantIdClaimType = "tid";
    }
}