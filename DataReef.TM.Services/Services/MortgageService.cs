using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using DataReef.Core.Attributes;
using DataReef.Integrations.RedBell;
using DataReef.Integrations.RedBell.RedBellBeta;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Mortgage;
using DataReef.TM.Services.InternalServices.Geo;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace DataReef.TM.Services.Services
{
    public class MortgageConfigurationKeys
    {
        public static string EsBaseUrl = "Es.Deed.BaseUrl";
        public static string EsIndex = "Es.Deed.Index";
        public static string EsTypeName = "Es.Deed.TypeName";
        public static string EsPageSize = "Es.Deed.PageSize";
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(IMortgageService))]
    public class MortgageService : IMortgageService
    {
        private readonly IGeoProvider _geoProvider;
        private readonly IRedBellAdapter _redBellAdapter;
        private readonly string _baseUrl = ConfigurationManager.AppSettings[MortgageConfigurationKeys.EsBaseUrl];
        private readonly string _index = ConfigurationManager.AppSettings[MortgageConfigurationKeys.EsIndex];
        private readonly string _typeName = ConfigurationManager.AppSettings[MortgageConfigurationKeys.EsTypeName];
        private readonly string _pageSize = ConfigurationManager.AppSettings[MortgageConfigurationKeys.EsPageSize];

        public MortgageService(IGeoProvider geoProvider, IRedBellAdapter redBellAdapter)
        {
            _geoProvider = geoProvider;
            _redBellAdapter = redBellAdapter;
        }

        public MortgageSource GetMortgageDetails(Guid propertyId)
        {
            Property property;
            using (var context = new DataContext())
            {
                property = context.Properties.FirstOrDefault(p => p.Guid == propertyId);
            }

            if (property == null)
                throw new ApplicationException("Property not found");
            if (string.IsNullOrWhiteSpace(property.ZipCode))
                throw new ApplicationException("ZipCode is invalid");
            if (string.IsNullOrWhiteSpace(property.PlusFour))
                throw new ApplicationException("PlusFour is invalid");
            if (string.IsNullOrWhiteSpace(property.HouseNumber))
                throw new ApplicationException("House number is invalid");

            var request = new MortgageRequest
            {
                State = property.State,
                ZipCode = property.ZipCode,
                ZipPlusFour = property.PlusFour,
                HouseNumber = property.HouseNumber
            };

            var mortgageDetails = _geoProvider.GetMortgageDetails(request);
            if (mortgageDetails == null)
            {
                return null;
            }

            try
            {
                var ave = _redBellAdapter.GetAveValue(new OrderRequest()
                {
                    Address = string.IsNullOrWhiteSpace(property.Address1) ? string.Empty : property.Address1,
                    City = string.IsNullOrWhiteSpace(property.City) ? mortgageDetails.City : property.City,
                    State = string.IsNullOrWhiteSpace(property.State) ? mortgageDetails.State : property.State,
                    Zip = string.IsNullOrWhiteSpace(property.ZipCode) ? mortgageDetails.ZipCode : property.ZipCode,
                    //  TODO: what should we use here
                    MonthsBack = MonthsBack.Three,
                    //  TODO: what should we use here
                    LoanNum = "",
                    //  TODO: what should we use here
                    PoolName = "Default",
                    ProductCode = ProductCode.Ave,
                    PropertyType = PropertyType.SingleFamily
                });
                mortgageDetails.Ave = ave;

                DateTime mortgageStartDate;
                if (mortgageDetails.FirstMortgageTerm.HasValue && DateTime.TryParseExact(mortgageDetails.FirstMortgageRecordingDate, "yyyymmdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out mortgageStartDate))
                {
                    var startDate = mortgageStartDate;
                    var endDate = DateTime.Today;
                    var monthsTranspired = Math.Abs((endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + (endDate.Day >= startDate.Day ? 0 : -1));

                    double pmt = Math.Abs(Financial.Pmt((double)(mortgageDetails.FirstMortgageEstimatedInterestRate / 100 / 12), mortgageDetails.FirstMortgageTerm.Value, (double)mortgageDetails.FirstMortgageAmount));
                    mortgageDetails.CurrentMortgageBalance = (decimal)Financial.FV((double)(mortgageDetails.FirstMortgageEstimatedInterestRate / 100 / 12), monthsTranspired, pmt, (double)-mortgageDetails.FirstMortgageAmount);
                    mortgageDetails.LoanToValue = mortgageDetails.CurrentMortgageBalance / ave;
                }
            }
            catch (Exception ex)
            {
                mortgageDetails.Exceptions = new List<string>
                {
                    $"Failed to get ave: {ex}"
                };
            }

            return mortgageDetails;
        }

        public MortgageResponse GetMortgageDetailsFiltered(MortgageRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var response = MakeEsRequest(request);

            return response;
        }

        private MortgageResponse MakeEsRequest(MortgageRequest request)
        {
            var address = BuildEsRequest(request);
            var content = EsRequest.FromMortgageRequest(request, _pageSize);

            var mortgageResponse = new MortgageResponse();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.PostAsJsonAsync(address, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    mortgageResponse = response.Content.ReadAsAsync<MortgageResponse>().Result;
                }

                return mortgageResponse;
            }
        }

        private string BuildEsRequest(MortgageRequest request)
        {
            var index = string.IsNullOrWhiteSpace(request.State)
                ? _index.ToLowerInvariant()
                : $"{_typeName}-{request.State}".ToLowerInvariant();
            var uriBuilder = new UriBuilder($"http://{_baseUrl}/{index}/_search");

            return uriBuilder.ToString();
        }
    }
}
