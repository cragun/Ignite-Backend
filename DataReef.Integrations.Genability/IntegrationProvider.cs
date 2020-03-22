using DataReef.Integrations.Core;
using DataReef.Integrations.Core.Models;
using DataReef.Integrations.Genability.Utils;
using DataReef.TM.Models.DTOs.Solar;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Genability;
using DataReef.TM.Models.DTOs.Solar.Genability.Enums;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DataReef.Integrations.Genability
{
    public class IntegrationProvider
    {
        private const string applicationJsonContentType = "application/json";
        private const string appIdParameter = "appId";
        private const string appKeyParameter = "appKey";
        private const string zipCodeParameter = "zipCode";
        private const string residentialServiceTypesParameter = "residentialServiceTypes";
        private const string masterTariffIdParameter = "masterTariffId";
        private const string lseIdParameter = "lseId";
        private const string providerAccountIdParameter = "providerAccountId";
        private const string customerClassPropertyName = "customerClass";
        private const string consumptionValue = "consumption";
        private const string kWhValue = "kWh";
        private const int monthsInYear = 12;
        private readonly string serviceUrl;
        private readonly ThirdPartyCredentials Credentials;

        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Delegate used to record calls made to 3rd party services
        /// </summary>
        public Action<string, string, string, string> OnRequest { get; set; }

        public IntegrationProvider(string serviceUrl, Action<string, string, string, string> onRequest, ThirdPartyCredentials credentials = null)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentNullException("serviceUrl");
            }

            Credentials = credentials;
            this.serviceUrl = serviceUrl;
            ErrorMessage = String.Empty;
            OnRequest = onRequest;
        }

        /// <summary>
        /// Method for creating a Genability account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="request">The CreateAccountRequest object that encapsulates the account's information.</param>
        /// <returns>Returns an CreateAccountResponse object.</returns>
        public CreateAccountResponse CreateAccount(string genabilityAppID, string genabilityAppKey, CreateAccountRequest request)
        {
            List<Parameter> parameters = new List<Parameter> {
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
           };

            if (request.properties == null)
            {
                request.properties = new AccountProperties();
            }

            //preset the customer to residential customer
            request.properties.customerClass = new AccountProperty
            {
                keyName = customerClassPropertyName,
                dataValue = ((int)CustomerClass.RESIDENTIAL).ToString()
            };

            CreateAccountResponse response = APIClient.MakeRequest<CreateAccountResponse>(serviceUrl, Endpoints.createAccountResource,
                                                                     Method.PUT, payload: request, parameters: parameters,
                                                                     serializer: new RestSharpJsonSerializer());
            //LogRequest(Constants.CreateAccount, Endpoints.createAccountResource, request, response);

            return response;
        }

        /// <summary>
        /// Gets PriceResult information for a given account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <returns>Returns an PriceResult object.</returns>
        public PriceResult GetAccountPriceResult(string genabilityAppID, string genabilityAppKey, string providerAccountId, string zipCode, bool? usagecollected)
        {
            GetAccountRecommendedPrice recommendedPrice = GetAccountRecommendedPrice(genabilityAppID, genabilityAppKey, providerAccountId);

            // try to get the data based on zip code
            if (recommendedPrice == null && !string.IsNullOrEmpty(zipCode))
            {
                recommendedPrice = GetRecommendedPriceForZipCode(genabilityAppID, genabilityAppKey, zipCode);
            }

            var price = recommendedPrice?.results?.FirstOrDefault();

            if (price == null)
            {
                return null;
            }

            List<Tariff> accountTariffs = this.GetAccountTariffs(genabilityAppID, genabilityAppKey, providerAccountId).results;
            Tariff currentTariff = accountTariffs.FirstOrDefault(at => at.masterTariffId.Equals(price.masterTariffId));
            PriceResult priceResult = currentTariff != null ? new PriceResult
            {
                PricePerKWH = price.rateMean,
                TariffID = currentTariff.tariffId,
                MasterTariffID = currentTariff.masterTariffId,
                TariffName = currentTariff.tariffName,
                TariffCode = currentTariff.tariffCode,
                UtilityID = currentTariff.lseId,
                UtilityName = currentTariff.lseName,
                UsageCollected = usagecollected
            } : new PriceResult();

            return priceResult;
        }

        /// <summary>
        /// Method for getting the recommended price(the rate of the account's tariff) for a given account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <returns>Returns an GetAccountRecommendedPrice object.</returns>
        public GetAccountRecommendedPrice GetAccountRecommendedPrice(string genabilityAppID, string genabilityAppKey, string providerAccountId)
        {
            List<Parameter> parameters = new List<Parameter> {
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = providerAccountIdParameter,
                                    Value = providerAccountId
                                }
            };

            string error = null;
            GetAccountRecommendedPrice response = APIClient.MakeRequest<GetAccountRecommendedPrice>(serviceUrl, Endpoints.getSmartPriceResource,
                                                                        Method.GET, out error, parameters: parameters);
            object logResponse = response;
            if (response == null)
            {
                logResponse = error;
            }

            //LogRequest(Constants.GetRecommendedPrice, Endpoints.getSmartPriceResource, parameters, logResponse);
            return response;
        }

        /// <summary>
        /// Method for getting the recommended price(the rate of the account's tariff) for a given account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="zipCode">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <returns>Returns an GetAccountRecommendedPrice object.</returns>

        public GetAccountRecommendedPrice GetRecommendedPriceForZipCode(string genabilityAppID, string genabilityAppKey, string zipCode)
        {
            List<Parameter> parameters = new List<Parameter> {
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = zipCodeParameter,
                                    Value = zipCode
                                }
            };

            string error = null;
            GetAccountRecommendedPrice response = APIClient.MakeRequest<GetAccountRecommendedPrice>(serviceUrl, Endpoints.getSmartPriceResource,
                                                                        Method.GET, out error, parameters: parameters);
            object logResponse = response;
            if (response == null)
            {
                logResponse = error;
            }

            //LogRequest(Constants.GetRecommendedPrice, Endpoints.getSmartPriceResource, parameters, logResponse);
            return response;
        }

        /// <summary>
        /// Method for getting the tariffs for a given account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <returns>Returns an GetTariffsResponse object.</returns>
        public GetTariffsResponse GetAccountTariffs(string genabilityAppID, string genabilityAppKey, string providerAccountId)
        {
            List<Parameter> parameters = new List<Parameter> {
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
            };

            string error = null;
            GetTariffsResponse response = APIClient.MakeRequest<GetTariffsResponse>(serviceUrl,
                                                                        String.Format(Endpoints.getAccountTariffsResource, providerAccountId),
                                                                        Method.GET, parameters: parameters, error: out error);
            object logResponse = response;
            if (response == null)
            {
                logResponse = error;
            }

            //LogRequest(Constants.GetTarrifs, Endpoints.getAccountTariffsResource, parameters, logResponse);
            return response;
        }

        /// <summary>
        /// Method for changing the LSE and Tariff for a given account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <param name="lseId">The ID of the LSE.</param>
        /// <param name="masterTariffId">The ID of the Tariff.</param>
        /// <returns>Returns an CreateAccountResponse object.</returns>
        public CreateAccountResponse ChangeAccountLseAndTariff(string genabilityAppID, string genabilityAppKey, string providerAccountId, string lseId, string masterTariffId)
        {
            this.ChangeAccountProperty(genabilityAppID, genabilityAppKey, providerAccountId, lseIdParameter, lseId);
            this.ChangeAccountProperty(genabilityAppID, genabilityAppKey, providerAccountId, masterTariffIdParameter, masterTariffId);

            return this.GetGenabilityAccount(genabilityAppID, genabilityAppKey, providerAccountId);
        }

        /// <summary>
        /// Method for changig the property of an account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <param name="keyName">The name of the property.</param>
        /// <param name="keyValue">The new value of the property.</param>
        /// <returns>Returns an ChangeAccountPropertyResponse object.</returns>
        public ChangeAccountPropertyResponse ChangeAccountProperty(string genabilityAppID, string genabilityAppKey, string providerAccountId, string keyName, string keyValue)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
            };

            var request = new AccountProperty { keyName = keyName, dataValue = keyValue };

            ChangeAccountPropertyResponse response = APIClient.MakeRequest<ChangeAccountPropertyResponse>(serviceUrl,
                                                                        String.Format(Endpoints.changeAccountPropertyResource, providerAccountId),
                                                                        Method.PUT,
                                                                        payload: request,
                                                                        parameters: parameters,
                                                                        serializer: new RestSharpJsonSerializer());
            //LogRequest(Constants.ChangeAccountProperty, Endpoints.changeAccountPropertyResource, request, response);
            return response;
        }

        /// <summary>
        /// Extracts a list of  MonthDetails from the BaselineMeasurements of a GetUsageProfileResponse.
        /// </summary>
        /// <param name="profile">The UpsertUsageProfileResponse containing the monthly BaselineMeasurements.</param>
        /// <returns>A list of MonthDetails.</returns>
        public List<EnergyMonthDetails> ExtractMonthDetailsFromPVWatts5UsageProfile(UpsertUsageProfileResponse profile)
        {
            return profile.results.First().baselineMeasures.Select(bm => new EnergyMonthDetails
            {
                Production = bm.v,
                Month = bm.i
            }).ToList();
        }

        /// <summary>
        /// Method for retrieving a usage profile.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <param name="providerProfileId">The usage profile provider ID of the profile.</param>
        /// <returns>Returns an GetUsageProfileResponse response object.</returns>
        public GetUsageProfileResponse GetUsageProfileForProviderProfileId(string providerAccountId, string providerProfileId, List<Parameter> additionalParameters = null)
        {
            var parameters = GetAuthParams();
            parameters.Add(new Parameter
            {
                Type = ParameterType.QueryString,
                Name = providerAccountIdParameter,
                Value = providerAccountId
            });

            if (additionalParameters != null)
            {
                parameters.AddRange(additionalParameters);
            }


            GetUsageProfileResponse response = APIClient.MakeRequest<GetUsageProfileResponse>(serviceUrl,
                                                                        String.Format(Endpoints.getUsageProfileForProviderProfileIdResource, providerProfileId),
                                                                        Method.GET,
                                                                        parameters: parameters);
            //LogRequest(Constants.GetUsageProfile, Endpoints.getUsageProfileForProviderProfileIdResource, parameters, response);
            return response;
        }

        public CostCalculatorResponse CalculateCostForAccount(string providerAccountId, string profileId, string startDate, string endDate)
        {
            var parameters = GetAuthParams();
            var resourcePath = String.Format(Endpoints.calculateUsageForProviderAccountResource, providerAccountId);
            var payload = new CostCalculatorRequest
            {
                fromDateTime = startDate,
                toDateTime = endDate,
                profileId = profileId,
                billingPeriod = Boolean.FalseString,
                minimums = true,
                groupBy = "MONTH",
                detailLevel = "TOTAL"
            };

            var response = APIClient.MakeRequest<CostCalculatorResponse>(serviceUrl,
                                                                        resourcePath,
                                                                        Method.POST,
                                                                        payload: payload,
                                                                        parameters: parameters,
                                                                        serializer: new RestSharpJsonSerializer());

            //LogRequest(Constants.CalculatePriceForConsumption, resourcePath, parameters, response);
            return response;
        }

        public CostCalculatorResponse CalculateConsumptionForAccount(string providerAccountId, string amount, string startDate, string endDate, string masterTarrifId)
        {
            var parameters = GetAuthParams();
            var payload = new CostCalculatorRequest
            {
                providerAccountId = providerAccountId,
                fromDateTime = startDate,
                toDateTime = endDate,
                groupBy = "MONTH",
                detailLevel = "TOTAL",
                minimums = false,
                billingPeriod = Boolean.FalseString,
                tariffInputs = new List<PropertyData>
                {
                    new PropertyData
                    {
                        fromDateTime = startDate,
                        toDateTime = endDate,
                        keyName = "total",
                        dataValue = amount,
                        unit = "cost",
                    },
                    new PropertyData
                    {
                        fromDateTime = startDate,
                        toDateTime = endDate,
                        keyName = "baselineType",
                        dataValue = "typicalElectricity",
                    },
                },
            };

            var endpoint = string.IsNullOrEmpty(masterTarrifId) ? Endpoints.calculateCostGetConsumptionResource : $"{Endpoints.calculateCostGetConsumptionResource}/{masterTarrifId}";

            var response = APIClient.MakeRequest<CostCalculatorResponse>(serviceUrl,
                                                                        endpoint,
                                                                        Method.POST,
                                                                        payload: payload,
                                                                        parameters: parameters,
                                                                        serializer: new RestSharpJsonSerializer());

            //LogRequest(Constants.GetUsageProfile, endpoint, parameters, response);
            return response;
        }

        /// <summary>
        /// Method for checking if a usage profile exists.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <param name="providerProfileId">The usage profile provider ID of the profile.</param>
        /// <returns>Flag telling if the usage profile exists or not.</returns>
        public bool CheckUsageProfileExistence(string providerAccountId, string providerProfileId)
        {
            GetUsageProfileResponse response = this.GetUsageProfileForProviderProfileId(providerAccountId, providerProfileId);

            return response.status.Equals(GenabilityResponseStatus.success.ToString());
        }

        /// <summary>
        /// Method for creating/updating an electricity(utility) usage profile.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <param name="providerProfileId">The usage profile provider ID for the profile to be created/updated.</param>
        /// <param name="profileName">The name of the usage profile.</param>
        /// <param name="months">The months information the usage profile is storing.</param>
        /// <returns>Returns an UpsertUsageProfileResponse response object.</returns>
        public UpsertUsageProfileResponse UpsertElectricityProfile(string genabilityAppID, string genabilityAppKey, string providerAccountId, string providerProfileId, string profileName, List<EnergyMonthDetails> months)
        {
            UpsertUsageProfileModel model = new UpsertUsageProfileModel
            {
                GenabilityAppID = genabilityAppID,
                GenabilityAppKey = genabilityAppKey,
                ProviderAccountId = providerAccountId,
                ProviderProfileId = providerProfileId,
                ProfileName = profileName,
                ServiceType = ServiceType.ELECTRICITY,
                SourceId = SourceIdOptions.ReadingEntry.ToString(),
                Months = months,
                Source = null,
                Properties = null
            };


            return this.UpsertUsageProfile(model);

        }

        public UpsertUsageProfileResponse UpsertSolarProfileWithIntegratedPVWatts(string genabilityAppID, string genabilityAppKey, string providerAccountId, string providerProfileId, UpsertPVWattsUsageProfileRequestModel model)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
            };

            Source source = new Source
            {
                sourceVersion = GenabilityDefaultParameterValues.PVWatts5SourceVersion,
                sourceId = SourceIdOptions.PVWatts.ToString()
            };

            PVWatts5UsageProfileProperties properties = new PVWatts5UsageProfileProperties
            {
                azimuth = new PropertyData(PVWatts5UsageProfilePropertyInputs.azimuth, model.Azimuth),
                systemSize = new PropertyData(PVWatts5UsageProfilePropertyInputs.systemSize, model.SystemSize),
                tilt = new PropertyData(PVWatts5UsageProfilePropertyInputs.tilt, model.Tilt),
                moduleType = new PropertyData(PVWatts5UsageProfilePropertyInputs.moduleType, model.ModuleType),
                losses = new PropertyData(PVWatts5UsageProfilePropertyInputs.losses, model.Losses),
                inverterEfficiency = new PropertyData(PVWatts5UsageProfilePropertyInputs.inverterEfficiency, model.InverterEfficiency),
                climateDataSearchRadius = new PropertyData(PVWatts5UsageProfilePropertyInputs.climateDataSearchRadius, "0"),
                DCACRatio = new PropertyData(PVWatts5UsageProfilePropertyInputs.DCACRatio, "1.3"),
                gcr = new PropertyData(PVWatts5UsageProfilePropertyInputs.gcr, "0.4"),
                climateDataset = new PropertyData(PVWatts5UsageProfilePropertyInputs.climateDataset, "tmy3"),
                trackMode = new PropertyData(PVWatts5UsageProfilePropertyInputs.trackMode, "0"),
            };

            PVWatts5UsageProfile request = new PVWatts5UsageProfile
            {
                providerAccountId = providerAccountId,
                providerProfileId = providerProfileId,
                serviceTypes = ServiceType.SOLAR_PV.ToString(),
                properties = properties,
                source = source,
                groupBy = GroupBy.MONTH.ToString()
            };

            if (!String.IsNullOrWhiteSpace(model.ProfileName))
            {
                request.profileName = model.ProfileName;
            }

            UpsertUsageProfileResponse response = APIClient.MakeRequest<UpsertUsageProfileResponse>(serviceUrl,
                                                                                            Endpoints.upsertUsageProfileResource,
                                                                                            Method.PUT,
                                                                                            payload: request,
                                                                                            parameters: parameters,
                                                                                            serializer: new RestSharpJsonSerializer());
            LogRequest(Constants.UpsertSolarProfileWithIntegratedPVWatts, Endpoints.upsertUsageProfileResource, request, response);

            return response;

        }

        /// <summary>
        /// Method for creating/updating a usage profile.
        /// </summary>
        /// <param name="model">UpsertUsageProfileModel object encapsulating the request properties.</param>
        /// <returns>Returns an UpsertUsageProfileResponse response object.</returns>
        public UpsertUsageProfileResponse UpsertUsageProfile(UpsertUsageProfileModel model)
        {
            List<ReadingData> readingData = new List<ReadingData>();
            DateTime fromDate;
            bool isCurrentYear;

            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = model.GenabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = model.GenabilityAppKey
                                }
            };


            foreach (var month in model.Months)
            {
                isCurrentYear = DateTime.Now.Month >= month.Month;
                fromDate = month.Year != 0 ?
                                    new DateTime(month.Year, month.Month, 1) :
                                    new DateTime(isCurrentYear ? DateTime.Now.Year : (DateTime.Now.Year - 1), month.Month, 1);

                // if the serviceType is ELECTRICTY, then the consumption values are passed; 
                // if the profile is SOLAR, then the production values are passed
                readingData.Add(new ReadingData
                {
                    fromDateTime = fromDate.ToString("o"),
                    toDateTime = fromDate.AddMonths(1).ToString("o"),
                    quantityUnit = kWhValue,
                    quantityValue = model.ServiceType.Equals(ServiceType.ELECTRICITY) ? month.Consumption.ToString() : month.Production.ToString()
                });
            }

            UsageProfile request = new UsageProfile
            {
                providerAccountId = model.ProviderAccountId,
                providerProfileId = model.ProviderProfileId,
                serviceTypes = model.ServiceType.ToString(),
                readingData = readingData,
                properties = model.Properties,
                sourceId = model.SourceId,
                source = model.Source,
                groupBy = model.GroupBy
            };

            if (!String.IsNullOrWhiteSpace(model.ProfileName))
            {
                request.profileName = model.ProfileName;
            }


            UpsertUsageProfileResponse response = APIClient.MakeRequest<UpsertUsageProfileResponse>(serviceUrl,
                                                                                            Endpoints.upsertUsageProfileResource,
                                                                                            Method.PUT,
                                                                                            payload: request,
                                                                                            parameters: parameters,
                                                                                            serializer: new RestSharpJsonSerializer());
            //LogRequest(Constants.UpsertUsageProfile, Endpoints.upsertUsageProfileResource, request, response);
            return response;

        }

        /// <summary>
        /// Method for generating the power purchase agreement.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The provider account ID of the acccount the usage profile belongs to.</param>
        /// <param name="request">The PowerPurchaseAgreementRequest object that encapsulates solar information.</param>
        /// <param name="solarCharge">The charge for solar power.</param>
        /// <returns>Returns the PowerPurchaseAgreementResponse object.</returns>
        public PowerPurchaseAgreementResponse GeneratePowerPurchaseAgreement(string genabilityAppID, string genabilityAppKey, string providerAccountId, PowerPurchaseAgreementRequest request, decimal solarCharge)
        {
            PowerPurchaseAgreementResponse response = new PowerPurchaseAgreementResponse();
            var monthsDetails = new List<EnergyMonthDetails>();

            DateTime? fromDate = null;
            if (monthsDetails.Count != 0)
            {
                var oldestMonth = monthsDetails.OrderBy(md => md.Year).ThenBy(md => md.Month).FirstOrDefault();
                if (oldestMonth != null)
                {
                    fromDate = new DateTime(oldestMonth.Year, oldestMonth.Month, 1);
                }
            }

            SavingAnalysisRequestModel savingAnalysisRequestModel = new SavingAnalysisRequestModel
            {
                ProviderAccountId = providerAccountId,
                ElectricityProviderProfileId = request.ElectricityProviderProfileID,
                SolarProviderProfileIDs = request.SolarProviderProfileIDs,
                FromDate = fromDate,
                GenerateSlope = false,
                RateInflation = request.EscalationRate.ToString(CultureInfo.InvariantCulture),
                SolarRateAmount = solarCharge,
                AnnualOutputDegradation = request.AnnualOutputDegradation
            };

            SavingsAnalysisResponse savingsAnalysisResponse = this.CalculateSavingAnalysis(genabilityAppID, genabilityAppKey, savingAnalysisRequestModel);


            return this.ExtractPowerPurchaseAgreementResponseFromSavingAnalysis(savingsAnalysisResponse, request.Consumption.Select(c => c.ConsumptionInKwh).ToArray());

        }

        /// <summary>
        /// Method for extracting the PowerPurchaseAgreementResponse object from a SavingsAnalysisResponse object.
        /// </summary>
        /// <param name="savingsAnalysisResponse">The SavingsAnalysisResponse object.</param>
        /// <param name="monthlyConsumptions">The monthly consumption array.</param>
        /// <returns>Returns the PowerPurchaseAgreementResponse object.</returns>
        public PowerPurchaseAgreementResponse ExtractPowerPurchaseAgreementResponseFromSavingAnalysis(SavingsAnalysisResponse savingsAnalysisResponse, int[] monthlyConsumptions)
        {
            PowerPurchaseAgreementResponse response = new PowerPurchaseAgreementResponse();

            if (savingsAnalysisResponse == null || savingsAnalysisResponse.results == null) throw new Exception("Invalid savings analysis response");

            AccountAnalysis analysis = savingsAnalysisResponse.results.FirstOrDefault();

            if (analysis != null && analysis.seriesData.Count != 0)
            {
                List<AmortizationTableRowPPA> amortizationTable = new List<AmortizationTableRowPPA>();

                #region DEFINE ANALYSIS SERIES


                Series monthlySolarSeries = analysis.series.FirstOrDefault(s => s.seriesPeriod.Equals(SalesPeriod.MONTH.ToString()) &&
                                                                              s.scenario.Equals(SavingAnalysisScenarios.solar.ToString()));

                Series yearlyBeforeSeries = analysis.series.FirstOrDefault(s => s.seriesPeriod.Equals(SalesPeriod.YEAR.ToString()) &&
                                                              s.scenario.Equals(SavingAnalysisScenarios.before.ToString()));

                Series yearlySolarSeries = analysis.series.FirstOrDefault(s => s.seriesPeriod.Equals(SalesPeriod.YEAR.ToString()) &&
                                                             s.scenario.Equals(SavingAnalysisScenarios.solar.ToString()));

                #endregion

                #region DEFINE SERIES IDs

                int monthlySolarSeriesID = monthlySolarSeries != null ? monthlySolarSeries.seriesId : 0;
                int yearlyBeforeSeriesID = yearlyBeforeSeries != null ? yearlyBeforeSeries.seriesId : 0;
                int yearlySolarSeriesID = yearlySolarSeries != null ? yearlySolarSeries.seriesId : 0;

                #endregion


                #region DEFINE SERIES ITEM LISTS

                List<SeriesMeasure> monthlySolarItems = analysis.seriesData.Where(d => d.seriesId.Equals(monthlySolarSeriesID.ToString())).ToList();
                List<SeriesMeasure> yearlyBeforeItems = analysis.seriesData.Where(d => d.seriesId.Equals(yearlyBeforeSeriesID.ToString())).ToList();
                List<SeriesMeasure> yearlySolarItems = analysis.seriesData.Where(d => d.seriesId.Equals(yearlySolarSeriesID.ToString())).ToList();

                #endregion

                decimal ppaPayment, postSolarCost, monthlyProduction, preSolarCost, currentYearSolarCharge, firstYearPostSolarCost, firstYearBeforeSolarCost;
                SeriesMeasure yearlySolarItem, yearlyBeforeItem;
                decimal[] monthlyProductionCoeficients = new decimal[monthsInYear];
                SeriesMeasure firstYearSolar = yearlySolarItems.First();
                decimal firstYearProduction = firstYearSolar.qty;
                DateTime fromDate = monthlySolarItems.First().fromDateTime.Value;


                // find production coeficients for each month; used to estimate the monthly production values for future years
                monthlySolarItems.ForEach(msi =>
                {
                    monthlyProductionCoeficients[msi.fromDateTime.Value.Month - 1] = (decimal)msi.qty / firstYearProduction;
                });



                for (int yearIndex = 0; yearIndex < yearlySolarItems.Count; yearIndex++)
                {
                    yearlySolarItem = yearlySolarItems[yearIndex];
                    yearlyBeforeItem = yearlyBeforeItems[yearIndex];
                    currentYearSolarCharge = yearlySolarItem.cost.Value / yearlySolarItem.qty;

                    for (int monthIndex = 0; monthIndex < monthsInYear; monthIndex++)
                    {
                        monthlyProduction = yearlySolarItem.qty * monthlyProductionCoeficients[fromDate.Month - 1];
                        preSolarCost = yearlyBeforeItem.rate.Value * monthlyConsumptions[fromDate.Month - 1];

                        if (monthlyProduction > monthlyConsumptions[fromDate.Month - 1])
                        {
                            ppaPayment = postSolarCost = monthlyConsumptions[fromDate.Month - 1] * currentYearSolarCharge;
                        }
                        else
                        {
                            ppaPayment = monthlyProduction * currentYearSolarCharge;
                            postSolarCost = ppaPayment + (monthlyConsumptions[fromDate.Month - 1] - monthlyProduction) * yearlyBeforeItem.rate.Value;
                        }

                        amortizationTable.Add(new AmortizationTableRowPPA
                        {
                            Month = fromDate.Month,
                            Year = yearIndex + 1,
                            Production = monthlyProduction / 1000, // from Wh to kWh
                            PreSolarCost = preSolarCost,
                            PostSolarCost = postSolarCost,
                            PPAPayment = ppaPayment
                        });

                        fromDate = fromDate.AddMonths(1);
                    }

                }

                firstYearPostSolarCost = amortizationTable.Take(monthsInYear).Sum(row => row.PostSolarCost);
                firstYearBeforeSolarCost = amortizationTable.Take(monthsInYear).Sum(row => row.PreSolarCost);

                response.AmortizationTable = amortizationTable.ToArray();

                response.CustomerValueProposition = new CustomerValuePropositionPPA
                {
                    PricePerKWH = firstYearSolar.cost.Value / firstYearSolar.qty,
                    FirstYearSavings = firstYearBeforeSolarCost - firstYearPostSolarCost,
                    FirstYearCost = firstYearPostSolarCost
                };
            }


            return response;
        }

        /// <summary>
        /// Extract a list of MonthDetails objects from a SavingsAnalysisResponse object.
        /// </summary>
        /// <param name="savingsAnalysisResponse">The SavingsAnalysisResponse object.</param>
        /// <returns>List of MonthDetails objects.</returns>
        public List<EnergyMonthDetails> ExtractMonthDetailsFromSavingAnalysis(SavingsAnalysisResponse savingsAnalysisResponse, List<EnergyMonthDetails> requestMonths, bool generateSlope)
        {
            var monthDetails = new List<EnergyMonthDetails>();
            AccountAnalysis analysis = savingsAnalysisResponse.results.FirstOrDefault();

            if (analysis != null && analysis.seriesData != null && analysis.seriesData.Count != 0)
            {
                Series series = analysis.series?.FirstOrDefault(s => s.seriesPeriod.Equals(SalesPeriod.MONTH.ToString()) &&
                                                                           s.scenario.Equals(SavingAnalysisScenarios.before.ToString()));
                int seriesId = series != null ? series.seriesId : 1;


                List<SeriesMeasure> items = analysis.seriesData.Where(d => d.seriesId.Equals(seriesId.ToString()) &&
                                                                           d.fromDateTime.HasValue &&
                                                                           d.fromDateTime.Value.Year <= DateTime.Now.Year)
                                                               .ToList();

                items.ForEach(i =>
                {
                    if (!monthDetails.Any(md => md.Month.Equals(i.fromDateTime.Value.Month)))
                    {
                        decimal pricePerKWH = i.rate.HasValue ? i.rate.Value : 0;

                        var monthDetail = new EnergyMonthDetails
                        {
                            Month = i.fromDateTime.Value.Month,
                            Year = i.fromDateTime.Value.Year,
                            PreSolarCost = i.cost.HasValue ? i.cost.Value : 0,
                            PricePerKWH = i.rate.HasValue ? i.rate.Value : 0,
                            Consumption = (int)Math.Round(i.qty)
                        };

                        var requestMonth = requestMonths != null ? requestMonths.FirstOrDefault(rc => rc.Month.Equals(i.fromDateTime.Value.Month) && rc.Year.Equals(i.fromDateTime.Value.Year)) : null;
                        if (DateTime.IsLeapYear(monthDetail.Year) && monthDetail.Month == 2 /*February*/)
                        { // hack for Genability erroneous returned consumption values in leap year
                            monthDetail.Consumption = Math.Round(requestMonth != null ? requestMonth.Consumption : (monthDetail.Consumption * 29 / 28));
                        }
                        else
                        {// Genability gives us some wrong values for Price, so we'll just calculate it by multiplying consumption with price per KW if we can
                            if (requestMonth != null && requestMonth.Consumption > 0)
                            {
                                monthDetail.Consumption = requestMonth.Consumption;
                            }
                            else
                            {
                                if (generateSlope == false) monthDetail.Consumption = 0;
                            }
                        }

                        if (monthDetail.PricePerKWH > 0 && monthDetail.Consumption > 0) monthDetail.PreSolarCost = monthDetail.Consumption * monthDetail.PricePerKWH;

                        monthDetails.Add(monthDetail);
                    }
                });
            }

            return monthDetails;
        }

        /// <summary>
        /// Calculates a Savings Analasys based on an account's usage profiles.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">A SavingAnalysisRequestModel object encapsulating SavingAnalysis request information.</param>
        /// <returns>The response from running SavingAnalysis.</returns>
        public SavingsAnalysisResponse CalculateSavingAnalysis(string genabilityAppID, string genabilityAppKey, SavingAnalysisRequestModel request)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
            };


            List<SavingAnalysisPropertyData> propertyInputs = new List<SavingAnalysisPropertyData>();
            List<TariffRate> rateInputs = new List<TariffRate>();

            if (!String.IsNullOrWhiteSpace(request.ElectricityProviderProfileId))
            {
                //add electricity profile id
                propertyInputs.Add(new SavingAnalysisPropertyData
                {
                    scenarios = String.Format("{0},{1}", SavingAnalysisScenarios.before.ToString(), SavingAnalysisScenarios.after.ToString()),
                    keyName = SavingAnalysisPropertyInputs.providerProfileId.ToString(),
                    dataValue = request.ElectricityProviderProfileId
                });
            }

            //if the SavingsAnalasys will be made for a solar usage profile as well(i.e: it has a solar scenario)
            if (request.SolarProviderProfileIDs != null && request.SolarProviderProfileIDs.Count != 0)
            {
                request.SolarProviderProfileIDs.ForEach(id =>
                {
                    propertyInputs.Add(new SavingAnalysisPropertyData
                    {
                        scenarios = String.Format("{0},{1}", SavingAnalysisScenarios.after.ToString(), SavingAnalysisScenarios.solar.ToString()),
                        keyName = SavingAnalysisPropertyInputs.providerProfileId.ToString(),
                        dataValue = id
                    });
                });

                //add information abou the rate at which energy production from the solar system degrades.
                propertyInputs.Add(new SavingAnalysisPropertyData
                {
                    scenarios = String.Format("{0},{1}", SavingAnalysisScenarios.solar.ToString(), SavingAnalysisScenarios.after.ToString()),
                    keyName = SavingAnalysisPropertyInputs.solarDegradation.ToString(),
                    dataValue = (request.AnnualOutputDegradation * 100).ToString(CultureInfo.InvariantCulture)
                });

                //add information about the rate at which the cost of energy raises for every year of the analysis
                propertyInputs.Add(new SavingAnalysisPropertyData
                {
                    scenarios = String.Format("{0},{1},{2}", SavingAnalysisScenarios.before.ToString(), SavingAnalysisScenarios.after.ToString(), SavingAnalysisScenarios.solar.ToString()),
                    keyName = SavingAnalysisPropertyInputs.rateInflation.ToString(),
                    dataValue = !String.IsNullOrWhiteSpace(request.RateInflation) ? request.RateInflation : GenabilityDefaultParameterValues.rateInflation
                });

                //add information about the duration of the project
                propertyInputs.Add(new SavingAnalysisPropertyData
                {
                    scenarios = String.Format("{0},{1},{2}", SavingAnalysisScenarios.before.ToString(), SavingAnalysisScenarios.after.ToString(), SavingAnalysisScenarios.solar.ToString()),
                    keyName = SavingAnalysisPropertyInputs.projectDuration.ToString(),
                    dataValue = GenabilityDefaultParameterValues.projectDuration.ToString()
                });

                rateInputs.Add(new TariffRate
                {
                    chargeType = ChargeType.CONSUMPTION_BASED.ToString(),
                    rateBands = new List<TariffRateBand>
                        {
                            new TariffRateBand
                            {
                                rateAmount =  request.SolarRateAmount ?? GenabilityDefaultParameterValues.solarRateAmount
                            }
                        }
                });
            }

            DateTime fromDateTime = request.FromDate.HasValue ? request.FromDate.Value : new DateTime(DateTime.Now.Year - 1, DateTime.Now.Month, 1);
            int februaryYear = fromDateTime.Month <= 2 ? fromDateTime.Year : fromDateTime.Year + 1;

            // hack made in order to fix bug on Genability side: the February month production is miscalculated for leap year
            if (request.SolarProviderProfileIDs != null && DateTime.IsLeapYear(februaryYear))
            {
                fromDateTime = fromDateTime.AddYears(1);
            }


            SavingsAnalysisRequest savingsAnalysisRequest = new SavingsAnalysisRequest
            {
                providerAccountId = request.ProviderAccountId,
                fromDateTime = fromDateTime.ToString("o"),
                populateCosts = false,
                propertyInputs = propertyInputs,
                rateInputs = rateInputs,
                useIntelligentBaselining = request.GenerateSlope
            };

            SavingsAnalysisResponse response = APIClient.MakeRequest<SavingsAnalysisResponse>(serviceUrl,
                                                                                    Endpoints.calculateSavingAnalysisResource,
                                                                                    Method.POST,
                                                                                    payload: savingsAnalysisRequest,
                                                                                    parameters: parameters,
                                                                                    serializer: new RestSharpJsonSerializer());

            //LogRequest(Constants.CalculateSavingAnalysis, Endpoints.calculateSavingAnalysisResource, savingsAnalysisRequest, response);
            return response;
        }

        /// <summary>
        /// Retrieves the estimated consumption value for a given month.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The account provider ID.</param>
        /// <param name="cost">The cost for that month.</param>
        /// <param name="fromDate">The month's start date.</param>
        /// <returns>The consumption value.</returns>
        public int GetMonthPresolarConsumption(string genabilityAppID, string genabilityAppKey, string providerAccountId, decimal cost, DateTime fromDate, string masterTarrifID)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
            };

            List<PropertyData> tariffInputs = new List<PropertyData>
            {
                new PropertyData
                {
                    fromDateTime    = fromDate.ToString("o"),
                    toDateTime      = fromDate.AddMonths(1).ToString("o"),
                    keyName         = CalculateCostPropertyInput.total.ToString(),
                    dataValue       = cost.ToString(CultureInfo.InvariantCulture),
                    unit            = CalculateCostPropertyInput.cost.ToString()
                },
                new PropertyData
                {
                    fromDateTime    = fromDate.ToString("o"),
                    toDateTime      = fromDate.AddMonths(1).ToString("o"),
                    keyName         = CalculateCostPropertyInput.baselineType.ToString(),
                    dataValue       = CalculateCostPropertyInput.typicalElectricity.ToString()
                }
            };

            CostCalculatorRequest request = new CostCalculatorRequest
            {
                providerAccountId = providerAccountId,
                fromDateTime = fromDate.ToString("o"),
                toDateTime = fromDate.AddMonths(1).ToString("o"),
                billingPeriod = Boolean.TrueString,
                tariffInputs = tariffInputs
            };
            var url = string.IsNullOrWhiteSpace(masterTarrifID) ? Endpoints.calculateCostGetConsumptionResource : $"{Endpoints.calculateCostGetConsumptionResource}/{masterTarrifID}";

            CostCalculatorResponse response = APIClient.MakeRequest<CostCalculatorResponse>(serviceUrl,
                                                            url,
                                                            Method.POST,
                                                            payload: request,
                                                            parameters: parameters,
                                                            serializer: new RestSharpJsonSerializer());

            //LogRequest(Constants.GetMonthPresolarConsumption, Endpoints.calculateCostGetConsumptionResource, request, response);

            if (response.results.Count == 0 || response.results[0].summary == null || !response.results[0].summary.kWh.HasValue)
            {
                throw new Exception("Incorrect request values!");
            }

            return (int)Math.Round(response.results[0].summary.kWh.Value);
        }

        public CostCalculatorResponse GetPriceForAverageConsumption(UsageProfileRequest request)
        {
            var parameters = GetAuthParams();

            // submit the averate consumption
            var response = APIClient.MakeRequest<UpsertUsageProfileResponse>(serviceUrl,
                                                                                            Endpoints.upsertUsageProfileResource,
                                                                                            Method.POST,
                                                                                            payload: request,
                                                                                            parameters: parameters,
                                                                                            serializer: new RestSharpJsonSerializer());

            //LogRequest(Constants.UpsertSolarProfileWithIntegratedPVWatts, Endpoints.upsertUsageProfileResource, request, response);
            var profileId = response.results?.FirstOrDefault()?.profileId;
            var readingData = request.ReadingData.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(profileId) && readingData != null)
            {

                var calculatorResponse = CalculateCostForAccount(request.ProviderAccountId, profileId, readingData.fromDateTime, readingData.toDateTime);
                return calculatorResponse;
            }
            return null;
        }

        /// <summary>
        /// Method for calculating the presolar cost.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The account provider ID.</param>
        /// <param name="masterTariffId">The ID of the Tariff.</param>
        /// <param name="calculateCostMonths">The months information for which the cost calculation is being made.</param>
        /// <returns>Returns an CostCalculatorResponse object.</returns>
        public CostCalculatorResponse CalculateCost(string genabilityAppID, string genabilityAppKey, string providerAccountId, string masterTariffId, List<EnergyMonthDetails> calculateCostMonths)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
            };

            List<PropertyData> consumptionInputs = new List<PropertyData>();
            DateTime fromDate;
            bool isCurrentYear;

            foreach (var calculateCostMonth in calculateCostMonths)
            {
                isCurrentYear = DateTime.Now.Month >= calculateCostMonth.Month;
                fromDate = calculateCostMonth.Year != 0 ?
                                            new DateTime(calculateCostMonth.Year, calculateCostMonth.Month, 1) :
                                            new DateTime(isCurrentYear ? DateTime.Now.Year : (DateTime.Now.Year - 1), calculateCostMonth.Month, 1);


                consumptionInputs.Add(new PropertyData
                {
                    keyName = consumptionValue,
                    fromDateTime = fromDate.ToString("o"),
                    toDateTime = fromDate.AddMonths(1).ToString("o"),
                    unit = kWhValue,
                    dataValue = calculateCostMonth.PostSolarConsumptionOrConsumption.ToString(CultureInfo.InvariantCulture)
                });
            }
            var oldestMonth = calculateCostMonths.OrderBy(md => md.Year).ThenBy(md => md.Month).First();
            fromDate = new DateTime(oldestMonth.Year, oldestMonth.Month, 1);

            CostCalculatorRequest request = new CostCalculatorRequest
            {
                fromDateTime = fromDate.ToString("o"),
                toDateTime = fromDate.AddYears(1).ToString("o"),
                groupBy = GroupBy.MONTH.ToString(),
                detailLevel = DetailLevel.TOTAL.ToString(),
                tariffInputs = consumptionInputs
            };

            CostCalculatorResponse response = APIClient.MakeRequest<CostCalculatorResponse>(serviceUrl,
                                                                        String.Format(Endpoints.calculateCostResource, masterTariffId),
                                                                        Method.POST,
                                                                        payload: request,
                                                                        parameters: parameters,
                                                                        serializer: new RestSharpJsonSerializer());
            //LogRequest(Constants.CalculateCost, Endpoints.calculateCostResource, request, response);
            return response;
        }

        /// <summary>
        /// Method for getting all the LSEs from a given ZIP code.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="zipCode">The ZIP code from which the LSEs are taken.</param>
        /// <returns>Returns an GetLSEsResponse object.</returns>
        public GetLSEsResponse GetLSEs(string genabilityAppID, string genabilityAppKey, string zipCode)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = zipCodeParameter,
                                    Value = zipCode
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = residentialServiceTypesParameter,
                                    Value = ServiceType.ELECTRICITY
                                }
             };
            var p = $"{zipCodeParameter}={zipCode}&{residentialServiceTypesParameter}={ServiceType.ELECTRICITY}";
            GetLSEsResponse response = APIClient.MakeRequest<GetLSEsResponse>(serviceUrl, Endpoints.getLSEsResource, Method.GET, parameters: parameters);
            //LogRequest(Constants.GetLSEs, Endpoints.getLSEsResource, p, response);

            return response;
        }

        /// <summary>
        /// Method for retrieving information about an account.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="providerAccountId">The account provider ID.</param>
        /// <returns>Returns an CreateAccountResponse object.</returns>
        public CreateAccountResponse GetGenabilityAccount(string genabilityAppID, string genabilityAppKey, string providerAccountId)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = genabilityAppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = genabilityAppKey
                                }
             };

            CreateAccountResponse response = APIClient.MakeRequest<CreateAccountResponse>(serviceUrl,
                                                                       String.Format(Endpoints.getAccountResource, providerAccountId),
                                                                       Method.GET, parameters: parameters);
            //LogRequest(Constants.GetLSEs, Endpoints.getAccountResource, $"providerAccountId={providerAccountId}", response);
            return response;

        }


        /// <summary>
        /// Method for retrieving tariffs afferent to a particular zip code.
        /// </summary>
        /// <param name="genabilityAppID">The genability application ID.</param>
        /// <param name="genabilityAppKey">The genability application key.</param>
        /// <param name="zipCode">The zip code.</param>
        /// <returns>Returns an GetTariffsResponse object.</returns>
        public GetTariffsResponse GetZipCodeAndLSEIdTariffs(string zipCode, string lseID)
        {
            List<Parameter> parameters = new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = Credentials.AppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = Credentials.AppKey
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = GetTariffsRequestParameters.zipCode.ToString(),
                                    Value = zipCode
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = GetTariffsRequestParameters.lseId.ToString(),
                                    Value = lseID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = GetTariffsRequestParameters.effectiveOn.ToString(),
                                    Value = DateTime.UtcNow.ToString("yyyy-MM-dd")
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = GetTariffsRequestParameters.customerClasses.ToString(),
                                    Value = CustomerClass.RESIDENTIAL.ToString()
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = GetTariffsRequestParameters.tariffTypes.ToString(),
                                    Value = String.Format("{0},{1}", TariffTypes.DEFAULT, TariffTypes.ALTERNATIVE)
                                }
             };

            var para = string.Join("&", parameters.Where(p => p.Name != appIdParameter && p.Name != appKeyParameter).Select(p => $"{p.Name}={p.Value}"));
            GetTariffsResponse response = APIClient.MakeRequest<GetTariffsResponse>(serviceUrl,
                                                                       Endpoints.getTariffsResource,
                                                                       Method.GET, parameters: parameters);
            //LogRequest(Constants.GetZipCodeTariffs, Endpoints.getTariffsResource, para, response);

            return response;

        }

        private List<Parameter> GetAuthParams()
        {
            return new List<Parameter> { new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appIdParameter,
                                    Value = Credentials.AppID
                                },
                                new Parameter {
                                    Type  = ParameterType.QueryString,
                                    Name  = appKeyParameter,
                                    Value = Credentials.AppKey
                                }
            };
        }

        private void LogRequest(string name, string url, object request, object response)
        {
            if (OnRequest == null)
            {
                return;
            }

            OnRequest(name, url, JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response));
        }
    }
}
