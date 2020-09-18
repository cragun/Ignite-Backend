using DataReef.Core.Attributes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.FinanceAdapters.SMARTBoard;
using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.Proposals;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Geo;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services.InternalServices;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Auth.Helpers;
using DataReef.TM.Models.DTOs.Signatures;

namespace DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ISolarSalesTrackerAdapter))]
    public class SolarSalesTrackerAdapter : FinancialAdapterBase, ISolarSalesTrackerAdapter
    {
        private readonly Lazy<IRepository> _repository;
        private readonly Lazy<IUtilServices> _utilServices;
        private readonly Lazy<IProposalService> _proposalService;
        private SMARTBoardIntegrationOptionData _sstSettings;

        public SolarSalesTrackerAdapter(
            Lazy<IOUSettingService> ouSettingService,
            Lazy<IRepository> repository,
            Lazy<IProposalService> proposalService,
            Lazy<IUtilServices> utilServices)
            : base(SolarTrackerResources.Name, ouSettingService)
        {
            _repository = repository;
            _utilServices = utilServices;
            _proposalService = proposalService;
        }

        public override string GetBaseUrl(Guid ouid)
        {
            return _sstSettings?.BaseUrl ?? ConfigurationManager.AppSettings[SolarTrackerResources.BaseUrl];
        }

        public override AuthenticationContext GetAuthenticationContext(Guid ouid)
        {
            return null;
        }

        public override TokenResponse AuthorizeAdapter(AuthenticationContext authenticationContext)
        {
            return null;
        }

        public override Dictionary<string, string> GetCustomHeaders(TokenResponse tokenResponse)
        {
            return null;
        }

        public void SetSSTSettings(SMARTBoardIntegrationOptionData sstSettings)
        {
            _sstSettings = sstSettings;
            serviceUrl = _sstSettings?.BaseUrl ?? ConfigurationManager.AppSettings[SolarTrackerResources.BaseUrl] ?? serviceUrl;
        }

        public SstResponse SubmitSolarData(Guid financePlanID)
        {
            FinancePlan financePlan;
            ProposalData proposalData = null;
            Person dealer;
            string homePhoneNumber;
            string emailAddress;
            using (var repository = _repository.Value)
            {
                financePlan = repository.Get<FinancePlan>()
                    .Include(fp => fp.SolarSystem.Proposal.Property.Territory)
                    .Include(fp => fp.SolarSystem.Proposal.Tariff)
                    .Include(fp => fp.FinancePlanDefinition.Provider)
                    .FirstOrDefault(fp => fp.Guid == financePlanID);

                proposalData = repository
                                .Get<ProposalData>()
                                .FirstOrDefault(pd => pd.FinancePlanID == financePlanID);

                if (financePlan == null)
                    throw new ApplicationException("Invalid finance plan ID");

                dealer = repository.Get<Person>()
                    .FirstOrDefault(p => p.Guid == financePlan.SolarSystem.Proposal.PersonID);
                if (dealer == null)
                    throw new ApplicationException("Invalid dealer ID");

                homePhoneNumber = repository.Get<Field>()
                    .Where(f => f.DisplayName == "Phone Number" &&
                                f.PropertyId == financePlan.SolarSystem.Proposal.PropertyID)
                    .Select(f => f.Value)
                    .FirstOrDefault();
                emailAddress = repository.Get<Field>()
                    .Where(f => f.DisplayName == "Email Address" &&
                                f.PropertyId == financePlan.SolarSystem.Proposal.PropertyID)
                    .Select(f => f.Value)
                    .FirstOrDefault();
            }

            var proposal = financePlan.SolarSystem.Proposal;
            var ouid = proposal.Property.Territory.OUID;

            EnsureInitialized(ouid);
            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };
            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;

            if (integrationData == null)
            {
                throw new ApplicationException(SolarTrackerResources.Exceptions.SmartBoardSettingsNotFound);
            }

            if (string.IsNullOrWhiteSpace(integrationData.ApiKey))
                throw new ApplicationException(SolarTrackerResources.Exceptions.ApiKeyNotSet);


            var salesOwner = dealer.EmailAddresses != null && dealer.EmailAddresses.Any()
                    ? dealer.EmailAddresses.First()
                    : dealer.EmailAddressString;

            var names = proposal.NameOfOwner.Split(' ');
            var createLeadRequest = new SstRequest
            {
                Lead = new SstRequestLead
                {
                    //  email address needs to be registered into SST
                    //SalesOwner = "nate@solarsalestracker.com"
                    SalesOwner = salesOwner
                },
                Customer = new SstRequestCustomer
                {
                    CustomerFirstName = names[0],
                    CustomerLastName = names[names.Length - 1],
                    Email = emailAddress,
                    Phone = homePhoneNumber,
                    Address = string.IsNullOrEmpty(proposal.Address)
                        ? proposal.Property.StreetName
                        : proposal.Address,
                    City = proposal.City,
                    State = proposal.State,
                    Zip = proposal.ZipCode
                },
                EnergyUsage = new SstRequestEnergyUsage
                {
                    UtilityCompany = proposal.Tariff?.UtilityName
                },
                LeadKwh = new SstRequestLeadKwh { SystemSize = (decimal)financePlan.SolarSystem.SystemSize / 1000 }
                //Lender = financePlan.FinancePlanDefinition.Provider.Name
            };

            // var proposalData = financePlan.ProposalData.OrderByDescending(pd => pd.DateCreated).FirstOrDefault();
            //  if proposal is signed send also proposal data
            if (proposalData?.SignatureDate != null)
            {
                createLeadRequest.Proposal = new SstRequestProposal
                {
                    Name = proposal.Name,
                    Url = financePlan.SolarSystem.Proposal.ProposalURL
                };
            }

            return SubmitLeadRequest(integrationData, createLeadRequest, ouid);
        }

        public SstResponse SubmitLead(Guid propertyID, Guid? overrideEC = null, bool disposeRepo = true, bool IsdispositionChanged = false)
        {
            var repository = _repository.Value;

            var property = repository
                            .Get<Property>()
                            .Include(p => p.Territory)
                            .Include(p => p.PropertyBag)
                            .Include(p => p.Occupants)
                            .Include(p => p.Appointments)
                            .Include(p => p.Appointments.Select(prop => prop.Assignee))
                            .Include(p => p.Appointments.Select(prop => prop.Creator))
                            .Include(p => p.Inquiries)
                            .Include(p => p.Proposals.Select(prop => prop.Tariff))
                            .Include(p => p.Proposals.Select(prop => prop.SolarSystem))
                            .Include(p => p.Proposals.Select(prop => prop.SolarSystem.PowerConsumption))
                            .FirstOrDefault(fp => fp.Guid == propertyID);

            var ouid = property.Territory.OUID;
            EnsureInitialized(ouid);
            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };
            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;

            // no SST/SB settings for this OU. bail
            if (integrationData == null)
            {
                return null;
            }

            var dealer = repository
                            .Get<Person>()
                            .FirstOrDefault(p => p.Guid == SmartPrincipal.UserId);

            //set another person as Energy Consultant, not the one from the appointment. Spare going to the db if user is the same as dealer
            var closer = overrideEC == null
                ? null
                : overrideEC.Value == SmartPrincipal.UserId
                    ? dealer
                    : repository
                                .Get<Person>()
                                .FirstOrDefault(p => p.Guid == overrideEC);

            //var mainOccupant = property.GetMainOccupant();
            var appointment = property.GetLatestAppointment();
            if (IsdispositionChanged)
            {
                appointment = null;
            }

            var proposal = property.Proposals?.FirstOrDefault();
            var tariff = proposal?.Tariff;

            var request = new SBLeadCreateRequest
            {
                Lead = new SBLeadModel(property, dealer, closer),
                Customer = new SBCustomerModel(property),
            };
            if (proposal?.SolarSystem != null)
            {
                request.LeadKwh = new SBLeadKwhModel(proposal?.SolarSystem);
            }
            if (appointment != null)
            {
                request.Appointment = new SBAppointmentModel(property);
            }
            if (tariff != null)
            {
                request.EnergyUsage = new SBEnergyUtilityModel(proposal);
            }

            if (proposal?.SolarSystem?.PowerConsumption?.Any() == true)
            {
                request.MonthlyUsage = new SBUsageModel(proposal?.SolarSystem);
            }

            //request.HOA = new HOAModel();
            //request.HOA.Name = property?.PropertyBag?.FirstOrDefault(x => x.DisplayName == "HOA/Management Name")?.Value;
            //request.HOA.PhoneEmail = property?.PropertyBag?.FirstOrDefault(x => x.DisplayName == "HOA/Management Phone/Email")?.Value;

            var response = SubmitLeadRequest(integrationData, request, ouid, property.SmartBoardId.HasValue);

            if (long.TryParse(response?.NewLead?.Lead_ID, out long leadId))
            {
                property.SmartBoardId = leadId;
                repository.SaveChanges();
            }

            if (disposeRepo)
            {
                repository.Dispose();
            }
            return response;
        }


        public SBIntegrationLoginModel GetSBToken(Guid ouid)
        {
            EnsureInitialized(ouid);
            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };
            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;

            // no SST/SB settings for this OU. 
            if (integrationData == null)
            {
                return new SBIntegrationLoginModel { Message = "No SmartBOARD integration settings found!" };
            }

            //get user email by its id
            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);
            string email;
            using (DataContext dc = new DataContext())
            {
                email = dc.Credentials.FirstOrDefault(x => x.UserID == SmartPrincipal.UserId)?.UserName;
            }


            var headers = new Dictionary<string, string>
            {
                {"x-sm-api-key", encryptedAPIkey},
                {"x-sm-email", email},
            };

            var url = "/apis/create_user_token";

            var response = MakeRequest<SBUserTokenResponse>(integrationData.BaseUrl, url, Method.POST, headers);

            if (response != null)
            {
                try
                {
                    //update the user's SmartBoard ID
                    using (var dc = new DataContext())
                    {
                        var currentPerson = dc.People.FirstOrDefault(x => x.Guid == SmartPrincipal.UserId);
                        if (currentPerson != null)
                        {
                            currentPerson.SmartBoardID = response?.User?.Id ?? currentPerson.SmartBoardID;
                            currentPerson.Updated();
                        }

                        dc.SaveChanges();

                    }
                    SaveRequest(null, response, url, headers, integrationData.ApiKey);
                }
                catch (Exception)
                {
                }
            }
            return new SBIntegrationLoginModel
            {
                Message = response?.Message?.Text,
                Token = response?.User?.Token,
                ApiKey = encryptedAPIkey
            };
        }

        public SBGetDocument GetProposalDocuments(Property property)
        {
            var ouid = property?.Territory?.OUID;
            if (!ouid.HasValue)
            {
                throw new ApplicationException("Orgainization Not Found");
            }
            EnsureInitialized(ouid.Value);
            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                throw new ApplicationException("Something Went Wrong");
            }

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

            var url = $"/apis/get_lead_documents/{encryptedAPIkey}";

            var request = new SstRequest
            {
                Lead = new SstRequestLead
                {
                    AssociatedID = property.Id
                },
            };

            var response = MakeRequest(ouid.Value, url, request, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());

            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response, url, null, integrationData.ApiKey);
            }
            catch (Exception)
            {
            }
            return JsonConvert.DeserializeObject<SBGetDocument>(response);
        }

        public SBGetDocument GetOuDocumentType(Guid ouid)
        {
            EnsureInitialized(ouid);
            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                throw new ApplicationException("Something Went Wrong");
            }

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

            var url = $"/apis/document_tabs_and_types/{encryptedAPIkey}";

            var response = MakeRequest(ouid, url, null, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());

            try
            {
                SaveRequest(null, response, url, null, integrationData.ApiKey);
            }
            catch (Exception)
            {
            }

            return JsonConvert.DeserializeObject<SBGetDocument>(response);
        }

        public void SBActiveDeactiveUser(bool IsActive, string sbid)
        {

            using (DataContext dc = new DataContext())
            {
                var ret = dc
                        .Database
                        .SqlQuery<OU>("exec proc_OUsForPerson {0}", SmartPrincipal.UserId)
                        .Where(o => !o.IsArchived)
                        .ToList();

                var ouid = ret.FirstOrDefault().Guid;

                EnsureInitialized(ouid);

                var integrationSettings = new IntegrationOptionSettings
                {
                    Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                    SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

                };

                var integrationData =
                    integrationSettings
                    ?.SelectedIntegrations
                    ?.FirstOrDefault(x =>
                    {
                        var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                        return matchingOption?.Type == IntegrationType.SMARTBoard;
                    })
                    ?.Data
                    ?.SMARTBoard;
                if (integrationData == null)
                {
                    return;
                }

                string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

                var apiMethod = IsActive ? "deactivate_user" : "activate_user";

                var url = $"/apis/{apiMethod}/{encryptedAPIkey}";

                var request = new SBLeadCreateRequest
                {
                    UserId = sbid,
                };

                var response = MakeRequest(ouid, url, request, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());

                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url, null, integrationData.ApiKey);
                }
                catch (Exception)
                {
                }
            }

        }

        public void SignAgreement(Proposal proposal, string documentTypeId, SignedDocumentDTO proposalDoc)
        {
            var ouid = proposal?.Property?.Territory?.OUID;
            if (!ouid.HasValue || proposalDoc == null)
            {
                return;
            }

            EnsureInitialized(ouid.Value);

            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                return;
            }
            string email;
            using (DataContext dc = new DataContext())
            {
                email = dc.People.FirstOrDefault(x => x.Guid == SmartPrincipal.UserId)?.EmailAddressString;
            }

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

            var url = $"/apis/add_document/{encryptedAPIkey}";
            var request = new SBAddDocument
            {
                Document = new DocumentModel
                {
                    AssociatedId = proposal?.Property?.Id,
                    DocumentName = proposalDoc.Name,
                    DocumentUrl = proposalDoc.PDFUrl,
                   // DocumentUrl = proposalDoc.Url,
                    DocumentTypeId = documentTypeId,
                    Email = email,
                    Lon = proposal?.Lon,
                    Lat = proposal?.Lat,
                    Signed = true
                }
            };

            var response = MakeRequest(ouid.Value, url, request, serializer: new RestSharp.Serializers.RestSharpJsonSerializer(), method: Method.GET);

            try
            {
                SaveRequest(request, response, url, null, integrationData.ApiKey);
            }
            catch (Exception)
            {
            }

        }

        public void UploadDocumentItem(Property property, string documentTypeId, ProposalMediaItem proposalDoc)
        {
            var ouid = property?.Territory?.OUID;
            if (!ouid.HasValue || proposalDoc == null)
            {
                return;
            }

            EnsureInitialized(ouid.Value);

            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                return;
            }
            string email;
            using (DataContext dc = new DataContext())
            {
                email = dc.People.FirstOrDefault(x => x.Guid == SmartPrincipal.UserId)?.EmailAddressString;
            }

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

            var url = $"/apis/add_document/{encryptedAPIkey}";

            var request = new SBAddDocument
            {
                Document = new DocumentModel
                {
                    AssociatedId = property?.Id,
                    DocumentName = proposalDoc.Name,
                    DocumentUrl = proposalDoc.Url,
                    DocumentTypeId = documentTypeId,
                    Email = email,
                    Lon = property?.Longitude,
                    Lat = property?.Latitude,
                    Signed = true
                }
            };

            var response = MakeRequest(ouid.Value, url, request, serializer: new RestSharp.Serializers.RestSharpJsonSerializer(), method: Method.POST);

            try
            {
                SaveRequest(request, response, url, null, integrationData.ApiKey);
            }
            catch (Exception)
            {
            }

        }
        public SstResponse AttachProposal(Proposal proposal, Guid proposalDataId, SignedDocumentDTO proposalDoc)
        {
            var ouid = proposal?.Property?.Territory?.OUID;
            if (!ouid.HasValue || proposalDoc == null)
            {
                return null;
            }

            EnsureInitialized(ouid.Value);

            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                return null;
            }

            // Push the lead to SB if it's not there yet.
            if (!proposal.Property.SmartBoardId.HasValue)
            {
                var response = SubmitLead(proposal.PropertyID, disposeRepo: false);
                proposal.Property.SmartBoardId = response?.GetNewLeadID();
            }

            var dealer = _repository
                .Value
                .Get<Person>()
                .FirstOrDefault(p => p.Guid == SmartPrincipal.UserId);

            using (var repository = _repository.Value)
            {
                var proposalData = repository
                            .Get<ProposalData>()
                            .FirstOrDefault(pd => pd.Guid == proposalDataId);


                var ProjectData = BuildProposalDataModelFromProposalData(proposalData);

                var request = new SBProposalAttachRequest(proposal)
                {
                    Lead = new SBLeadModel(proposal?.Property, dealer),
                    Proposal = new SBProposalModel
                    {

                        Name = (proposal?.Property.Name + "_" +
                                 ProjectData?.ModuleCount + "_" +
                                ProjectData?.ModuleModel + "_" +
                                ProjectData?.ModuleSize + "_" +
                                ProjectData?.SystemSize + "kW_" +
                                proposalDoc?.ProviderName + "_" +
                                proposalDoc?.Year + "_" +
                                proposalDoc?.Apr + "_" +
                                proposalData?.SignatureDate?.Year + "." + proposalData?.SignatureDate?.Month + "." + proposalData?.SignatureDate?.Day + "_" +
                                proposalData?.SignatureDate?.Hour + "." + proposalData?.SignatureDate?.Minute).Replace(" ", ""),


                        ProposalName = proposalDoc.Description,
                        Url = proposalDoc.PDFUrl,
                        EnergyBillUrl = proposalDoc.EnergyBillUrl,
                        SignedDate = proposalData?.SignatureDate,
                        SignedLocation = proposalData?.MetaInformation?.AcceptLocation
                    },
                    ProjectData = ProjectData,
                };


                //var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(request);
                //if (json != null)
                //{
                //    ApiLogEntry apilog = new ApiLogEntry();
                //    apilog.Id = Guid.NewGuid();
                //    apilog.User = "/sign/proposal/SubmitProposal";
                //    apilog.Machine = Environment.MachineName;
                //    apilog.RequestContentType = "";
                //    apilog.RequestRouteTemplate = "";
                //    apilog.RequestRouteData = "";
                //    apilog.RequestIpAddress = "";
                //    apilog.RequestMethod = "";
                //    apilog.RequestHeaders = "";
                //    apilog.RequestTimestamp = DateTime.UtcNow;
                //    apilog.RequestUri = "";
                //    apilog.ResponseContentBody = "";
                //    apilog.RequestContentBody = json;

                //    using (var dc = new DataContext())
                //    {
                //        dc.ApiLogEntries.Add(apilog);
                //        dc.SaveChanges();
                //    }
                //}

                return SubmitProposal(integrationData, request, ouid.Value);
            }


        }

        public SBProposalDataModel BuildProposalDataModel(Guid proposalDataId)
        {
            using (var repository = _repository.Value)
            {
                var proposalData = repository
                            .Get<ProposalData>()
                            .FirstOrDefault(pd => pd.Guid == proposalDataId);
                return BuildProposalDataModelFromProposalData(proposalData);
            }
        }

        public SBProposalDataModel BuildProposalDataModelFromProposalData(ProposalData proposalData)
        {
            if (proposalData == null)
            {
                return null;
            }
            using (var repository = _repository.Value)
            {

                var proposal = repository
                                .Get<Proposal>()
                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.SolarPanel))
                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Panels))
                                .Include(p => p.SolarSystem.RoofPlanes.Select(rp => rp.Inverter))
                                .Include(p => p.SolarSystem.AdderItems)
                                .Include(p => p.SolarSystem.FinancePlans.Select(fp => fp.FinancePlanDefinition.Provider))
                                .Include(p => p.SolarSystem.SystemProduction.Months)
                                .Include(p => p.Property)
                                .Include(p => p.Property.PropertyBag)
                                .FirstOrDefault(p => p.Guid == proposalData.ProposalID);

                var financePlan = proposal
                                .SolarSystem?
                                .FinancePlans?
                                .FirstOrDefault();

                if (proposal == null || financePlan == null)
                {
                    return null;
                }

                var proposalDataView = _proposalService.Value.GetProposalDataView(proposalData.Guid, null);

                if (proposalDataView == null)
                {
                    return null;
                }
                return new SBProposalDataModel(proposal, financePlan, proposalDataView);
            }
        }

        public string AddUserTaggingNotification(Property property, Guid createdByID, Guid taggedID)
        {
            var ouid = property?.Territory?.OUID;
            if (!ouid.HasValue)
            {
                return null;
            }

            EnsureInitialized(ouid.Value);

            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                return null;
            }

            using (var dc = new DataContext())
            {
                var people = dc.People.Where(x => x.Guid == createdByID || x.Guid == taggedID).ToList();

                var createdBySmartboardID = people.FirstOrDefault(x => x.Guid == createdByID)?.SmartBoardID;
                var taggedSmartboardID = people.FirstOrDefault(x => x.Guid == taggedID)?.SmartBoardID;

                if (!string.IsNullOrEmpty(createdBySmartboardID) && !string.IsNullOrEmpty(taggedSmartboardID))
                {
                    SetSSTSettings(integrationData);

                    string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

                    var url = $"/apis/add_user_tagging_notification/{encryptedAPIkey}?lead_id={property.SmartBoardId}&from={createdBySmartboardID}&to={taggedSmartboardID}";

                    var response = MakeRequest(ouid.Value, url, null, serializer: new RestSharp.Serializers.RestSharpJsonSerializer(), method: Method.GET);

                    try
                    {
                        SaveRequest(null, response, url, null, integrationData.ApiKey);
                    }
                    catch (Exception)
                    {
                    }

                    return response;
                }
            }

            return null;
        }

        public string DismissNotification(Guid ouid, string smartboardNotificationID)
        {
            EnsureInitialized(ouid);

            var integrationSettings = new IntegrationOptionSettings
            {
                Options = ouSettings.GetByKey<ICollection<IntegrationOption>>(SolarTrackerResources.SettingName),
                SelectedIntegrations = ouSettings.GetByKey<ICollection<SelectedIntegrationOption>>(SolarTrackerResources.SelectedSettingName)

            };

            var integrationData =
                integrationSettings
                ?.SelectedIntegrations
                ?.FirstOrDefault(x =>
                {
                    var matchingOption = integrationSettings?.Options?.FirstOrDefault(o => o.Id == x.Id);

                    return matchingOption?.Type == IntegrationType.SMARTBoard;
                })
                ?.Data
                ?.SMARTBoard;
            if (integrationData == null)
            {
                return null;
            }

            using (var dc = new DataContext())
            {
                SetSSTSettings(integrationData);

                string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(integrationData.ApiKey);

                var url = $"/apis/dismiss_user_tagging_notification/{encryptedAPIkey}?notification_id={smartboardNotificationID}";

                var response = MakeRequest(ouid, url, null, serializer: new RestSharp.Serializers.RestSharpJsonSerializer(), method: Method.GET);

                try
                {
                    SaveRequest(null, response, url, null, integrationData.ApiKey);
                }
                catch (Exception)
                {
                }

                return response;
            }
        }

        private SstResponse SubmitLeadRequest(SMARTBoardIntegrationOptionData settings, object request, Guid ouid, bool update = false)
        {
            SetSSTSettings(settings);

            var apiMethod = update ? "update" : "create";

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(settings.ApiKey);

            var url = $"/apis/{apiMethod}/{encryptedAPIkey}";

            var response = MakeRequest(ouid, url, request, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());

            try
            {
                SaveRequest(JsonConvert.SerializeObject(request), response, url, null, settings.ApiKey);
            }
            catch (Exception)
            {
            }


            return JsonConvert.DeserializeObject<SstResponse>(response);
        }

        private SstResponse SubmitProposal(SMARTBoardIntegrationOptionData settings, object request, Guid ouid)
        {
            SetSSTSettings(settings);

            string encryptedAPIkey = CryptographyHelper.getEncryptAPIKey(settings.ApiKey);

            var url = $"/apis/attach_proposal/{encryptedAPIkey}";

            var response = MakeRequest(ouid, url, request, serializer: new RestSharp.Serializers.RestSharpJsonSerializer());
            if (response != null)
            {
                try
                {
                    SaveRequest(JsonConvert.SerializeObject(request), response, url, null, settings.ApiKey);
                }
                catch (Exception)
                {
                }
            }

            return JsonConvert.DeserializeObject<SstResponse>(response);
        }
    }
}
