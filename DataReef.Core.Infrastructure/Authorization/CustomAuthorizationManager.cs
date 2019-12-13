using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;

namespace DataReef.Core.Infrastructure.Authorization
{
    public class CustomAuthorizationManager : ServiceAuthorizationManager
    {
        private static readonly Dictionary<string, Dictionary<string, bool>> CachedAnonymousOperationContracts = new Dictionary<string, Dictionary<string, bool>>();
        private static readonly object Locker = new object();
        private const string NS = "http://datareef.com";

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            BuildPrincipal();

            var hasAccess = IsAnonymousService(operationContext) || HasAccessRights(Thread.CurrentPrincipal);
            return hasAccess;
        }

        private void BuildPrincipal()
        {
            var headers = OperationContext.Current.IncomingMessageHeaders;

            DateTimeOffset deviceDate = (DateTimeOffset)DateTime.UtcNow;
            string deviceId, tenantId, ouId, accountID, clientVersion, deviceType;
            string userId = deviceId = tenantId = ouId = accountID = clientVersion = deviceType = "";

            if (headers.FindHeader(RequestHeaders.UserIDHeaderName, NS) != -1)
                userId = headers.GetHeader<string>(RequestHeaders.UserIDHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.DeviceIDHeaderName, NS) != -1)
                deviceId = headers.GetHeader<string>(RequestHeaders.DeviceIDHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.DeviceDateHeaderName, NS) != -1)
                deviceDate = headers.GetHeader<DateTimeOffset>(RequestHeaders.DeviceDateHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.TenantIDHeaderName, NS) != -1)
                tenantId = headers.GetHeader<string>(RequestHeaders.TenantIDHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.OUIDHeaderName, NS) != -1)
                ouId = headers.GetHeader<string>(RequestHeaders.OUIDHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.AccountIDHeaderName, NS) != -1)
                accountID = headers.GetHeader<string>(RequestHeaders.AccountIDHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.ClientVersionHeaderName, NS) != -1)
                clientVersion = headers.GetHeader<string>(RequestHeaders.ClientVersionHeaderName, NS);

            if (headers.FindHeader(RequestHeaders.DeviceTypeHeaderName, NS) != -1)
                deviceType = headers.GetHeader<string>(RequestHeaders.DeviceTypeHeaderName, NS);

            ClaimsIdentity claimsIdentity;

            if (!IsValidClaimValue(userId) || !IsValidClaimValue(ouId) || !IsValidClaimValue(accountID))
            {
                claimsIdentity = new ClaimsIdentity(authenticationType: null); // is not authenticated
                if (IsValidClaimValue(deviceId)) claimsIdentity.AddClaim(new Claim(DataReefClaimTypes.DeviceId, deviceId));
                if (IsValidClaimValue(clientVersion)) claimsIdentity.AddClaim(new Claim(DataReefClaimTypes.ClientVersion, clientVersion));
            }
            else
            {
                claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(DataReefClaimTypes.UserId, userId),
                    new Claim(DataReefClaimTypes.DeviceId, deviceId),
                    new Claim(DataReefClaimTypes.OuId, ouId),
                    new Claim(DataReefClaimTypes.TenantId, tenantId),
                    new Claim(DataReefClaimTypes.AccountID, accountID),
                    new Claim(DataReefClaimTypes.ClientVersion, clientVersion),
                    new Claim(DataReefClaimTypes.DeviceDate, deviceDate.ToString()),
                    new Claim(DataReefClaimTypes.DeviceType, deviceType),
                }, "ExternalBearer"); // is authenticated
            }

            System.Threading.Thread.CurrentPrincipal = new ClaimsPrincipal(claimsIdentity);
        }

        private bool IsValidClaimValue(string claimValue)
        {
            const string defaultGuid = "00000000-0000-0000-0000-000000000000";
            const string defaultInt = "0";

            return !string.IsNullOrWhiteSpace(claimValue) && !defaultGuid.Equals(claimValue) && !defaultInt.Equals(claimValue);
        }

        private bool IsAnonymousService(OperationContext operationContext)
        {
            string contractName = operationContext.EndpointDispatcher.ContractName;
            var actionMethods = ServiceModelHelper.GetActionMethod(operationContext);

            Dictionary<string, bool> serviceContractMethodAccessData;
            if (!CachedAnonymousOperationContracts.TryGetValue(contractName, out serviceContractMethodAccessData))
            {
                lock (Locker)
                {
                    bool isAnonymous;
                    if (CachedAnonymousOperationContracts.TryGetValue(contractName, out serviceContractMethodAccessData))
                    {
                        if (serviceContractMethodAccessData.TryGetValue(actionMethods.Name, out isAnonymous))
                            return isAnonymous;

                        isAnonymous = IsAnonymouseAction(operationContext, contractName, actionMethods);

                        serviceContractMethodAccessData.Add(actionMethods.Name, isAnonymous);
                        return isAnonymous;
                    }

                    isAnonymous = IsAnonymouseAction(operationContext, contractName, actionMethods);

                    CachedAnonymousOperationContracts.Add(contractName, new Dictionary<string, bool> { { actionMethods.Name, isAnonymous } });
                    return isAnonymous;
                }
            }

            bool isMethodAnonymous;
            if (serviceContractMethodAccessData.TryGetValue(actionMethods.Name, out isMethodAnonymous))
                return isMethodAnonymous;

            lock (Locker)
            {
                if (serviceContractMethodAccessData.TryGetValue(actionMethods.Name, out isMethodAnonymous))
                    return isMethodAnonymous;

                isMethodAnonymous = IsAnonymouseAction(operationContext, contractName, actionMethods);

                serviceContractMethodAccessData.Add(actionMethods.Name, isMethodAnonymous);
                return isMethodAnonymous;
            }
        }

        private static bool IsAnonymouseAction(OperationContext operationContext, string contractName, MethodInfo actionMethods)
        {
            var anonymousAnnotation = actionMethods.GetCustomAttribute(typeof(AnonymousAccessAttribute));
            if (anonymousAnnotation != null)
                return true;

            var serviceType = operationContext.Host.Description.ServiceType.GetInterface(contractName);
            // if no server contract was found return anonymous as false
            if (serviceType == null)
                return false;

            anonymousAnnotation = serviceType.GetCustomAttribute(typeof(AnonymousAccessAttribute));
            return anonymousAnnotation != null;
        }

        private static bool HasAccessRights(IPrincipal currentPrincipal)
        {
            return currentPrincipal.Identity.IsAuthenticated;
        }
    }
}