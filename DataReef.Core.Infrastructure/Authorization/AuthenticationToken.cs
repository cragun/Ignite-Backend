using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace DataReef.TM.Contracts.Auth
{
    [NotMapped]
    public class AuthenticationToken
    {

        public AuthenticationToken()
        {
           
        }

        /// <summary>
        /// The Users Guid according to the application
        /// </summary>
        /// 
        [DataMember]
        public Guid UserID { get; set; }

        /// <summary>
        /// The guid of the account that the user can access with this token.  User must use API in the context of an account
        /// </summary>
        [DataMember]
        public Guid AccountID { get; set; }

        /// <summary>
        /// String that represents the intended audience of the token
        /// </summary>
        [DataMember]
        public string Audience { get; set; }

        /// <summary>
        /// Seconds since Epoch.  Unix Time
        /// </summary>
        [DataMember]
        public long Expiration { get; set; }

        /// <summary>
        /// A string unique to the client application
        /// </summary>
        [DataMember]
        public string ClientSecret { get; set; }

        /// <summary>
        /// A token used to automatically refresh auth
        /// </summary>
        [DataMember]
        public string RefreshToken { get; set; }


        public static AuthenticationToken FromEncryptedString(string token, X509Certificate2 decryptionCertificate)
        {
            RSACryptoServiceProvider rsa = decryptionCertificate.PrivateKey as RSACryptoServiceProvider;
            byte[] encryptedBytes = Convert.FromBase64String(token);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, true);

            string jsonToken = System.Text.UTF8Encoding.UTF8.GetString(decryptedBytes);
            return JsonConvert.DeserializeObject<AuthenticationToken>(jsonToken);
        }
    }
}
