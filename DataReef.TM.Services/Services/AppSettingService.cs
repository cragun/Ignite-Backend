using DataReef.Core;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations.LoanPal;
using DataReef.Integrations.PushNotification;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class AppSettingService : DataService<AppSetting>, IAppSettingService
    {
        private Lazy<IAPNPushBridge> _pushBridge;
        private Lazy<ILoanPalBridge> _loanPalBridge;
        public AppSettingService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory, Lazy<IAPNPushBridge> pushBridge, Lazy<ILoanPalBridge> loanPalBridge)
            : base(logger, unitOfWorkFactory)
        {
            _pushBridge = pushBridge;
            _loanPalBridge = loanPalBridge;
        }


        public string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            using (var dataContext = new DataContext())
            {
                var entity = dataContext
                            .AppSettings
                            .AsNoTracking()
                            .FirstOrDefault(a => a.Key == key);

                return entity == null ? null : entity.Value;
            }
        }

        public void SetValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            using (var dataContext = new DataContext())
            {
                var entity = dataContext
                            .AppSettings
                            .FirstOrDefault(a => a.Key == key);

                if (entity == null)
                {
                    entity = new AppSetting
                    {
                        Key = key,
                        Value = value
                    };
                    dataContext
                        .AppSettings
                        .Add(entity);
                }
                else
                {
                    entity.Value = value;
                }

                dataContext.SaveChanges();
            }
        }

        public bool IsHealthy()
        {
            //try
            //{
            //    var nosql = new NoSQLDataService();
            //    //var value = "{\"Guid\":\"18f7a864-1a77-455c-82d5-b54efe9e3b15\",\"Discriminator\":\"OU\",\"CentroidLat\":45.7098961,\"CentroidLon\":-113.022423,\"Radius\":6610888.0,\"BatchPrescreenTableName\":\"BatchPrescreenData_SolarPanels\",\"IsDisabled\":false,\"AccountID\":\"4185317c-6dd9-4591-8561-162cc09921ec\",\"Summary\":{\"PersonCount\":34,\"TodayCount\":0,\"TerritoryCount\":57,\"SubOUsCount\":11,\"SalesToday\":0,\"SalesThisWeek\":0,\"SalesThisMonth\":0,\"SalesThisYear\":0,\"SalesAllTime\":52},\"IsDeletableByClient\":false,\"RootOrganizationID\":\"18f7a864-1a77-455c-82d5-b54efe9e3b15\",\"RootOrganizationName\":\"DataReef Solar\",\"ShapesVersion\":1,\"TokenPriceInDollars\":0.3,\"ActivityTypes\":1,\"IsArchived\":false,\"Addresses\":[],\"PhoneNumbers\":[],\"RootOrganization\":{\"Guid\":\"18f7a864-1a77-455c-82d5-b54efe9e3b15\",\"Discriminator\":\"OU\"},\"IsRoot\":true,\"Id\":21176,\"Name\":\"DataReef Solar\",\"TenantID\":0,\"DateCreated\":\"2014-12-05T20:54:11\",\"DateLastModified\":\"2017-05-11T14:15:33.487\",\"LastModifiedBy\":\"965658bb-909c-47da-9fae-9962e166a1a2\",\"Version\":1,\"IsDeleted\":false}";
            //    //nosql.PutValue(value);
            //    var value = nosql.GetValue<OU>("18f7a864-1a77-455c-82d5-b54efe9e3b15");
            //}
            //catch (Exception)
            //{
            //}
            //try
            //{
            using (var dataContext = new DataContext())
            {
                var result = dataContext.AppSettings.Any();
                return true;
            }
            //}
            //catch
            //{
            //    return false;
            //}
        }

        public int GetLoginDays()
        {
            var loginDays = GetValue(Constants.LoginDays);
            return int.Parse(loginDays);
        }


        public Version GetMinimumRequiredVersionForIPad()
        {
            var versionString = GetValue(Constants.IPadMinimumVersionSettingName);
            return Version.Parse(versionString);
        }

        public bool TestPushNotifications(string token, string payload)
        {
            _pushBridge.Value.Test(token, payload);
            return true;
        }

        public override ICollection<AppSetting> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            filter = string.IsNullOrEmpty(filter) ? "VisibleToClients=true" : filter;
            return base.List(deletedItems, pageNumber, itemsPerPage, filter, include, exclude, fields);
        }
    }
}
