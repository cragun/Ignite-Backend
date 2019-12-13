using DataReef.Integrations.Google.Models;
using DataReef.Integrations.Google.Util;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Google.Helpers
{
    public class CredentialsHelper
    {
        public static ServiceAccountCredential GetCredentials()
        {
            var keyFile = File.ReadAllText(Constants.AccountKeyFilePath);
            var account = ServiceAccountModel.FromString(keyFile);

            var clientEmail = account.ClientEmail;
            var privateKey = account.PrivateKey;

            var xCred = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(clientEmail)
            {
                Scopes = Constants.SheetScopes
            }
            .FromPrivateKey(privateKey));

            return xCred;
        }
    }
}
