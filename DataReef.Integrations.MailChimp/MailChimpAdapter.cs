using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Attributes;
using DataReef.Integrations.Core;
using DataReef.Integrations.MailChimp.Models;
using Newtonsoft.Json;
using RestSharp;

namespace DataReef.Integrations.MailChimp
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IMailChimpAdapter))]
    public class MailChimpAdapter : IntegrationProviderBase, IMailChimpAdapter
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings[MailChimpResources.ApiKey];
        private readonly string _userList = ConfigurationManager.AppSettings[MailChimpLists.LegionUsers];
        private const string Username = "username";

        public MailChimpAdapter() : base(MailChimp.GetBaseAddress(ConfigurationManager.AppSettings[MailChimpResources.ApiKey]))
        {
        }

        public MailChimpUserDetails RegisterUser(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(_userList)) return null;

            var userDetails = GetUserDetails(emailAddress);
            if (string.IsNullOrWhiteSpace(userDetails.Id))
            {
                userDetails = AddUser(new MailChimpUser
                {
                    EmailAddress = emailAddress,
                    Status = "subscribed"
                });
            }
            else
            {
                if (userDetails.Status.Equals("unsubscribed", StringComparison.InvariantCultureIgnoreCase))
                {
                    userDetails = UpdateStatus(new MailChimpUser
                    {
                        EmailAddress = emailAddress,
                        Status = "subscribed"
                    });
                }
            }

            return userDetails;
        }

        public MailChimpUserDetails UnregisterUser(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(_userList)) return null;

            var userDetails = GetUserDetails(emailAddress);
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                userDetails = UpdateStatus(new MailChimpUser { EmailAddress = emailAddress, Status = "unsubscribed" });
            }

            return userDetails;
        }

        private MailChimpUserDetails GetUserDetails(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));

            try
            {
                var resource = string.Format(MailChimpResources.Member, _userList, MailChimp.Md5Encrypt(emailAddress));
                var result = MakeRequest(serviceUrl, resource, Method.GET, userName: Username, password: _apiKey);
                var userDetails = (MailChimpUserDetails)JsonConvert.DeserializeObject(result, typeof(MailChimpUserDetails));

                return userDetails;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error connection with MailChimp", ex);
            }
        }

        private MailChimpUserDetails AddUser(MailChimpUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var resource = string.Format(MailChimpResources.Members, _userList);
                var result = MakeRequest(serviceUrl, resource, Method.POST, payload: user, userName: Username, password: _apiKey, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());
                var userDetails = (MailChimpUserDetails)JsonConvert.DeserializeObject(result, typeof(MailChimpUserDetails));

                return userDetails;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to add MailChimp user", ex);
            }
        }

        private MailChimpUserDetails UpdateStatus(MailChimpUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var existingUser = GetUserDetails(user.EmailAddress);
                if (string.IsNullOrEmpty(existingUser.Id))
                    throw new ApplicationException("Failed to update MailChimp status, user does not exist");

                var resource = string.Format(MailChimpResources.Member, _userList, MailChimp.Md5Encrypt(user.EmailAddress));
                var result = MakeRequest(serviceUrl, resource, Method.PATCH, payload: user, userName: Username, password: _apiKey, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());
                var userDetails = (MailChimpUserDetails)JsonConvert.DeserializeObject(result, typeof(MailChimpUserDetails));

                return userDetails;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to update MailChimp user status", ex);
            }
        }
    }
}
