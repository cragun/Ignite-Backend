using DataReef.Core.Classes;
using DataReef.Core.Configuration;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.Accounting;
using DataReef.TM.Models.Credit;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services.InternalServices.Geo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Integrations.SolarCloud;
using DataReef.Integrations.SolarCloud.DataViews;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PrescreenInstantService : DataService<PrescreenInstant>, IPrescreenInstantService
    {
        private ITokensProvider tokensProvider;
        private IDataService<TokenExpense> expenseService;
        private IDataService<Property> propertyService;
        private IGeoProvider geoProvider;
        private Lazy<ICloudBridge> _cloudBridge;
        private readonly IOUSettingService _ouSettings;

        public PrescreenInstantService(ILogger logger, IOUSettingService ouSettings, Func<IUnitOfWork> unitOfWorkFactory, Lazy<ICloudBridge> cloudBridge)
            : base(logger, unitOfWorkFactory)
        {
            _ouSettings = ouSettings;
            this.tokensProvider = new TokensProvider();
            this.propertyService = new DataService<Property>(logger, unitOfWorkFactory);
            this.expenseService = new DataService<TokenExpense>(logger, unitOfWorkFactory);


            //todo: we should use Unity or some other framework to inject the dependencies here
            this.geoProvider = new DataReefGeoProvider();
            _cloudBridge = cloudBridge;
        }

        public override PrescreenInstant Update(PrescreenInstant entity)
        {
            throw new ApplicationException("Updates are not supported for this object");
        }


        public override SaveResult Delete(Guid uniqueId)
        {
            throw new ApplicationException("Deletes are not supported for this object");
        }


        public override ICollection<SaveResult> DeleteMany(Guid[] uniqueIds)
        {
            throw new ApplicationException("Deletes are not supported for this object");
        }

        public override PrescreenInstant Insert(PrescreenInstant entity)
        {
            if (entity == null || entity.PropertyID == null)
            {
                throw new ApplicationException("Invalid Territory");
            }

            var userID = SmartPrincipal.UserId;
            entity.CreatedByID = userID;

            //we need to see if the user has a balance; if they dont, they cant do the instant prescreen
            TokenLedger ledger = tokensProvider.GetDefaultLedgerForPerson(userID);

            //if ledger == null then the user has no credits available, no ledger=no credits
            if (ledger == null)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            double balance = tokensProvider.GetBalanceForLedger(ledger.Guid);
            if (balance <= 0)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            //next we need to get the property
            var property = this.propertyService.Get(entity.PropertyID, "Territory");

            if (property == null)
            {
                throw new ApplicationException("Invalid property GUID");
            }

            //insert a new instant prescreen entry into the database; this will be updated with the prescreen result
            PrescreenInstant prescreenInstant = base.Insert(entity);

            //ok, we have reached this we can start to run the prescreen
            Task.Run(() =>
            {
                using (DataContext dc = new DataContext())
                {
                    //get a copy connected to the Data Context so we can update its status
                    PrescreenInstant prescreen = dc.PrescreenInstants
                                                   .Include(ps => ps.Property)
                                                   .Include(ps => ps.Property.Occupants)
                                                   .Include(ps => ps.Property.Territory)
                                                   .FirstOrDefault(bb => bb.Guid == prescreenInstant.Guid);
                    prescreen.Status = PrescreenStatus.Pending;

                    int tokensPerInquiry = int.Parse(ConfigurationManager.AppSettings["InstantPrescreenTokensPerInquiry"]);
                    //the user spends tokens on the instant prescreen
                    TokenExpense expense = new TokenExpense();
                    expense.Amount = tokensPerInquiry;
                    expense.CreatedByID = userID;
                    expense.DateCreated = System.DateTime.UtcNow;
                    expense.LedgerID = ledger.Guid;
                    expense.TenantID = SmartPrincipal.TenantId;
                    expense.InstantPrescreenID = prescreenInstant.Guid;
                    dc.TokenExpenses.Add(expense);

                    try
                    {
                        dc.SaveChanges();

                        switch (entity.Source)
                        {
                            case PrescreenSource.DataReef:
                                var guid = DoDatareefInstantPrescreen(property, prescreen.Guid);
                                prescreen.ExternalID = guid.ToString();
                                dc.SaveChanges();

                                break;
                            case PrescreenSource.Spruce:
                                var propertyAttribute = DoSpruceInstantPrescreen(prescreen, expense, userID);
                                if (propertyAttribute != null)
                                {
                                    propertyAttribute.AttributeKey = $"prescreen-instant-{entity.Source}";

                                    if (propertyAttribute.DisplayType == "review")
                                    {
                                        expense.Amount = 0;
                                        expense.Notes = "prescreen pending";
                                    }
                                    else if (propertyAttribute.DisplayType == "none")
                                    {
                                        expense.Amount = 0;
                                        expense.Notes = "prescreen not found";
                                    }
                                }
                                else
                                {
                                    expense.Amount = 0;
                                    expense.Notes = "no prescreen results";
                                }

                                dc.PropertyAttributes.Add(propertyAttribute);

                                //all done, update status and save
                                prescreen.Status = PrescreenStatus.Completed;
                                prescreen.CompletionDate = System.DateTime.UtcNow;
                                dc.SaveChanges();
                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        prescreen.Status = PrescreenStatus.Error;
                        prescreen.ErrorString = ex.Message + ":" + ex.StackTrace;
                        prescreen.CompletionDate = DateTime.UtcNow;
                        expense.Amount = 0;
                        dc.SaveChanges();
                    }
                }
            });
            return prescreenInstant;
        }

        private Guid DoDatareefInstantPrescreen(Property property, Guid instantPrescreenId)
        {
            var lead = new Lead
            {
                Address = property.Address1,
                City = property.City,
                State = property.State,
                ZipCode = property.ZipCode,
                TerritoryID = property.TerritoryID,
                ExternalID = property.ExternalID
            };
            return _cloudBridge.Value.InitiateWorkflowForLead(lead, property.Territory.OUID, property.TerritoryID, instantPrescreenId);
        }

        private PropertyAttribute DoSpruceInstantPrescreen(PrescreenInstant prescreen, TokenExpense expense, Guid userID)
        {
            var propertyAttributes = new List<PropertyAttribute>();

            string serviceUrl = ConfigurationManager.AppSettings["SpruceUrl"].ToString();

            var settings = _ouSettings.GetSettings(prescreen.Property.Territory.OUID, null);
            string contractorID = settings["Contractor ID"].Value;

            var provider = new Integrations.Spruce.IntegrationProvider(serviceUrl);
            var mainOccupant = prescreen.Property.GetMainOccupant();

            var request = new Integrations.Spruce.DTOs.PrescreenRequest
            {
                ContractorID = contractorID,
                FirstName = mainOccupant.FirstName,
                LastName = mainOccupant.LastName,
                StreetNumber = prescreen.Property.HouseNumber,
                StreetName = prescreen.Property.StreetName,
                City = prescreen.Property.City,
                State = prescreen.Property.State,
                ZipCode = prescreen.Property.ZipCode
            };

            var response = provider.Prescreen(request, ConfigurationKeys.SpruceUsername, ConfigurationKeys.SprucePassword);
            if (!String.IsNullOrWhiteSpace(provider.ErrorMessage))
            {
                var errorMessage = provider.ErrorMessage;
                var matches = Regex.Matches(provider.ErrorMessage, @":\[\""(.*?)\""\]");
                if (matches != null && matches.Count > 0) errorMessage = string.Join("\n", matches.Cast<Match>().Select(m => m.Groups[1].Value));
                throw new Exception(errorMessage);
            }

            int propertyAttributeExpiryMinutes = int.Parse(ConfigurationManager.AppSettings["InstantPrescreenExpiryMinutes"]);
            var propertyAttribute = new PropertyAttribute()
            {
                DisplayType = GetSpruceCreditCategoryString(response.Code),
                Value = "1", //should always be 1, not supposed to match the star rating
                TerritoryID = prescreen.Property.TerritoryID,
                CreatedByID = userID,
                UserID = userID,
                PropertyID = prescreen.PropertyID,
                ExpirationDate = DateTime.UtcNow.AddMinutes(propertyAttributeExpiryMinutes)
            };
            return propertyAttribute;
        }

        private string GetSpruceCreditCategoryString(string creditCategory)
        {
            if (creditCategory.Equals("A", StringComparison.InvariantCultureIgnoreCase))
            {
                return "star-1"; // pass
            }
            else if (creditCategory.Equals("D", StringComparison.InvariantCultureIgnoreCase))
            {
                return "strikethrough"; // decline
            }
            else if (creditCategory.Equals("R", StringComparison.InvariantCultureIgnoreCase))
            {
                return "review"; // the prescreen is pending, the agent should call Spruce to get more info
            }
            else // if (creditCategory.Equals("X", StringComparison.InvariantCultureIgnoreCase))
            {
                return "none";
            }
        }

        private PropertyAttribute DoExperianInstantPrescreen(PrescreenInstant prescreen, TokenExpense expense, Guid userID)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatusById(Guid prescreenInstantId, PrescreenStatus newStatus, int processedHousesCount)
        {
            using (var dc = new DataContext())
            {
                var entity = dc
                            .PrescreenInstants
                            .Include(pi => pi.Property)
                            .FirstOrDefault(pb => pb.Guid == prescreenInstantId);

                if (entity == null)
                {
                    return;
                }
                entity.Status = newStatus;

                // We processed the home, no need to refund.
                if (processedHousesCount == 1)
                {
                    dc.SaveChanges();
                    return;
                }

                var personID = entity.CreatedByID.Value;

                var ledger = tokensProvider.GetDefaultLedgerForPerson(personID);

                if (ledger == null)
                {
                    throw new ApplicationException("Invalid ledger");
                }

                //  credit back the person's ledger for the houses that did not receive stars; amount should be nagative for refunds
                var tokenExpense = dc.TokenExpenses.FirstOrDefault(te => te.InstantPrescreenID == prescreenInstantId);

                if (tokenExpense != null)
                {
                    var prescreenGuid = prescreenInstantId.ToString().ToLowerInvariant();
                    if (!dc.TokenAdjustments.Any(ta => ta.ExternalID == prescreenGuid))
                    {
                        var adjustment = new TokenAdjustment
                        {
                            Amount = tokenExpense.Amount,
                            ExternalID = prescreenInstantId.ToString().ToLowerInvariant(),
                            LedgerID = ledger.Guid,
                            Notes = "Instant prescreen refund"
                        };
                        dc.TokenAdjustments.Add(adjustment);
                    }
                }

                dc.SaveChanges();
            }
        }
    }
}
