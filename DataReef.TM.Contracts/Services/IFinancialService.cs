using DataReef.TM.Models.DTOs.Solar.Finance;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IFinancialService
    {
        [OperationContract]
        LoanResponse CalculateLoan(Guid financePlanId, LoanRequest request);

        [OperationContract]
        LoanResponse CalculateLease(LoanRequest request);

        [OperationContract]
        List<FinanceEstimate> CompareFinancePlans(FinanceEstimateComparisonRequest request);

    }
}
