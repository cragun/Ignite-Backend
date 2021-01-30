using DataReef.Core.Attributes;
using DataReef.TM.Contracts.DataViews.Ledgers;
using DataReef.TM.Models.Accounting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
     [ServiceContract]
    public interface ITokensProvider
    {
        [OperationContract]
        Task<double> GetBalanceForLedger(Guid ledgerID);

        [OperationContract]
        Task<TokenLedger> GetDefaultLedgerForPerson(Guid personID);

        [OperationContract]
        Task<LedgerDataView> GetLedgerDataViewForPerson(Guid personID);

        [OperationContract]
        void PerformTransfers(IEnumerable<TransferDataCommand> transfers);

    }
}
