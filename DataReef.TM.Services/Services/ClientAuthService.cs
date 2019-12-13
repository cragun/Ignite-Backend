using DataReef.Core.Logging;
using DataReef.TM.Contracts.Faults;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core;
using Newtonsoft.Json;

namespace DataReef.Application.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class ClientAuthService : IClientAuthService
    {
        private const int TOKEN_EXPIRATION_MINUTES = 60 * 24;
        private ILogger logger = null;
        private Lazy<IOUAssociationService> _ouAssociationService;

        public ClientAuthService(ILogger logger,
            Lazy<IOUAssociationService> ouAssociationService)
        {
            this.logger = logger;
            _ouAssociationService = ouAssociationService;
        }

        private string CalculateSignature(string apiKey, string secretKey, long timeStamp)
        {

            // Generate the hash
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            HMACMD5 hmac = new HMACMD5(encoding.GetBytes(secretKey));
            byte[] hash = hmac.ComputeHash(encoding.GetBytes(apiKey + timeStamp));

            // Convert hash to digital signature string
            string signature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return signature;
        }

        public ApiToken Authenticate(string apiKey, long timestamp, string signature)
        {
            ApiToken ret = null;

            using (var dc = new DataContext())
            {

                ApiKey key = dc.ApiKeys.Where(kk => kk.AccessKey == apiKey).FirstOrDefault();
                if (key == null)
                {
                    string reason = "Invalid Api Key";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }
                if (key.IsDisabled == true)
                {
                    string reason = "Api Key Is Disabled";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }
                if (key.IsDeleted == true)
                {
                    string reason = "Api Key Has Been Deleted";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }


                System.DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                System.DateTime dateTime = epoch.AddSeconds(timestamp);

                var elapsedtime = System.DateTime.UtcNow.Subtract(dateTime);
                if (elapsedtime.TotalMinutes > 5)
                {
                    string reason = "TimeStamp has expired.";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);
                }

                if (this.CalculateSignature(key.AccessKey, key.SecretKeyHash, timestamp) != signature)
                {
                    string reason = "Invalid Signature";
                    throw new FaultException<PreconditionFailedFault>(new PreconditionFailedFault(100, reason), reason);

                }

                ret = new ApiToken();
                ret.ApiKeyID = key.Guid;
                ret.ApiKey = key;
                ret.DateCreated = System.DateTime.UtcNow;
                ret.ExpirationDate = System.DateTime.UtcNow.AddHours(3);
                dc.ApiTokens.Add(ret);
                dc.SaveChanges();

            }
            return ret;
        }

        public string ValidateIntegrationToken(Guid tokenId)
        {
            using (var dc = new DataContext())
            {
                var token = dc
                            .ApiIntegrationTokens
                            .Include(it => it.Person)
                            .FirstOrDefault(t => t.Guid == tokenId);


                if (token == null || token.IsDeleted || token.ExpirationDate < DateTime.UtcNow)
                {
                    return null;
                }

                var associations = _ouAssociationService
                                        .Value
                                        .SmartList(filter: $"PersonID={token.UserId}", itemsPerPage: 1000)
                                        .ToList();
                var userData = associations.Select(a => new
                {
                    OUID = a.OUID,
                    OUName = a.OU.Name,
                    Role = a.OURole.Name
                });

                var ret = new Dictionary<string, object>
                {
                    { "UserId", token.UserId },
                    { "Email", token?.Person?.EmailAddressString},
                    { "UserRoles",  userData}
                };

                return JsonConvert.SerializeObject(ret);
            }
        }


        public async Task<IDictionary<string, string>> ValidateToken(Guid guid)
        {
            // public class DataReefClaimTypes
            //public const string UserId = "http://datareef.com/claims/userid";
            //public const string DeviceId = "http://datareef.com/claims/deviceid";
            //public const string OuId = "http://datareef.com/claims/ouid";
            //public const string AccountID = "http://datareef.com/claims/accountid";
            //public const string TenantId = "http://datareef.com/claims/tenantid";
            //public const string ClientVersion = "http://datareef.com/claims/clientversion";
            //public const string DeviceDate = "http://datareef.com/claims/devicedate";


            using (var dc = new DataContext())
            {
                // note the FirstOrDefaultAsync and the await prefix
                var token = await dc.ApiTokens
                                    .Include("ApiKey.OU")
                                    .FirstOrDefaultAsync(e => e.Guid == guid);



                if (token != null && token.ExpirationDate != null && token.ExpirationDate > System.DateTime.UtcNow)
                {
                    return new Dictionary<string, string>
                    {
                        { DataReefClaimTypes.OuId, token.ApiKey.OUID.ToString()  },
                        { DataReefClaimTypes.AccountID,token.ApiKey.OU.AccountID.ToString() },
                        { DataReefClaimTypes.UserId,Guid.NewGuid().ToString() },
                        { DataReefClaimTypes.TenantId,1.ToString() },
                        { DataReefClaimTypes.ClientVersion,"1000000"},
                        { DataReefClaimTypes.DeviceDate,System.DateTime.UtcNow.ToShortDateString() },

                    };
                }
            }

            return null;
        }
    }
}
