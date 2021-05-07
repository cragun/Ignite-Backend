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
            var proposal =  _manualProposalService.Get(proposalID);

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

        [Route("smartboardlead"), HttpGet]
        public async Task<IHttpActionResult> smartboardlead()
        {
            List<Guid> Live = new List<Guid>();
            Live.Add(Guid.Parse("A6C906B2-A0B5-491A-BD8E-34C1B0C8EE8C"));
            Live.Add(Guid.Parse("4F048165-F4E3-49FC-B4FC-EF1E3CD796EA"));
            Live.Add(Guid.Parse("C8AD77D0-70AA-4DF4-8A24-7A41BD42156B"));
            Live.Add(Guid.Parse("7B163847-7FAF-4506-ACD3-700CF39F8A28"));
            Live.Add(Guid.Parse("11CC311C-BB20-4C8D-A364-C44165E85B45"));
            Live.Add(Guid.Parse("1F00376F-D4BF-4D8C-88AF-9177781F39FE"));
            Live.Add(Guid.Parse("C44AAA25-0F60-4F66-BF15-5ABAD1B2DEE0"));
            Live.Add(Guid.Parse("E8884408-AA4A-4AA1-B70B-A3943F12EC87"));
            Live.Add(Guid.Parse("3A91A47C-1C4C-4D2D-909F-4F29221A4E5D"));
            Live.Add(Guid.Parse("62D241F4-C3F0-4FF5-9D53-BF7EBB772A58"));
            Live.Add(Guid.Parse("DC5DF88A-0000-486A-B3FE-C5873C888536"));
            Live.Add(Guid.Parse("DF615096-336E-4915-BEAB-85C0422FF7D0"));
            Live.Add(Guid.Parse("3FD17346-8F7B-4D59-A3FA-EE58275895CC"));
            Live.Add(Guid.Parse("5313C893-2FFE-461F-BFD8-082811C04938"));
            Live.Add(Guid.Parse("E3183F01-2C6B-47C6-BD46-6AAFFCE8BD3A"));
            Live.Add(Guid.Parse("2244DA54-1AE7-42C9-B223-587CA097525D"));
            Live.Add(Guid.Parse("5EA4D38A-3B72-4C9A-B903-1C834D2F407D"));
            Live.Add(Guid.Parse("5E0C0A66-5F00-4EF3-9305-AC139A73082F"));
            Live.Add(Guid.Parse("8FE8F5CB-4F3F-401B-A8B3-B28F357F2A5C"));
            Live.Add(Guid.Parse("660542D0-8161-46A2-A286-C2D3463825C7"));
            Live.Add(Guid.Parse("088A1305-B5CD-48C7-A688-B066ED3EA260"));
            Live.Add(Guid.Parse("38D9B003-7440-4235-AED7-027756A49890"));
            Live.Add(Guid.Parse("55480E10-AD18-415A-87C5-8F263C460B44"));
            Live.Add(Guid.Parse("30D0A1FF-FAA9-41AB-B445-8B04A139BEE2"));
            Live.Add(Guid.Parse("D75F38EF-ED8B-4E68-BFA7-F210BD54402C"));
            Live.Add(Guid.Parse("CB976EDA-FD3D-4381-878C-57C073F3BA69"));
            Live.Add(Guid.Parse("C7508F06-E81A-4B60-9BE4-91D3AE9700B2"));
            Live.Add(Guid.Parse("3AA09EA1-C853-40CF-9951-D1F75A609482"));
            Live.Add(Guid.Parse("974FB4F0-BC63-4A97-B72C-EF0357B3F462"));
            Live.Add(Guid.Parse("7B02975C-345D-4536-AB3D-3EF2FA4A418D"));
            Live.Add(Guid.Parse("5EE9940B-BC5E-4F3B-ABBA-BCEEF5176D03"));
            Live.Add(Guid.Parse("E47D1B5C-A71E-492E-8BDE-1390D9AF6292"));
            Live.Add(Guid.Parse("DD0598B3-C400-4E47-850C-5C9431426320"));
            Live.Add(Guid.Parse("A961B9DE-ABA1-47DC-88BE-0250385004AA"));
            Live.Add(Guid.Parse("BA932677-EA7A-4E6D-80ED-6281A9184D48"));
            Live.Add(Guid.Parse("868E2EF8-483A-447F-A9C3-8B5994DF0164"));
            Live.Add(Guid.Parse("189BDCA7-D562-443C-AFE4-12C83B3ED011"));
            Live.Add(Guid.Parse("94E4C9E7-72FB-44D3-A517-989BC7DFED26"));
            Live.Add(Guid.Parse("7B90B475-AD3E-49DC-8DD5-99C3E7A71E89"));
            Live.Add(Guid.Parse("84DCD590-2A95-4604-AF2F-6FD162FB245D"));
            Live.Add(Guid.Parse("389F6B28-CE0F-4395-95F5-A36C4A4AFFEE"));
            Live.Add(Guid.Parse("F7B73949-AC87-4765-812F-D08691F18DFC"));
            Live.Add(Guid.Parse("0FB4A712-E66B-43E5-A6AA-2FA1158C9907"));
            Live.Add(Guid.Parse("CC50DDD7-6A0B-42BC-BD7F-3C0797615B22"));
            Live.Add(Guid.Parse("266F0866-26EA-4134-A7F5-0CBE73D589C1"));
            Live.Add(Guid.Parse("FB6A3F1C-5A33-4646-A72B-8A34A2B28BD3"));
            Live.Add(Guid.Parse("CDCF3E7C-77D0-4E61-92B5-9C62919676F1"));
            Live.Add(Guid.Parse("398964C2-75B5-4E67-BEB9-3E2C9C292B3D"));
            Live.Add(Guid.Parse("CC0D558A-482F-4E11-8200-F1768CBC8F7F"));
            Live.Add(Guid.Parse("5E11A92E-594C-46A3-86DF-B5EC4681CF00"));
            Live.Add(Guid.Parse("9F14ABE7-A522-4937-8EF6-E2B6DB1B6403"));
            Live.Add(Guid.Parse("A7235B8E-A76D-4EC1-B786-EFB533C15CBF"));
            Live.Add(Guid.Parse("4081BB76-8F07-4546-A580-F71E46947207"));
            Live.Add(Guid.Parse("4AD80CED-3E82-46FD-B225-3C2122F0BB6C"));
            Live.Add(Guid.Parse("7298424A-B0E2-4DB6-8FAE-E22C0F2BEA70"));
            Live.Add(Guid.Parse("7DD6C934-2C22-407D-8B2E-81485688AF33"));
            Live.Add(Guid.Parse("CAE01E2A-454E-41DC-846C-9DFCE9B8966E"));
            Live.Add(Guid.Parse("F029AD33-09C2-4229-B273-E3910C802A72"));
            Live.Add(Guid.Parse("3C5E7129-1E18-47A7-8C0D-67686EAFF2AF"));
            Live.Add(Guid.Parse("F09CB9B5-839D-4748-B80D-7B6759E880F5"));
            Live.Add(Guid.Parse("20FF44E9-03B5-40C7-A942-9BC19CC008E1"));
            Live.Add(Guid.Parse("41A4293B-EAEC-4BCE-AEC3-8C0C5052D298"));
            Live.Add(Guid.Parse("F074FCDB-EF2A-4565-8D32-DEC7847F8906"));
            Live.Add(Guid.Parse("716563CF-F1AB-4BBA-9CD2-C4E1D5E6FCF5"));
            Live.Add(Guid.Parse("4D680482-F48A-4AF0-ADFE-CBD3ED7E73A1"));
            Live.Add(Guid.Parse("432665EB-374F-4DC1-AEB3-D96A20194C9F"));
            Live.Add(Guid.Parse("0AAE90D5-BE4F-4DDE-B956-735A92ED6F6D"));
            Live.Add(Guid.Parse("DFC216DC-D050-4EEC-A9B9-8DE81FA109AB"));
            Live.Add(Guid.Parse("915011BB-AAA1-41B0-B9AE-50D74D6B322E"));
            Live.Add(Guid.Parse("F660CE9F-3162-4B60-86AD-474E6DBCE087"));
            Live.Add(Guid.Parse("7A70648B-5B13-40AB-983A-5CDBE950F9E1"));
            Live.Add(Guid.Parse("2993453C-3FA4-4A56-B1C6-6CF7D7CC4F4D"));
            Live.Add(Guid.Parse("F674253D-35ED-4EB9-BB0F-C0D010D1EAE4"));
            Live.Add(Guid.Parse("94DEA0C8-A143-49AE-9FCC-2A5D7399C5CB"));
            Live.Add(Guid.Parse("CAA35EA5-9CE6-46B8-B862-92A637FA7361"));
            Live.Add(Guid.Parse("B3FA41E5-7CA7-41CD-936D-2676F5CC5F38"));
            Live.Add(Guid.Parse("C7DFE079-9D92-462F-99D1-F9D9FE860279"));
            Live.Add(Guid.Parse("8A3E8F28-902F-4B5E-88A1-849CB7C96C05"));
            Live.Add(Guid.Parse("E608DDCD-0112-4C3E-B265-1F515E0A05E8"));
            Live.Add(Guid.Parse("B51222DB-FD04-4DBF-845A-273698328153"));
            Live.Add(Guid.Parse("DD96F3C3-0112-46CD-A905-3854CECC7928"));
            Live.Add(Guid.Parse("09917825-D7E8-4F9D-BF26-F0C378B1453F"));
            Live.Add(Guid.Parse("FE9407E9-2039-401C-A8E8-36EC9C97DC7F"));
            Live.Add(Guid.Parse("EF3EF951-388B-46B1-A124-19CD4CC51ED1"));
            Live.Add(Guid.Parse("B67A68B8-26D9-4141-91E6-E7961BDD900C"));
            Live.Add(Guid.Parse("C84B153F-20F3-4F34-A993-5A62E52F0294"));
            Live.Add(Guid.Parse("5B88827C-E706-43D0-A280-70A41AD17A7B"));
            Live.Add(Guid.Parse("16506E43-DD2B-4110-804A-053E854EAC09"));
            Live.Add(Guid.Parse("02C2816D-A578-497A-8FE4-7CF199A91C25"));
            Live.Add(Guid.Parse("C1CB40F5-6EB8-41AB-857D-5CA4207A74A5"));
            Live.Add(Guid.Parse("C2BD8B2B-DFFB-4166-B5FB-A1BEF7FD5550"));
            Live.Add(Guid.Parse("E8334BFE-EC50-47A0-B8CE-1238B6C5FDFC"));
            Live.Add(Guid.Parse("DD469A8E-0CAA-4C44-A44E-721D6C3DFB27"));
            Live.Add(Guid.Parse("97B9E427-1617-4409-A3E4-EA4397760E73"));
            Live.Add(Guid.Parse("1C8061CC-8DF1-4B09-843D-E82AA4AB1E72"));
            Live.Add(Guid.Parse("6E8FE484-7A3A-46AE-A409-C4CF0989C53D"));
            Live.Add(Guid.Parse("D28276A2-5591-4252-8CAF-4C7BE39DBCAC"));
            Live.Add(Guid.Parse("3F156719-D7A0-4BB4-A0D2-3D446F0B914E"));
            Live.Add(Guid.Parse("9833888D-BCFB-43DA-BE06-9851723D1042"));
            Live.Add(Guid.Parse("04C8ADE0-27FD-4A89-8750-85ADDB6321CA"));
            Live.Add(Guid.Parse("748C0DA8-B7B4-4A1E-B0F3-AC638DB06599"));
            Live.Add(Guid.Parse("55FD12D3-393B-4A04-B977-83ABCF90B345"));
            Live.Add(Guid.Parse("52A8762F-BA1D-412E-A65A-BCCB4205E96F"));
            Live.Add(Guid.Parse("F991B7D7-272F-4D1A-A257-B9532E5D936B"));
            Live.Add(Guid.Parse("D2EBD7DA-8261-4DC8-A41B-9F9A0A8290A4"));
            Live.Add(Guid.Parse("5526968D-63FA-4C4A-94D0-EFA6DF5CD26A"));
            Live.Add(Guid.Parse("A579F9AF-24E8-4251-B24B-FBFD91496E69"));
            Live.Add(Guid.Parse("A217C7C8-844E-4A91-8658-6BF1EC4C76C4"));
            Live.Add(Guid.Parse("B88DDB57-CE35-408A-AB0A-2DF1005406B1"));
            Live.Add(Guid.Parse("EC9F2C59-2FC0-43E1-855E-D6DE796683D2"));
            Live.Add(Guid.Parse("AD6F5B10-BC19-47AD-80EF-A0D36CB1ACB4"));
            Live.Add(Guid.Parse("B1CDC8CE-3CA4-48E2-BF11-34D1FF73A9EF"));
            Live.Add(Guid.Parse("C4DA8D63-5CFC-4ED4-BA22-5480B82CF028"));
            Live.Add(Guid.Parse("E8F354F3-CD3C-41AE-A3C5-6EC3E9243196"));
            Live.Add(Guid.Parse("163AF1DB-E875-4B30-8D5C-1A331F28E582"));
            Live.Add(Guid.Parse("7FF3FE5F-F222-4136-80E2-CD0EBB5AE055"));
            Live.Add(Guid.Parse("BF833A5C-4CC3-4D80-B27D-D99C076590C7"));
            Live.Add(Guid.Parse("342784FB-E900-461E-827A-7A8CB79C7B26"));
            Live.Add(Guid.Parse("55CBE114-5CB1-4569-BFBD-5BD6150F574C"));
            Live.Add(Guid.Parse("D1002600-A3BC-4095-92FF-065144B782A0"));
            Live.Add(Guid.Parse("ED096DD3-C0F3-403C-B008-3C360B3DFDC6"));
            Live.Add(Guid.Parse("95C1DD3D-BC47-43C2-A64A-490C45F68030"));
            Live.Add(Guid.Parse("E21F8B41-8FE2-4466-970D-1E4236D0CD07"));
            Live.Add(Guid.Parse("DAD527F4-259F-4DE9-9EB6-815D7C2865F8"));
            Live.Add(Guid.Parse("2403D22D-EF01-432D-9455-C74EABEA75D9"));
            Live.Add(Guid.Parse("79B4F8F8-D2A2-4A87-9F08-91B947ED95FC"));
            Live.Add(Guid.Parse("4724DAED-18AA-43FB-B697-ACF6A9814174"));
            Live.Add(Guid.Parse("AEE83297-A7C9-481C-96A5-2E76EA765A0C"));
            Live.Add(Guid.Parse("9066BA4F-5350-4037-BB51-513A0E7D89E4"));
            Live.Add(Guid.Parse("35AB33CD-39B0-4D40-AFA3-7FEF567EC03D"));
            Live.Add(Guid.Parse("5EFD65D8-B554-4BEB-99D5-A082DE754EAA"));
            Live.Add(Guid.Parse("C749855E-49DA-41FC-90D7-B35839F4EDA8"));
            Live.Add(Guid.Parse("667093BC-9BBE-4A60-B6F0-9F963E7EB945"));
            Live.Add(Guid.Parse("99ADB615-210D-4409-8B40-2FB6A35056C0"));
            Live.Add(Guid.Parse("D1A7194D-7E96-4DDD-8689-B5D3DB8F0BDB"));
            Live.Add(Guid.Parse("BA311C02-AC97-427A-8EB5-939D4B8CB8D0"));
            Live.Add(Guid.Parse("E57B6AD2-986A-40A8-AB46-3B6A993B5444"));
            Live.Add(Guid.Parse("FAFFDDFE-3406-456C-AD44-9E4DF67854ED"));
            Live.Add(Guid.Parse("49C277C4-AC49-4DB8-B0D1-372D4DAC8C24"));
            Live.Add(Guid.Parse("11D09973-E63A-4813-9FEE-DE9A9B3668FC"));
            Live.Add(Guid.Parse("F5A573E9-8D4F-4E66-AE9E-729179ADB9C5"));
            Live.Add(Guid.Parse("1C6C6060-66BF-4D27-9721-739BB0DBF237"));
            Live.Add(Guid.Parse("ABBF8172-8F53-47AA-B64E-02357AA608E9"));
            Live.Add(Guid.Parse("5E47CCD9-8E67-423B-9192-88E709A371E1"));
            Live.Add(Guid.Parse("4A640358-F0D0-4DA9-97E2-FA2FCE6780A3"));
            Live.Add(Guid.Parse("063B5812-5CAD-4FEC-A314-4AE3C6794704"));
            Live.Add(Guid.Parse("54D41C67-474F-41A8-9189-6D02C09D731B"));
            Live.Add(Guid.Parse("D2B3D039-C074-486E-8868-D05C4B552561"));
            Live.Add(Guid.Parse("986926DC-248A-4F08-8305-1E9FE85A2DF0"));
            Live.Add(Guid.Parse("F1C9B732-7E2D-40B7-AFB3-694B0436BA3D"));
            Live.Add(Guid.Parse("36B6045D-5BC2-44F9-9874-519CDB84BE8E"));
            Live.Add(Guid.Parse("13E5440F-3FBF-48A4-B280-B664E2E89694"));
            Live.Add(Guid.Parse("67EADBEC-95CC-40CC-9417-8202357467D0"));
            Live.Add(Guid.Parse("F90A7EA3-C95C-40D1-B7A8-01E7F1916A66"));
            Live.Add(Guid.Parse("B91E3BA8-91FA-497E-8F63-1B2079393230"));
            Live.Add(Guid.Parse("609FC4B5-1DC6-42BC-A695-E5ED8BE50FFA"));
            Live.Add(Guid.Parse("04352F6E-3763-49D7-8A20-5D40591BA4C6"));
            Live.Add(Guid.Parse("BD180CEE-9CD4-4DE9-93ED-D9D604F87A51"));
            Live.Add(Guid.Parse("383C0385-249F-4679-AD49-797A942BB031"));
            Live.Add(Guid.Parse("CF73A8A7-5F33-4DF7-9E91-A9492861DD12"));
            Live.Add(Guid.Parse("3068465B-4C85-4D3F-ABFD-B14C0E769A04"));
            Live.Add(Guid.Parse("F2A281E6-91F7-4189-BADE-B6131847C1F7"));
            Live.Add(Guid.Parse("FA622676-665D-4078-9AC7-4E4D874A25EC"));
            Live.Add(Guid.Parse("32123005-8A14-4B78-8FFC-BEF75E473B60"));
            Live.Add(Guid.Parse("9F0DED75-E283-4A82-BB4B-3BB6C4F7CC5B"));
            Live.Add(Guid.Parse("0B0DC07E-61A1-4A8A-8035-EAFF0BDA931D"));
            Live.Add(Guid.Parse("5CFDB4CD-42BB-4674-A3D0-281FA150088C"));
            Live.Add(Guid.Parse("5B63D427-9A2A-4893-8253-D9C7F3BC0D78"));
            Live.Add(Guid.Parse("8A4C22CE-8BA7-4CB8-AC3C-6E42B264DD44"));
            Live.Add(Guid.Parse("EEC997B5-860C-4F53-AE62-8BA4016A28DA"));
            Live.Add(Guid.Parse("BFF087B4-AC38-4CEE-9670-CC85EE74D343"));
            Live.Add(Guid.Parse("1F03219F-8F74-4AA4-A42F-3B06688CAD3A"));
            Live.Add(Guid.Parse("E1095A55-80AB-447C-B686-7A2BE92CB7B8"));
            Live.Add(Guid.Parse("DF314AD2-0ED2-4E26-807D-90B47A88ABEE"));
            Live.Add(Guid.Parse("369B4236-1D54-42A0-B472-AF9BD23268BA"));
            Live.Add(Guid.Parse("589F91E6-B539-46CC-8B5A-6203C3CF82C8"));
            Live.Add(Guid.Parse("DA9D5A90-3CCC-4FB6-AF71-77D86BD61CBA"));
            Live.Add(Guid.Parse("BE698D2B-CA17-43B8-B226-19BB25F5558A"));
            Live.Add(Guid.Parse("6A080B28-83BA-4C83-899B-F587928E4BC3"));
            Live.Add(Guid.Parse("374B1E7E-6C9F-4F6D-891F-E2AC4B2A4E11"));
            Live.Add(Guid.Parse("BCED9A49-DC4A-44FC-87F5-E53B27C5FCFF"));
            Live.Add(Guid.Parse("AE51F918-74CC-4B7A-AB16-D0C87E5CFAF9"));
            Live.Add(Guid.Parse("34EC9CDA-19A9-4DCA-8CD7-206EF31DEE4F"));
            Live.Add(Guid.Parse("44672211-6738-4A12-9725-01ED3E49ABF6"));
            Live.Add(Guid.Parse("CA3CDC34-ADB7-4F55-9D01-F54C966704F1"));
            Live.Add(Guid.Parse("5073E818-2ACA-4D80-B5CD-3D3779BDB871"));
            Live.Add(Guid.Parse("C2F88BEF-FB69-4767-B5CF-3E8E54169E19"));
            Live.Add(Guid.Parse("83D571D6-8AB8-4480-9710-194E4F7BEF64"));
            Live.Add(Guid.Parse("DBD25A22-3ACE-4818-8BB5-20195C3407FE"));
            Live.Add(Guid.Parse("480B7DF5-108F-44C1-91A2-4AC735C750B8"));
            Live.Add(Guid.Parse("0021C9AF-02A4-4391-AAC2-FBE3C42F4B78"));
            Live.Add(Guid.Parse("FEFAEBAA-8B74-4D44-9BE1-566CC55D6591"));
            Live.Add(Guid.Parse("D035F748-81E8-4919-AF42-BE11180B3D9C"));
            Live.Add(Guid.Parse("E6AF360E-E915-48D8-A1CA-6C25473FD9E5"));


            var list = new List<test>();

            foreach (var i in Live)
            {
                var response = _smartBoardAdapter.Value.SubmitLead(i);
            }

            return Ok();
        }

        public class test
        {
            public string guid { get; set; }

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