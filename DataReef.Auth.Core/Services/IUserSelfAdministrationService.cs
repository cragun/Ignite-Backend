using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataReef.Auth.Core.Models;

namespace DataReef.Auth.Core.Services
{
    public interface IUserSelfAdministrationService
    {
        void ChangePassword(Guid userId, string oldPassword, string newPassword, CredentialType credentialType);
    }
}