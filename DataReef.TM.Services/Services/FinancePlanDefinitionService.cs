using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.Solar;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using DataReef.Core.Infrastructure.Repository;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.DataViews.Financing;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class FinancePlanDefinitionService : DataService<FinancePlanDefinition>, IFinancePlanDefinitionService
    {
        public FinancePlanDefinitionService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory) { }

        public override ICollection<FinancePlanDefinition> GetMany(IEnumerable<Guid> uniqueIds, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            var result = base
                            .GetMany(uniqueIds, include, exclude, fields, deletedItems)
                            .ToList();

            // If the Cash PlanDefinition GUID was not sent, we'll retrieve the Cash from DB, and append it to the results.
            if (!result.Any(r => r.Name == "Cash"))
            {
                var cashPlanDefinition = base.List(filter: "Name=Cash", include: include, exclude: exclude, fields: fields);
                if (cashPlanDefinition != null)
                {
                    result.AddRange(cashPlanDefinition);
                }
            }
            return result;
        }

        public ICollection<FinancePlanDefinition> GetPlansForRequest(FinanceEstimateComparisonRequest req)
        {
            if (!req.SortAndFilterResponse)
            {
                return GetMany(req.FinancePlanGuids, "Details");
            }
            else
            {
                return base.GetMany(req.FinancePlanGuids, "Details");
            }
        }

        public ICollection<FinancePlanDefinition> GetPlans(Guid ouid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SmartBOARDCreditCheck> GetCreditCheckUrlForFinancePlanDefinition(Guid financePlanDefinitionId)
        {
            using(var dc = new DataContext())
            {
                var financePlan = dc.FinancePlaneDefinitions.FirstOrDefault(x => x.Guid == financePlanDefinitionId);
                if(financePlan != null)
                {
                    var metaData = financePlan.GetMetaData<FinancePlanDataModel>();

                    return metaData?.SBMeta?.SmartBoardCreditCheckUrls ?? new List<SmartBOARDCreditCheck>();
                }
            }

            return new List<SmartBOARDCreditCheck>();
        }

        public IEnumerable<SmartBOARDCreditCheck> GetCreditCheckUrlForFinancePlanDefinitionAndPropertyID(Guid financePlanDefinitionId, Guid propertyID)
        {
            using (var dc = new DataContext())
            {
                var financePlan = dc.FinancePlaneDefinitions.FirstOrDefault(x => x.Guid == financePlanDefinitionId);
                if (financePlan != null)
                {
                    var property = dc.Properties.FirstOrDefault(x => x.Guid == propertyID && !x.IsDeleted);
                    if(property != null)
                    {
                        var metaData = financePlan.GetMetaData<FinancePlanDataModel>();

                        var creditCheckUrls = metaData?.SBMeta?.SmartBoardCreditCheckUrls ?? new List<SmartBOARDCreditCheck>();
                        foreach(var url in creditCheckUrls)
                        {
                            if (url.UseSMARTBoardAuthentication && url.CreditCheckUrl.Contains("{smartBoardID}"))
                            {
                                url.CreditCheckUrl = url.CreditCheckUrl.Replace("{smartBoardID}", property.SmartBoardId.ToString() ?? string.Empty);
                            }

                            if (url.CreditCheckUrl.Contains("{smartBoardID}"))
                            {
                                url.CreditCheckUrl = url.CreditCheckUrl.Replace("{smartBoardID}", property.SmartBoardId.ToString() ?? string.Empty);
                            }
                        }

                        return creditCheckUrls;
                    }
                    
                }
            }

            return new List<SmartBOARDCreditCheck>();
        }
    }
}
