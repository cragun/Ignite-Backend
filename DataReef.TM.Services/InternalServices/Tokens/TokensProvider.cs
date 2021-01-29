using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.DataViews.Ledgers;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TokensProvider : ITokensProvider
    {
        public async Task<double> GetBalanceForLedger(Guid ledgerID)
        {
            using (var dc = new DataContext())
            {
                var ret = await dc.Database.SqlQuery<int>(
                        "SELECT Balance FROM [dbo].[TokenBalances]() WHERE LedgerID = @LedgerID",
                        new SqlParameter("LedgerID", ledgerID.ToString())).FirstOrDefaultAsync();

                return ret;
            }
        }

        public async Task<TokenLedger> GetDefaultLedgerForPerson(Guid personID)
        {
            using (DataContext dc = new DataContext())
            {
                TokenLedger ret = await dc.TokenLedgers
                    .Include(tl => tl.Purchases)
                    .Include(tl => tl.TransfersIn)
                    .Include(tl => tl.Expenses)
                    .Include(tl => tl.TransfersOut)
                    .Include(tl => tl.Adjustments).AsNoTracking()
                    .FirstOrDefaultAsync(tl => tl.PersonID == personID && tl.IsDeleted == false && tl.IsPrimary == true);
                return ret;
            }
        }

        public async Task<LedgerDataView> GetLedgerDataViewForPerson(Guid personID)
        {
              
                TokenLedger ledger = await this.GetDefaultLedgerForPerson(personID);

                if (ledger == null)
                {
                    return null;
                }

                LedgerDataView ret = new LedgerDataView();
                ret.PersonID = personID;
                ret.Name = ledger.Name;
                ret.Balance = await this.GetBalanceForLedger(ledger.Guid);
                ret.LedgerItems = new List<LedgerItemDataView>();

                foreach (TokenPurchase p in ledger.Purchases)
                {
                    ret.LedgerItems.Add(
                        new LedgerItemDataView()
                        {
                            Amount = p.Amount,
                            Date = p.DateCreated,
                            Reference = p.Reference,
                            Type = LedgerItemType.Purchase
                        }
                        );
                }

                foreach (TokenExpense p in ledger.Expenses)
                {
                    ret.LedgerItems.Add(
                        new LedgerItemDataView()
                        {
                            Amount = p.Amount,
                            Date = p.DateCreated,
                            Reference = p.Notes,
                            Type = LedgerItemType.Expense
                        }
                        );
                }

                foreach (TokenAdjustment p in ledger.Adjustments)
                {
                    ret.LedgerItems.Add(
                        new LedgerItemDataView()
                        {
                            Amount = p.Amount,
                            Date = p.DateCreated,
                            Reference = p.Notes,
                            Type = LedgerItemType.Adjustment
                        }
                        );
                }

                foreach (TokenTransfer p in ledger.TransfersIn)
                {
                    ret.LedgerItems.Add(
                        new LedgerItemDataView()
                        {
                            Amount = p.Amount,
                            Date = p.DateCreated,
                            Reference = p.Notes,
                            Type = LedgerItemType.TransferIn
                        }
                        );
                }

                foreach (TokenTransfer p in ledger.TransfersOut)
                {
                    ret.LedgerItems.Add(
                        new LedgerItemDataView()
                        {
                            Amount = p.Amount,
                            Date = p.DateCreated,
                            Reference = p.Notes,
                            Type = LedgerItemType.TransferOut
                        }
                        );
                }

                ret.LedgerItems = ret.LedgerItems.OrderBy(li => li.Date).ToList();

                return ret;
            
        }

        public void PerformTransfers(IEnumerable<TransferDataCommand> transfers)
        {
            foreach (TransferDataCommand tdc in transfers)
            {
                this.PerformTransfer(tdc);
            }
        }

        private TokenLedger CreateTokenLedgerForPerson(Guid personID)
        {
            try
            {
                TokenLedger ret = null;

                using (DataContext dc = new DataContext())
                {
                    ret = new TokenLedger();
                    ret.PersonID = personID;
                    ret.UserID = personID;
                    ret.IsPrimary = true;
                    ret.Name = "Default Ledger";
                    dc.TokenLedgers.Add(ret);
                    dc.SaveChanges();
                }

                return ret;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void PerformTransfer(TransferDataCommand transfer)
        {
            using (DataContext dc = new DataContext())
            {
                Guid fromPersonID = SmartPrincipal.UserId;
                Person fromPerson = dc.People.Where(pp => pp.Guid == fromPersonID).FirstOrDefault();
                if (fromPerson == null)
                {
                    transfer.SaveResult = SaveResult.FromException(new ApplicationException("From Person Not Found"), DataAction.Insert);
                    return;
                }

                TokenLedger fromLedger = this.GetDefaultLedgerForPerson(fromPersonID).Result;
                if (fromLedger == null)
                {
                    transfer.SaveResult = SaveResult.FromException(new ApplicationException("Ledger not found (From)"), DataAction.Insert);
                    return;
                }

                double balance = this.GetBalanceForLedger(fromLedger.Guid).Result;
                if (balance < transfer.Amount)
                {
                    transfer.SaveResult = SaveResult.FromException(new ApplicationException("Insufficient Funds To Transfer"), DataAction.Insert);
                    return;
                }

                TokenLedger toLedger = this.GetDefaultLedgerForPerson(transfer.ToPersonID).Result;
                try
                {
                    if (toLedger == null) toLedger = this.CreateTokenLedgerForPerson(transfer.ToPersonID);
                }
                catch (Exception ex)
                {
                    transfer.SaveResult = SaveResult.FromException(ex, DataAction.Insert);
                    return;
                }

                if (toLedger == null)
                {
                    transfer.SaveResult = SaveResult.FromException(new ApplicationException("Error Creating To Ledger"), DataAction.Insert);
                    return;
                }

                // --------------- Create Transfer

                TokenTransfer tt = new TokenTransfer();
                tt.FromLedgerID = fromLedger.Guid;
                tt.ToLedgerID = toLedger.Guid;
                tt.Amount = transfer.Amount;
                tt.Notes = string.Format("Transfer From {0} {1}", fromPerson.FirstName, fromPerson.LastName);
                dc.TokenTransfers.Add(tt);

                try
                {
                    dc.SaveChanges();
                    transfer.SaveResult = SaveResult.SuccessfulInsert;
                }
                catch (Exception ex)
                {
                    transfer.SaveResult = SaveResult.FromException(ex, DataAction.Insert);
                }
            }
        }

    }
}
