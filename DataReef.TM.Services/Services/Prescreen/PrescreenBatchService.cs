using DataReef.Core.Classes;
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
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Integrations.SolarCloud;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PrescreenBatchService : DataService<PrescreenBatch>, IPrescreenBatchService
    {
        private readonly ITokensProvider tokensProvider;
        private readonly IDataService<TokenLedger> ledgerService;
        private readonly IDataService<TokenExpense> expenseService;
        private readonly IGeoProvider geoProvider;
        private readonly ITerritoryService territoryService;
        private readonly Lazy<ICloudBridge> _cloudBridge;

        public PrescreenBatchService(ILogger logger,
                ITokensProvider tokensProvider,
                IDataService<TokenLedger> ledgerService,
                IDataService<TokenExpense> expenseService,
                IGeoProvider geoProvider,
                ITerritoryService territoryService,
                Func<IUnitOfWork> unitOfWorkFactory,
                Lazy<ICloudBridge> cloudBridge
            )
            : base(logger, unitOfWorkFactory)
        {
            this.tokensProvider = tokensProvider;
            this.ledgerService = ledgerService;
            this.territoryService = territoryService;
            this.expenseService = expenseService;

            //todo: we should use Unity or some other framework to inject the dependencies here
            this.geoProvider = new DataReefGeoProvider();
            _cloudBridge = cloudBridge;
        }

        public override PrescreenBatch Update(PrescreenBatch entity)
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

        public override PrescreenBatch Insert(PrescreenBatch entity)
        {
            //if ledger == null then the user has no credits available, no ledger=no credits
            if (entity == null || entity.TerritoryID == null)
            {
                throw new ApplicationException("Invalid Territory");
            }

            //we need to see if the person even has a balance, if they dont, they cant create prescreen batches.
            //next we need to find out how many homes are in the shape, if they have enough credits, we can start the batch process and move the process to a different
            //thread so the client can return

            //get the guid of the person logged in ( provided by our Auth framework)
            Guid userID = SmartPrincipal.UserId;
            entity.PersonID = userID;

            //check to see if they have any balance first
            TokenLedger ledger = Task.Run(() => tokensProvider.GetDefaultLedgerForPerson(userID)).Result;

            //if ledger == null then the user has no credits available, no ledger=no credits
            if (ledger == null)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            double balance = Task.Run(() => tokensProvider.GetBalanceForLedger(ledger.Guid)).Result;
            if (balance <= 0)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            //next we need to get the WKT associated with the Territory
            Territory territory = Task.Run(() => this.territoryService.Get(entity.TerritoryID, "Shapes")).Result;

            if (territory == null)
            {
                throw new ApplicationException("Invalid Territory GUID");
            }
            else if (string.IsNullOrWhiteSpace(territory.WellKnownText))
            {
                throw new ApplicationException("The territory does not have Well Known Text ( no shape associated with the territory) ");
            }

            //Ok, we have a valid territory and we know they have some balance, so lets now go to the geo service and find out how many possible homes exist
            int houseCount = territoryService.GetPropertiesCount(territory);
            if (houseCount == 0)
            {
                throw new ApplicationException("There are no homes associated with this territory.  Nothing to prescreen ");
            }

            //check to see if they have enough credits
            if (houseCount > balance)
            {
                throw new ApplicationException("Insufficient Credits Available");
            }

            //insert a new prescreen batch entry into the database; this will be updated with the prescreen results
            PrescreenBatch prescreenBatch = base.Insert(entity);

            //ok, we have reached this we can start to run the prescreens
            var hostNotifier = new HostNotifier();
            Task.Run(() =>
            {
                hostNotifier.Begin();
                using (DataContext dc = new DataContext())
                {
                    PrescreenBatch batch = null;
                    TokenExpense expense = null;
                    try
                    {
                        //get a copy connected to the Data Context so we can update its status
                        batch = dc.PrescreenBatches.Where(bb => bb.Guid == prescreenBatch.Guid).FirstOrDefault();
                        batch.Status = PrescreenStatus.Pending;

                        //now create the accounting 
                        //first create an expense for ALL the houses available. Leter we credit back the unused portionn
                        expense = new TokenExpense
                        {
                            Amount = houseCount,
                            CreatedByID = userID,
                            DateCreated = System.DateTime.UtcNow,
                            LedgerID = ledger.Guid,
                            TenantID = SmartPrincipal.TenantId,
                            BatchPrescreenID = prescreenBatch.Guid,
                            Notes = "Batch Preauthorization",
                            TagString = "preauth"

                        };
                        dc.TokenExpenses.Add(expense);

                        //save the changes to db.. 
                        dc.SaveChanges();

                        //ok, now that we have created the batch.  We just need to add all the houses into the the Workflow

                        Guid ret = _cloudBridge.Value.InitiateWorkflowForWKT(territory.WellKnownText, territory.Guid, prescreenBatch.Guid);

                        batch.ExternalID = ret.ToString();
                        //all done, update status and save                        
                        batch.CompletionDate = DateTime.UtcNow;

                        //  if ret is Guid.Empty then refund all tokens to user and set status to error
                        if (ret == Guid.Empty)
                        {
                            batch.Status = PrescreenStatus.Error;
                            var adjustment = new TokenAdjustment
                            {
                                Amount = -houseCount,
                                ExternalID = batch.Guid.ToString(),
                                LedgerID = ledger.Guid,
                                Notes = "Batch prescreen refund, error starting workflow"
                            };
                            dc.TokenAdjustments.Add(adjustment);
                        }
                        else
                        {
                            batch.Status = PrescreenStatus.InProgress; //the workflow process will update the status    
                        }

                        dc.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        if (batch != null)
                        {
                            batch.Status = PrescreenStatus.Error;
                            batch.ErrorString = ex.Message + ":" + ex.StackTrace;
                            batch.CompletionDate = System.DateTime.UtcNow;
                            if (expenseService != null)
                            {
                                expense.Amount = 0;
                            }
                        }
                        dc.SaveChanges();
                    }
                }
                hostNotifier.Finish();
            });
            return prescreenBatch;
        }

        public static string GetCreditCategoryString(int creditCategory)
        {
            if (creditCategory > 0)
            {
                return string.Format("star-{0}", creditCategory);
            }
            else if (creditCategory == -1)
            {
                return "strikethrough";
            }
            else // if (creditCategory == 0)
            {
                return "none";
            }
        }

        public void UpdateStatus(string externalId, PrescreenStatus newStatus)
        {
            using (var dc = new DataContext())
            {
                var entity = dc
                            .PrescreenBatches
                            .FirstOrDefault(pb => pb.ExternalID == externalId);

                if (entity == null)
                {
                    return;
                }
                entity.Status = newStatus;
                dc.SaveChanges();
            }
        }

        public void UpdateStatusById(Guid prescreenBatchId, PrescreenStatus newStatus, int processedHousesCount)
        {
            using (var dc = new DataContext())
            {
                var entity = dc
                            .PrescreenBatches
                            .FirstOrDefault(pb => pb.Guid == prescreenBatchId);

                if (entity == null)
                {
                    return;
                }
                entity.Status = newStatus;

                var personID = entity.PersonID;
                var territory = Task.Run(() => territoryService.Get(entity.TerritoryID)).Result;
                if (territory == null)
                {
                    throw new ApplicationException("Invalid territory");
                }
                var ledger = Task.Run(() => tokensProvider.GetDefaultLedgerForPerson(personID)).Result;
                if (ledger == null)
                {
                    throw new ApplicationException("Invalid ledger");
                }

                //  credit back the person's ledger for the houses that did not receive stars; amount should be nagative for refunds
                var tokenExpense = dc.TokenExpenses.FirstOrDefault(te => te.BatchPrescreenID == prescreenBatchId);
                if (tokenExpense != null)
                {
                    var prescreenGuid = prescreenBatchId.ToString().ToLowerInvariant();
                    if (!dc.TokenAdjustments.Any(ta => ta.ExternalID == prescreenGuid))
                    {
                        int houseCount = territoryService.GetPropertiesCount(territory);
                        var amountToRefund = processedHousesCount - houseCount;
                        if (-amountToRefund < tokenExpense.Amount)
                        {
                            var adjustment = new TokenAdjustment
                            {
                                Amount = processedHousesCount - houseCount,
                                ExternalID = prescreenBatchId.ToString().ToLowerInvariant(),
                                LedgerID = ledger.Guid,
                                Notes = "Batch prescreen refund"
                            };
                            dc.TokenAdjustments.Add(adjustment);
                        }
                    }
                }

                dc.SaveChanges();
            }
        }
    }
}
