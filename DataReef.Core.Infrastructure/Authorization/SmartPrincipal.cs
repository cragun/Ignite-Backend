using DataReef.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace DataReef.Core.Infrastructure.Authorization
{
    public class SmartPrincipal
    {
        /// <summary>
        /// The System Guid of the Device being used.  This Guid comes from registering the Device UUID through the Devices Post
        /// </summary>
        public static Guid DeviceId
        {
            get { return GetClaim<Guid>(DataReefClaimTypes.DeviceId); }
        }

        /// <summary>
        /// The Guid of the User
        /// </summary>
        public static Guid UserId
        {
            get { return GetClaim<Guid>(DataReefClaimTypes.UserId); }
        }

        public static string UserName => Thread.CurrentPrincipal.Identity?.Name;

        /// <summary>
        /// The OUID that the user is currently operating under.   A user must access the API under a certain OU context
        /// </summary>
        public static Guid OuId
        {
            get { return GetClaim<Guid>(DataReefClaimTypes.OuId); }
        }

        /// <summary>
        /// The guid of the Account (paying entity used for billing)
        /// </summary>
        public static Guid AccountID
        {
            get { return GetClaim<Guid>(DataReefClaimTypes.AccountID); }
        }

        /// <summary>
        /// The id of the tenant
        /// </summary>
        public static int TenantId
        {
            get { return GetClaim<int>(DataReefClaimTypes.TenantId); }
        }

        /// <summary>
        /// The version of the 
        /// </summary>
        public static string ClientVersion
        {
            get { return GetClaim<string>(DataReefClaimTypes.ClientVersion); }
        }

        /// <summary>
        /// The DateTime of the Device being used.
        /// </summary>
        public static DateTimeOffset DeviceDate
        {
            get { return GetClaim<DateTimeOffset>(DataReefClaimTypes.DeviceDate); }
        }
        public static DeviceType DeviceType
        {
            get
            {
                var value = GetClaim<string>(DataReefClaimTypes.DeviceType);
                var devType = DeviceType.Unknown;
                Enum.TryParse(value, out devType);
                return devType;
            }
        }

        /// <summary>
        /// Checks if the principal is authenticated.
        /// </summary>
        public static bool IsAuthenticated
        {
            get { return System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// Checks if the principal contains the minimum expected data.
        /// </summary>
        public static bool IsValid
        {
            get
            {
                return HasClaims(
                    new[]
                {
                    DataReefClaimTypes.UserId, DataReefClaimTypes.OuId, DataReefClaimTypes.TenantId, DataReefClaimTypes.ClientVersion
                });
            }
        }

        private static T GetClaim<T>(string claimType)
        {
            var claimsPrincipal = System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal;
            if (claimsPrincipal == null)
                return default(T);

            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null)
                return default(T);

            if (typeof(T) == typeof(Guid))
                return (T)Convert.ChangeType(new Guid(claim.Value), typeof(T));

            if (typeof(T) == typeof(DateTimeOffset))
                return (T)Convert.ChangeType(String.IsNullOrEmpty(claim.Value) ? DateTime.UtcNow : DateTimeOffset.Parse(claim.Value), typeof(T));

            return (T)Convert.ChangeType(claim.Value, typeof(T));
        }

        private static IEnumerable<T> GetClaims<T>(string claimType)
        {
            var claimsPrincipal = System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal;
            return claimsPrincipal == null ? Enumerable.Empty<T>() : claimsPrincipal.Claims.Where(c => c.Type == claimType).Select(c => (T)Convert.ChangeType(c.Value, typeof(T)));
        }

        private static bool HasClaims(IEnumerable<string> claimTypes)
        {
            var claimsPrincipal = System.Threading.Thread.CurrentPrincipal as ClaimsPrincipal;
            if (claimsPrincipal == null)
                return false;

            return claimTypes.All(claimType => !claimsPrincipal.Claims.All(c => c.Type != claimType || string.IsNullOrWhiteSpace(c.Value)));
        }


        public static void ImpersonateUser(Guid userID, Guid ouID)
        {


            var identity = new ClaimsIdentity("Custom");
            var principal = new ClaimsPrincipal(identity);

            var clientVersion = "100.0.0";
            identity.AddClaim(new Claim(DataReefClaimTypes.ClientVersion, clientVersion));

            var deviceId = Guid.Empty;
            identity.AddClaim(new Claim(DataReefClaimTypes.DeviceId, Guid.NewGuid().ToString()));

            var deviceDate = System.DateTime.Now;
            identity.AddClaim(new Claim(DataReefClaimTypes.DeviceDate, deviceDate.ToString()));
            identity.AddClaim(new Claim(DataReefClaimTypes.UserId, userID.ToString()));

            identity.AddClaim(new Claim(DataReefClaimTypes.TenantId, "0"));
            identity.AddClaim(new Claim(DataReefClaimTypes.OuId, ouID.ToString()));
            identity.AddClaim(new Claim(DataReefClaimTypes.AccountID, Guid.NewGuid().ToString()));

            Thread.CurrentPrincipal = principal;

        }


    }
}