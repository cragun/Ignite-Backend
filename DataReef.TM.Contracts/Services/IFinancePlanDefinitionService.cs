﻿using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Financing;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IFinancePlanDefinitionService : IDataService<FinancePlanDefinition>
    {
        [OperationContract]
        ICollection<FinancePlanDefinition> GetPlans(Guid ouid);

        [OperationContract]
        ICollection<FinancePlanDefinition> GetPlansForRequest(FinanceEstimateComparisonRequest req);

        [OperationContract]
        IEnumerable<SmartBOARDCreditCheck> GetCreditCheckUrlForFinancePlanDefinition(Guid financePlanDefinitionId);

        [OperationContract]
        IEnumerable<SmartBOARDCreditCheck> GetCreditCheckUrlForFinancePlanDefinitionAndPropertyID(Guid financePlanDefinitionId, Guid propertyID);

        [OperationContract]
        string GetSunlightloanstatus(Guid proposalId);

        [OperationContract]
        string Sunlightsendloandocs(Guid proposalId);

    }
}