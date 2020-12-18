using DataReef.Core.Configuration;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Integrations.Core.Models;
using DataReef.Integrations.Genability;
using DataReef.TM.Api.Classes.Genability;
using DataReef.TM.Api.Classes.Requests;
using DataReef.TM.Api.Classes.Solar.Proposal;
using DataReef.TM.Api.Mappers;
using DataReef.TM.Contracts.FaultContracts;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.Solar;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DTOs.Solar.Genability;
using DataReef.TM.Models.DTOs.Solar.Proposal;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Solar;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace DataReef.TM.Api.Controllers
{
    public enum ThirdPartyProviderType
    {
        Genability,
        Mosaic,
        NetSuite
    }

    [RoutePrefix("api/v1/integrations")]
    public partial class IntegrationsController : ApiController
    {
        private readonly IDataService<User> _userService;
        private readonly IDataService<ManualProposal> _manualProposalService;
        private readonly IDataService<ProposalRoofPlaneInfo> _proroofService;
        private readonly IPersonService _personService;
        private readonly IOUService _ouService;
        private readonly IBlobService _blobService;
        private readonly ISpruceQuoteRequestService _spruceQuoteRequestService;
        private readonly ISpruceQuoteResponseService _spruceQuoteResponseService;
        private readonly ISpruceGenDocsRequestService _spruceGenDocsRequestService;
        private readonly ISignatureService _signatureService;
        private readonly IProposalService _proposalService;
        private readonly IFinancePlanDefinitionService _financePlanDefinitionService;
        private readonly Lazy<IOUSettingService> _ouSettingsService;
        private readonly Lazy<IDataService<SolarTariff>> _solarTarrifService;
        private readonly Lazy<IProposalIntegrationAuditService> _proposalIntegrationAudit;
        private readonly Lazy<IPropertyService> _propertyService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _smartBoardAdapter;

        private readonly string _spruceUrl = ConfigurationManager.AppSettings["SpruceUrl"];

        public IntegrationsController(IDataService<User> userService,
                                      IDataService<ManualProposal> manualProposalService,
                                      IDataService<ProposalRoofPlaneInfo> proroofService,
                                      IPersonService personService,
                                      IOUService ouService,
                                      IBlobService blobService,
                                      ISpruceQuoteRequestService spruceQuoteRequestService,
                                      ISpruceQuoteResponseService spruceQuoteResponseService,
                                      ISpruceGenDocsRequestService spruceGenDocsRequestService,
                                      ISignatureService signatureService,
                                      IFinancePlanDefinitionService financePlanDefinitionService,
                                      IProposalService proposalService,
                                      Lazy<IOUSettingService> ouSettingsService,
                                      Lazy<IDataService<SolarTariff>> solarTarrifService,
                                      Lazy<IProposalIntegrationAuditService> proposalIntegrationAudit,
                                      Lazy<IPropertyService> propertyService,
                                      Lazy<ISolarSalesTrackerAdapter> smartBoardAdapter)
        {
            HttpContext.Current.Server.ScriptTimeout = 300;
            _userService = userService;
            _manualProposalService = manualProposalService;
            _proroofService = proroofService;
            _personService = personService;
            _ouService = ouService;
            _blobService = blobService;
            _spruceQuoteRequestService = spruceQuoteRequestService;
            _spruceQuoteResponseService = spruceQuoteResponseService;
            _spruceGenDocsRequestService = spruceGenDocsRequestService;
            _signatureService = signatureService;
            _proposalService = proposalService;
            _financePlanDefinitionService = financePlanDefinitionService;
            _ouSettingsService = ouSettingsService;
            _solarTarrifService = solarTarrifService;
            _proposalIntegrationAudit = proposalIntegrationAudit;
            _propertyService = propertyService;
            _smartBoardAdapter = smartBoardAdapter;
        }
        #region Common API

        private List<Tuple<string, string, string, string>> AuditData;

        private void ResetRequestData()
        {
            AuditData = new List<Tuple<string, string, string, string>>();
        }

        private void AddAuditData(string name, string url, string request, string response)
        {
            AuditData.Add(new Tuple<string, string, string, string>(name, url, request, response));
        }

        private void SaveAuditData(Guid? proposalID, Guid? ouid, string accountProfileId)
        {
            if ((AuditData?.Count ?? 0) == 0)
            {
                return;
            }

            var data = AuditData
                .Select(a => new ProposalIntegrationAudit
                {
                    Name = a.Item1,
                    Url = a.Item2,
                    RequestJSON = a.Item3,
                    ResponseJSON = a.Item4,
                    ProposalID = proposalID,
                    OUID = ouid,
                    CreatedByID = SmartPrincipal.UserId,
                    ExternalID = accountProfileId
                })
                .ToList();

            _proposalIntegrationAudit.Value.InsertMany(data);
        }

        [Route("{ouid:guid}/Genability/account/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(GenabilityAccount))]
        public async Task<IHttpActionResult> CreateAccount(Guid ouid, CreateAccountRequest request, Guid? proposalID = null)
        {
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            var provider = new Integrations.Genability.IntegrationProvider(credentials.Url, AddAuditData);
            var response = provider.CreateAccount(credentials.AppID, credentials.AppKey, request);

            SaveAuditData(proposalID, ouid, null);

            return Ok(response.count > 0 ? response.results.First() : null);
        }

        [Route("{ouid:guid}/Genability/account/{accountId}/ppa/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(GenericResponse<PowerPurchaseAgreementResponse>))]
        public async Task<IHttpActionResult> PowerPurchaseAgreement(Guid ouid, string accountId, GenericRequest<PowerPurchaseAgreementRequest> req, Guid? proposalID = null)
        {
            ResetRequestData();
            var request = req.Request;

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            Integrations.Genability.IntegrationProvider provider = new Integrations.Genability.IntegrationProvider(credentials.Url, AddAuditData, credentials);
            PowerPurchaseAgreementResponse response = new PowerPurchaseAgreementResponse();

            bool hasElectricityProfile = !String.IsNullOrWhiteSpace(request.ElectricityProviderProfileID) &&
                                          provider.CheckUsageProfileExistence(accountId, request.ElectricityProviderProfileID);

            if (!hasElectricityProfile)
            {
                var monthDetails = new List<EnergyMonthDetails>();
                for (int i = 0; i < request.Consumption.Length; i++)
                {
                    var consumption = request.Consumption[i];
                    monthDetails.Add(new EnergyMonthDetails
                    {
                        Month = i + 1,
                        Consumption = consumption.ProductionInKwh,
                        Production = consumption.ProductionInKwh,
                        PreSolarCost = consumption.Price,
                    });
                }

                UpsertUsageProfileResponse upsertUsageProfileResponse = provider.UpsertElectricityProfile(credentials.AppID, credentials.AppKey,
                                                                    accountId, request.ElectricityProviderProfileID, request.SolarProfileName, monthDetails);
            }

            double yield = request.AnnualProduction / request.SystemSize * 1000.0;
            ChargeLookupItem chargeItem = ChargeLookup.LookupCharge(request.UtilityID, request.Market, request.EscalationRate, yield);

            if (chargeItem == null)
            {
                SaveAuditData(proposalID, ouid, accountId);
                throw new ApplicationException("Unable to find matching PPA Pricing for Utility " + request.UtilityID);
            }
            decimal solarRate = chargeItem.Charge;
            if (request.IsPricingB.HasValue && request.IsPricingB.Value)
            {
                solarRate += 0.02m;
            }

            response = provider.GeneratePowerPurchaseAgreement(credentials.AppID, credentials.AppKey, accountId, request, solarRate);

            response.MinimumYield = chargeItem.RequiredYield;
            response.HasMinimumYield = yield >= chargeItem.RequiredYield;

            SaveAuditData(proposalID, ouid, accountId);

            return Ok<GenericResponse<PowerPurchaseAgreementResponse>>(new GenericResponse<PowerPurchaseAgreementResponse> { Response = response });

        }

        [Route("{ouid:guid}/Genability/account/{accountId}/presolarconsumption/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(EnergyMonthDetails))]
        public async Task<IHttpActionResult> PresolarConsumption(Guid ouid, string accountId, EnergyMonthDetails request, Guid? proposalID = null)
        {
            string masterTarrifId = null;
            if (proposalID.HasValue)
            {
                var tarrif = _solarTarrifService.Value.Get(proposalID.Value);

                // Dirty hack that fixes some of the invalid data we get from Genability in some situations.
                // Genability has an issue w/ Rocky Mountain Power
                // While they are finishing implementing the new Tariffs.
                // For a new proposal, before the auto-save kicks in, we don't have any Tarrif.
                // To avoid having weird values, we default to 633 when there's no value.
                if (tarrif == null || tarrif?.UtilityID == "737")
                {
                    masterTarrifId = "633";
                }
            }

            ResetRequestData();
            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            Integrations.Genability.IntegrationProvider provider = new Integrations.Genability.IntegrationProvider(credentials.Url, AddAuditData, credentials);

            DateTime fromDate = new DateTime(request.Year, request.Month, 1);

            int consumption = provider.GetMonthPresolarConsumption(credentials.AppID, credentials.AppKey, accountId, request.PreSolarCost, fromDate, masterTarrifId);

            request.Consumption = consumption;
            SaveAuditData(proposalID, ouid, accountId);

            return Ok(request);
        }

        [Route("{ouid:guid}/Genability/{accountId}/averages/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(PresolarCostResponseModel))]
        public async Task<IHttpActionResult> Averages(Guid ouid, string accountId, [FromBody]EnergyAveragesRequest req, Guid? proposalID = null)
        {
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            var provider = new IntegrationProvider(credentials.Url, AddAuditData, credentials);

            CalculatedCost costData = null;
            // Get the response based on Monthly or Annual average consumption
            if (req.Consumption.HasValue)
            {
                var request = new UsageProfileRequest(accountId, req);

                var data = provider.GetPriceForAverageConsumption(request);

                costData = data?.results?.FirstOrDefault();
            }
            else if (req.Price.HasValue)
            {
                string masterTarrifId = null;
                if (proposalID.HasValue)
                {
                    var tarrif = _solarTarrifService.Value.Get(proposalID.Value);

                    // Dirty hack that fixes some of the invalid data we get from Genability in some situations.
                    // Genability has an issue w/ Rocky Mountain Power
                    // While they are finishing implementing the new Tariffs.
                    // For a new proposal, before the auto-save kicks in, we don't have any Tarrif.
                    // To avoid having weird values, we default to 633 when there's no value.
                    if (tarrif == null || tarrif?.UtilityID == "737")
                    {
                        masterTarrifId = "633";
                    }
                }
                var data = provider.CalculateConsumptionForAccount(accountId, $"{req.Price}", req.Start.ToString("o"), req.End.ToString("o"), masterTarrifId);
                costData = data?.results?.FirstOrDefault();
            }

            if (costData != null)
            {
                var consumptionCostData = costData?
                                            .items?
                                            .Where(i => i.quantityKey == "consumption" && i.cost != 0)?
                                            .ToList();

                var response = new PresolarCostResponseModel
                {
                    AnnualTotalConsumption = (long)(costData.summary?.kWh ?? 0),
                    AnnualTotalPrice = costData.totalCost ?? 0,
                    //AvgUtilityCost = consumptionCostData?.Average(d => d.rateAmount) ?? 0,
                    CalculateCostMonths = consumptionCostData?
                                            .Select(i => new EnergyMonthDetails
                                            {
                                                Year = i.fromDateTime?.Year ?? 0,
                                                Month = i.fromDateTime?.Month ?? 0,
                                                Consumption = i.itemQuantity ?? 0,
                                                PreSolarCost = i.cost ?? 0,
                                                PricePerKWH = i.rateAmount ?? 0,
                                            }).ToList()
                };

                // there's a bug w/ Genability that sends an incorrect TotalConsumption value
                if (response.AnnualTotalConsumption > 1000000)
                {
                    response.AnnualTotalConsumption = (long)response.CalculateCostMonths.Sum(m => m.Consumption);
                }

                //if (response.AvgUtilityCost == 0 && response.AnnualTotalPrice != 0 && response.AnnualTotalConsumption != 0)
                //{
                //    response.AvgUtilityCost = response.AnnualTotalPrice / response.AnnualTotalConsumption;
                //}
                return Ok(response);
            }

            return Ok(new PresolarCostResponseModel());
        }

        [Route("{ouid:guid}/Genability/account/{accountId}/utilitycost/{proposalID?}")]
        [HttpGet]
        [ResponseType(typeof(PriceResult))]
        public async Task<IHttpActionResult> GetAccountRecommendedPrice(Guid ouid, string accountId, Guid? proposalID = null)
        {
            ResetRequestData();
            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            // Temporary FIX until Genability fixes their API
            // get the zip code of the property, and alternatively use it if the AccountID request does not work.
            // TODO: remove this once Genability's API start working w/ AccountID.
            string zipCode = null;
            bool? usagecollected = null;

            if (proposalID.HasValue)
            {
                var proposal = _proposalService.Get(proposalID.Value, "Property");
                zipCode = proposal?.Property?.ZipCode;
                usagecollected = proposal?.Property?.UsageCollected;
            }

            var provider = new IntegrationProvider(credentials.Url, AddAuditData);
            var response = provider.GetAccountPriceResult(credentials.AppID, credentials.AppKey, accountId, zipCode, usagecollected);

            // if both AccountID and ZipCode requests failed, try to get an older Tariff that has data.
            if (response == null)
            {
                // Temporary FIX until Genability fixes their API
                var tariff = _propertyService.Value.GetTariffByGenabilityProviderAccountID(accountId);
                if (tariff != null)
                {
                    response = new PriceResult
                    {
                        MasterTariffID = tariff.MasterTariffID,
                        PricePerKWH = (decimal)tariff.PricePerKWH,
                        TariffCode = tariff.TariffCode,
                        TariffID = tariff.TariffID,
                        TariffName = tariff.TariffName,
                        UtilityID = tariff.UtilityID,
                        UtilityName = tariff.UtilityName,
                        UsageCollected = usagecollected
                    };
                }
            }
            SaveAuditData(proposalID, ouid, accountId);

            return Ok(response ?? new PriceResult());
        }


        [Route("{ouid:guid}/Genability/account/{accountId}/profile/{profileId}/presolarcost/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(PresolarCostResponseModel))]
        public async Task<IHttpActionResult> PresolarCost(Guid ouid, string accountId, string profileId, UpsertUsageProfileRequestModel request, Guid? proposalID = null)
        {
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            Integrations.Genability.IntegrationProvider provider = new Integrations.Genability.IntegrationProvider(credentials.Url, AddAuditData);

            DateTime? fromDate = null;
            var oldestMonth = request.Consumptions.OrderBy(c => c.Year).ThenBy(c => c.Month).FirstOrDefault();
            if (oldestMonth != null)
            {
                fromDate = new DateTime(oldestMonth.Year, oldestMonth.Month, 1);
            }

            bool enableGenabilityIncompleteConsumptionsCalculationFix = false;
            bool generateSlope = request.GenerateSlope.HasValue && request.GenerateSlope.Value;
            if (generateSlope)
            {
                if (request.Consumptions.Any(c => c.Consumption == 0))
                {
                    enableGenabilityIncompleteConsumptionsCalculationFix = true;
                    request.Consumptions.RemoveAll(c => c.Consumption == 0);
                }
            }

            if (request != null)
            {
                //Upsert of monthly Electricity Readings profile
                UpsertUsageProfileResponse upsertUsageProfileResponse = provider.UpsertElectricityProfile(credentials.AppID, credentials.AppKey,
                                                                                    accountId, profileId, request.ProfileName, request.Consumptions);
            }

            SavingAnalysisRequestModel savingAnalysisRequestModel = new SavingAnalysisRequestModel
            {
                ProviderAccountId = accountId,
                ElectricityProviderProfileId = profileId,
                SolarProviderProfileIDs = null,
                FromDate = fromDate,
                GenerateSlope = request.GenerateSlope ?? false,
                RateInflation = String.Empty,
                SolarRateAmount = null
            };

            SavingsAnalysisResponse savingsAnalysisResponse = provider.CalculateSavingAnalysis(credentials.AppID, credentials.AppKey, savingAnalysisRequestModel);

            var monthDetails = provider.ExtractMonthDetailsFromSavingAnalysis(savingsAnalysisResponse, request.Consumptions, generateSlope);


            if (enableGenabilityIncompleteConsumptionsCalculationFix)
            {
                // we save back into Genability their own estimations, to have the complete consumption profile
                UpsertUsageProfileResponse upsertUsageProfileResponse = provider.UpsertElectricityProfile(credentials.AppID, credentials.AppKey,
                                                                                    accountId, profileId, request.ProfileName, monthDetails);
            }

            PresolarCostResponseModel response = new PresolarCostResponseModel();

            if (monthDetails != null && monthDetails.Count != 0)
            {
                response.AnnualTotalConsumption = (int)monthDetails.Sum(md => md.Consumption);
                response.AnnualTotalPrice = monthDetails.Sum(md => md.PreSolarCost);
                response.CalculateCostMonths = monthDetails;
            }

            SaveAuditData(proposalID, ouid, profileId);

            return Ok(response);
        }


        [Route("Manualpresolarcost/{proposalID}")]
        [HttpPost]
        public async Task<IHttpActionResult> Manualpresolarcost(Guid proposalID, ManualProposal req)
        {
            var prop = _manualProposalService.Get(proposalID);

            var proposal = new ManualProposal();
            if (prop == null)
            {
                prop = new ManualProposal();
                prop.Guid = proposalID;
                prop.IsManual = req.IsManual;
                prop.TotalBill = req.TotalBill;
                prop.TotalKWH = req.TotalKWH;
                proposal = _manualProposalService.Insert(prop);
            }
            else
            {
                prop.IsManual = req.IsManual;
                prop.TotalBill = req.TotalBill;
                prop.TotalKWH = req.TotalKWH;
                proposal = _manualProposalService.Update(prop);
            }

            return Ok(proposal);
        }


        [Route("ManualpresolarcostData/{proposalID}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetManualpresolarcostData(Guid proposalID)
        {
            var proposal = _manualProposalService.Get(proposalID);

            if (proposal == null)
            {
                throw new ArgumentNullException("Please send valid ProposalId");
            }

            return Ok(proposal);
        }

        [Route("{ouid:guid}/Genability/account/{accountId}/lseandtariff/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(GenabilityAccount))]
        public async Task<IHttpActionResult> ChangeAccountLseAndTariff(Guid ouid, string accountId, ChangeAccountLseAndTariffRequestModel request, Guid? proposalID = null)
        {
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            Integrations.Genability.IntegrationProvider provider = new Integrations.Genability.IntegrationProvider(credentials.Url, AddAuditData);
            CreateAccountResponse response = provider.ChangeAccountLseAndTariff(credentials.AppID, credentials.AppKey, accountId,
                                                                                                   request.UtilityID, request.MasterTariffID);

            SaveAuditData(proposalID, ouid, accountId);
            return Ok(response.count > 0 ? response.results.First() : null);
        }


        [Route("{ouid:guid}/Genability/lses/{proposalID?}")]
        [HttpGet]
        [ResponseType(typeof(ICollection<LoadServingEntityResponseModel>))]
        public async Task<IHttpActionResult> GetLSEs(Guid ouid, string zipCode, Guid? proposalID = null)
        {
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            Integrations.Genability.IntegrationProvider provider = new Integrations.Genability.IntegrationProvider(credentials.Url, AddAuditData);
            GetLSEsResponse getLSEsResponse = provider.GetLSEs(credentials.AppID, credentials.AppKey, zipCode);
            List<LoadServingEntity> lses = getLSEsResponse.results != null ? getLSEsResponse.results.OrderBy(l => l.lseId).ToList() :
                                                                                    new List<LoadServingEntity>();

            List<LoadServingEntityResponseModel> response = lses.Select(l => new LoadServingEntityResponseModel
            {
                UtilityID = l.lseId.ToString(),
                UtilityName = l.name
            }).ToList();

            SaveAuditData(proposalID, ouid, null);

            return Ok<ICollection<LoadServingEntityResponseModel>>(response);
        }

        [Route("{ouid:guid}/Genability/lses/{lseID}/tariffs/{proposalID?}")]
        [HttpGet]
        [ResponseType(typeof(ICollection<TariffResponseModel>))]
        public async Task<IHttpActionResult> GetZipCodeTariffs(Guid ouid, string lseID, string zipCode, Guid? proposalID = null)
        {
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            var provider = new IntegrationProvider(credentials.Url, AddAuditData, credentials);
            GetTariffsResponse getZipCodeTariffsResponse = provider.GetZipCodeAndLSEIdTariffs(zipCode, lseID);
            List<Tariff> tariffs = getZipCodeTariffsResponse != null ? getZipCodeTariffsResponse.results : new List<Tariff>();

            //List<TariffResponseModel> response = tariffs.Where(t => t.lseId.Equals(lseID, StringComparison.OrdinalIgnoreCase)).Select(t => new TariffResponseModel
            List<TariffResponseModel> response = tariffs.Select(t => new TariffResponseModel
            {
                MasterTariffID = t.masterTariffId,
                TariffCode = t.tariffCode,
                TariffID = t.tariffId,
                TariffName = t.tariffName,
                UtilityID = t.lseId,
                UtilityName = t.lseName
            }).ToList();

            SaveAuditData(proposalID, ouid, lseID);

            return Ok(response);
        }

        [Route("{ouid:guid}/Genability/account/{accountId}/proposal/{proposalID?}")]
        [HttpPost]
        [ResponseType(typeof(SolarProposalResponse))]
        public async Task<IHttpActionResult> CreateProposalPVWatts5(Guid ouid, string accountId, SolarProposalRequest request, Guid? proposalID = null)
        {
            if (request == null
                || request.RoofPlanes == null
                || request.RoofPlanes.Count == 0)
            {
                throw new ArgumentNullException("Request needs to contain roof planes!");
            }
            ResetRequestData();

            var credentials = ResolveCredentials(ouid, ThirdPartyProviderType.Genability);

            IntegrationProvider provider = new IntegrationProvider(credentials.Url, AddAuditData, credentials);
            RoofPlaneInfo property = request.RoofPlanes.First();

            var ouSettings = _ouSettingsService.Value.GetSettings(ouid, null);


            //foreach(var itm in request.RoofPlanes)
            //{
            //    ProposalRoofPlaneInfo proroofplane = new ProposalRoofPlaneInfo();
            //    proroofplane.ArrayName = itm.ProfileName;
            //    proroofplane.ArrayOffset = itm.ArrayOffset;
            //    proroofplane.Azimuth = itm.Azimuth;
            //    proroofplane.InverterEfficiency = itm.InverterEfficiency;
            //    proroofplane.Losses = itm.Losses;
            //    proroofplane.Name  = itm.ProfileName;
            //    proroofplane.PanelsEfficiency = itm.PanelsEfficiency;
            //    proroofplane.ProfileName = itm.ProfileName;
            //    proroofplane.ProposalId = Guid.Parse(proposalID.Value.ToString());
            //    proroofplane.ProviderProfileId = itm.ProviderProfileId;
            //    proroofplane.Shading = itm.Shading;
            //    proroofplane.Size = itm.Size;
            //    proroofplane.TargetOffset = itm.TargetOffset;
            //    proroofplane.Tilt = itm.Tilt;
            //    _proroofService.Insert(proroofplane);
            //}
            



            // call all these methods in parallel
            var tasks = request.RoofPlanes.Select(p => CreatePVWatts5UsageProfileAsync(p, accountId, provider, credentials.AppID, credentials.AppKey, ouSettings));

            UpsertUsageProfileResponse[] pvWatts5Profiles = await Task.WhenAll(tasks);

            List<EnergyMonthDetails> monthlyProductions;

            SolarProposalResponse ret = new SolarProposalResponse();

            var consumptionBeforeSolar = (request.OriginalConsumption ?? request.Consumption)
                                            .OrderByDescending(c => c.Year)
                                            .ThenByDescending(c => c.Month)
                                            .ToList();

            // if we have Original Consumption, we use the consumption as 
            var consumptionAfterSolar = request.OriginalConsumption != null
                ? request
                    .ConsumptionWithAdders
                    .ToList()
                : new List<EnergyMonthDetails>(consumptionBeforeSolar.Count);

            try
            {
                foreach (var profile in pvWatts5Profiles)
                {

                    monthlyProductions = provider.ExtractMonthDetailsFromPVWatts5UsageProfile(profile);

                    foreach (var monthlyProduction in monthlyProductions)
                    {
                        var postsolarMonth = consumptionAfterSolar.FirstOrDefault(cas => cas.Month == monthlyProduction.Month);

                        if (postsolarMonth != null)
                        {
                            // will set (regular consumption + adders reduction) - solarProduction to PostSolarConsumption 
                            //postsolarMonth.PostSolarConsumption = postsolarMonth.Consumption = postsolarMonth.Consumption - monthlyProduction.Production;
                            postsolarMonth.PostSolarConsumption = postsolarMonth.PostSolarConsumptionOrConsumption - monthlyProduction.Production;
                            // will save the production
                            postsolarMonth.Production += monthlyProduction.Production;
                        }
                        else
                        {
                            var presolarMonth = consumptionBeforeSolar.First(cbs => cbs.Month.Equals(monthlyProduction.Month));
                            decimal consumption = presolarMonth.PostAddersConsumptionOrConsumption - monthlyProduction.Production;

                            consumptionAfterSolar.Add(new EnergyMonthDetails
                            {
                                Month = presolarMonth.Month,
                                Year = presolarMonth.Year,
                                Consumption = consumption,
                                PostAddersConsumption = presolarMonth.Consumption,
                                PostSolarConsumption = consumption,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SaveAuditData(proposalID, ouid, accountId);
                // there are scenarios when the genability call fails, and something crashes on our end.
                var genabilityResponse = $"Genability Response: {JsonConvert.SerializeObject(pvWatts5Profiles)}";
                throw new ApplicationException(genabilityResponse, ex);
            }

            consumptionAfterSolar = consumptionAfterSolar
                                        .OrderByDescending(pc => pc.Year)
                                        .ThenByDescending(pc => pc.Month).ToList();

            CostCalculatorResponse costCalculatorResponse = provider.CalculateCost(credentials.AppID,
                                                                    credentials.AppKey, accountId, request.TariffID, consumptionAfterSolar);

            var calculatedCost = costCalculatorResponse.results.FirstOrDefault();

            consumptionAfterSolar.ForEach(postSolarMonth =>
            {
                CalculatedCostItem calculatedCostItem = calculatedCost.items.First(cc => cc.fromDateTime.HasValue && cc.fromDateTime.Value.Month.Equals(postSolarMonth.Month));

                var presolarMonth = consumptionBeforeSolar.First(cbs => cbs.Month.Equals(postSolarMonth.Month));

                postSolarMonth.PostSolarCost = calculatedCostItem.cost.Value;

                ret.Months.Add(new EnergyMonthDetails
                {
                    PostAddersConsumption = postSolarMonth.PostAddersConsumptionOrConsumption,
                    PostSolarConsumption = postSolarMonth.PostSolarConsumptionOrConsumption,
                    Consumption = presolarMonth.Consumption,
                    Month = presolarMonth.Month,
                    Year = presolarMonth.Year,
                    PostSolarCost = postSolarMonth.PostSolarCost,
                    PreSolarCost = presolarMonth.PreSolarCost,
                    Production = postSolarMonth.Production
                });

            });

            ret.Months = ret.Months.OrderByDescending(m => m.Year).ThenBy(m => m.Month).ToList();
            ret.Consumption = ret.Months.Sum(m => m.Consumption);
            ret.PostAddersConsumption = ret.Months.Sum(m => m.PostAddersConsumption);
            ret.PostSolarConsumption = ret.Months.Sum(m => m.PostSolarConsumption);
            ret.PostSolarCost = ret.Months.Sum(m => m.PostSolarCost);
            ret.PreSolarCost = ret.Months.Sum(m => m.PreSolarCost);
            ret.Production = ret.Months.Sum(m => m.Production);
            ret.ProposalID = Guid.NewGuid();

            SaveAuditData(proposalID, ouid, accountId);

            return Ok(ret);
        }

        [HttpPost]
        [Route("{ouid:guid}/{financePlanDefinitionId:guid}/{financePlanType}/sign/agreements")]
        [ResponseType(typeof(List<SignDocumentResponse>))]
        public async Task<IHttpActionResult> SignAgreement(Guid ouid, Guid financePlanDefinitionId, FinancePlanType financePlanType, SignDocumentRequest request)
        {
            try
            {
                var response = _signatureService.SignDocument(request, ouid, financePlanDefinitionId, financePlanType, FinanceDocumentType.Agreement);
                return Ok(response);
            }
            catch (TemplateNotFoundException exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(exception.Message) });
            }
        }

        [HttpPost]
        [Route("{ouid:guid}/{financePlanDefinitionId:guid}/{financePlanType}/sign/contracts")]
        [ResponseType(typeof(List<SignDocumentResponse>))]
        public async Task<IHttpActionResult> SignContract(Guid ouid, Guid financePlanDefinitionId, FinancePlanType financePlanType, SignDocumentRequest request)
        {
            try
            {
                var response = _signatureService.SignDocument(request, ouid, financePlanDefinitionId, financePlanType, FinanceDocumentType.Contract);
                return Ok(response);
            }
            catch (TemplateNotFoundException exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(exception.Message) });
            }
        }

        [Route("{ouid:guid}/sign/proposals")]
        [HttpPost]
        [ResponseType(typeof(List<SignDocumentResponseHancock>))]
        public async Task<IHttpActionResult> SignProposals(Guid ouid, SignDocumentRequestHancock request)
        {
            try
            {
                var response = _signatureService.SignDocumentHancock(request, ouid, FinanceDocumentType.Proposal);
                return Ok(response);
            }
            catch (TemplateNotFoundException exception)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent(exception.Message) });
            }
        }

        [Route("{ouid:guid}/{financePlanDefinitionId:guid}/hardcreditcheck")]
        [HttpPost]
        [ResponseType(typeof(Models.Spruce.QuoteResponse))]
        public async Task<IHttpActionResult> DoHardCreditCheck(Guid ouid, Guid financePlanDefinitionId, Models.Spruce.QuoteRequest request)
        {
            if (request == null || !request.IsModelValid())
            {
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
            }

            var planDefinition = _financePlanDefinitionService.Get(financePlanDefinitionId, "Provider");
            if (planDefinition?.Provider?.Name != "Spruce Financial")
            {
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
            }

            PrepareRequest(request);

            Integrations.Spruce.IntegrationProvider provider = new Integrations.Spruce.IntegrationProvider(_spruceUrl);
            var response = provider.HardCreditCheck(SpruceQuoteMapper.MapRequestToDTO(request), ConfigurationKeys.SpruceUsername, ConfigurationKeys.SprucePassword);
            if (!String.IsNullOrWhiteSpace(provider.ErrorMessage))
            {
                return await GetErrors(provider.ErrorMessage);
            }
            request.QuoteResponse = SpruceQuoteMapper.MapDTOToResponse(response, request.Guid);

            _spruceQuoteRequestService.Insert(request);

            return Ok(request.QuoteResponse);
        }

        [Route("{ouid:guid}/{financePlanDefinitionId:guid}/documents")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateDocuments(Guid ouid, Guid financePlanDefinitionId, Integrations.Spruce.DTOs.GenDocsRequest request)
        {
            if (request == null)
            {
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
            }

            var planDefinition = _financePlanDefinitionService.Get(financePlanDefinitionId, "Provider");
            if (planDefinition?.Provider?.Name != "Spruce Financial")
            {
                throw new HttpResponseException(HttpStatusCode.ExpectationFailed);
            }

            Models.Spruce.QuoteRequest quoteRequest = _spruceQuoteRequestService.List().Where(qr => qr.QuoteResponse.QuoteNumber == request.QuoteNumber).First();
            Models.Spruce.GenDocsRequest genDocsRequest = quoteRequest.GenDocsRequest;

            if (genDocsRequest == null)
            {
                genDocsRequest = new Models.Spruce.GenDocsRequest();
            }

            genDocsRequest.QuoteNumber = request.QuoteNumber;
            genDocsRequest.PartnerId = request.PartnerId;
            genDocsRequest.SponsorId = request.SponsorId;
            genDocsRequest.TotalCashSalesPrice = request.TotalCashSalesPrice;
            genDocsRequest.SalesTax = request.SalesTax;
            genDocsRequest.CashDownPayment = request.CashDownPayment;
            genDocsRequest.AmountFinanced = request.AmountFinanced;
            genDocsRequest.InstallCommencementDate = request.InstallCommencementDate;
            genDocsRequest.SubstantialCompletionDate = request.SubstantialCompletionDate;
            genDocsRequest.ProjectedPTODate = request.ProjectedPTODate;
            genDocsRequest.EmailApplicant = request.EmailApplicant;
            genDocsRequest.EmailCoapplicant = request.EmailCoapplicant;
            genDocsRequest.EmailAgreement = request.EmailAgreement;
            genDocsRequest.QuoteRequest = quoteRequest;

            _spruceGenDocsRequestService.Update(genDocsRequest);

            Integrations.Spruce.IntegrationProvider provider = new Integrations.Spruce.IntegrationProvider(_spruceUrl);
            provider.GenerateDocuments(request, ConfigurationKeys.SpruceUsername, ConfigurationKeys.SprucePassword);
            if (!string.IsNullOrWhiteSpace(provider.ErrorMessage))
            {
                return await GetErrors(provider.ErrorMessage);
            }

            return Ok(new { });
        }

        [Route("{ouid:guid}/smartboard/usertoken"), HttpPost, ResponseType(typeof(SBIntegrationLoginModel))]
        public async Task<IHttpActionResult> GetSmartBoardToken(Guid ouid)
        {
            var response = _smartBoardAdapter.Value.GetSBToken(ouid);
            return Ok(response);
        }

        #endregion Common API

        #region Private

        /// <summary>
        /// Creates a PVWatts5UsageProfile.
        /// </summary>
        /// <param name="property">The SolarArrayProperty.</param>
        /// <param name="providerAccountId">The providerAccountId.</param>
        /// <param name="provider">The integration provider.</param>
        /// <param name="genabilityAppID">The genabilityAppID.</param>
        /// <param name="genabilityAppKey">The genabilityAppKey.</param>
        /// <returns>Returns a Task of <see cref="UpsertUsageProfileResponse"/> .</returns>
        private async Task<UpsertUsageProfileResponse> CreatePVWatts5UsageProfileAsync(RoofPlaneInfo property, string providerAccountId, IntegrationProvider provider, string genabilityAppID, string genabilityAppKey, Dictionary<string, ValueTypePair<SettingValueType, string>> ouSettings)
        {
            // Create the PVWatts 5 UsageProfile.
            UpsertPVWattsUsageProfileRequestModel upsertSolarProfileWithIntegratedPVWatts = new UpsertPVWattsUsageProfileRequestModel(ouSettings)
            {
                Azimuth = property.Azimuth.ToString(CultureInfo.InvariantCulture),
                ProfileName = property.ProfileName,
                SystemSize = (property.Size / 1000).ToString(CultureInfo.InvariantCulture),
                Tilt = property.Tilt.ToString(CultureInfo.InvariantCulture),
                Losses = property.GetLosses(ouSettings).ToString(CultureInfo.InvariantCulture),
                InverterEfficiency = property.InverterEfficiency.ToString(CultureInfo.InvariantCulture),
                ModuleType = property.ModuleType.ToString(CultureInfo.InvariantCulture)
            };

            UpsertUsageProfileResponse response = await Task.Run(() => provider.UpsertSolarProfileWithIntegratedPVWatts(genabilityAppID, genabilityAppKey,
                                                                           providerAccountId, property.ProviderProfileId, upsertSolarProfileWithIntegratedPVWatts));


            return response;

        }

        private ThirdPartyCredentials ResolveCredentials(Guid ouid, ThirdPartyProviderType provider)
        {
            switch (provider)
            {
                case ThirdPartyProviderType.Genability:
                    //TODO: implement logic to get credentials from OUSettings later
                    return new ThirdPartyCredentials
                    {
                        AppKey = ConfigurationManager.AppSettings["DataReefGenabilityAppKey"],
                        AppID = ConfigurationManager.AppSettings["DataReefGenabilityAppID"],
                        Url = ConfigurationManager.AppSettings["GenabilityUrl"]
                    };
                case ThirdPartyProviderType.Mosaic:
                    return new ThirdPartyCredentials
                    {
                        Username = ConfigurationManager.AppSettings["MosaicUserName"],
                        Password = ConfigurationManager.AppSettings["MosaicPassword"],
                        Url = ConfigurationManager.AppSettings["MosaicUrl"]
                    };
                case ThirdPartyProviderType.NetSuite:
                    return new ThirdPartyCredentials
                    {
                        Username = ConfigurationManager.AppSettings["NetSuiteUserName"].ToString(),
                        Password = ConfigurationManager.AppSettings["NetSuitePassword"].ToString(),
                        Url = ConfigurationManager.AppSettings["NetSuiteUrl"].ToString()
                    };
                default:
                    break;
            }

            return new ThirdPartyCredentials();
        }

        #endregion Private

    }
}