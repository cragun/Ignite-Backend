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
using DataReef.Core.Infrastructure.Authorization;
using System.Text;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.Models.DTOs.Signatures.Proposals;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART;
using DataReef.TM.Services.Services.ProposalAddons.TriSMART.Models;
using DataReef.Engines.FinancialEngine.Loan;
using Newtonsoft.Json;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class FinancePlanDefinitionService : DataService<FinancePlanDefinition>, IFinancePlanDefinitionService
    {

        private readonly Lazy<ISunlightAdapter> _sunlightAdapter;
        private readonly Lazy<IFinancingCalculator> _loanCalculator;
        private readonly Lazy<IOUSettingService> _ouSettingService;

        public FinancePlanDefinitionService(ILogger logger,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IFinancingCalculator> loanCalculator,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<ISunlightAdapter> sunlightAdapter
            ) : base(logger, unitOfWorkFactory)
        {
            _sunlightAdapter = sunlightAdapter;
            _loanCalculator = loanCalculator;
            _ouSettingService = ouSettingService;
        }

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
            using (var dc = new DataContext())
            {
                var financePlan = dc.FinancePlaneDefinitions.FirstOrDefault(x => x.Guid == financePlanDefinitionId);
                if (financePlan != null)
                {
                    var metaData = financePlan.GetMetaData<FinancePlanDataModel>();

                    return metaData?.SBMeta?.SmartBoardCreditCheckUrls ?? new List<SmartBOARDCreditCheck>();
                }
            }

            return new List<SmartBOARDCreditCheck>();
        }

        public int GetloantermID(string planname)
        {
            var list = new List<KeyValuePair<string, int>>();
            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 2.99%", 23));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 2.99%", 4));
            list.Add(new KeyValuePair<string, int>("LoanPal 12 Year / 2.99%", 21));
            list.Add(new KeyValuePair<string, int>("LoanPal 20 Year / 2.99%", 8));
            list.Add(new KeyValuePair<string, int>("LoanPal 25 Year / 2.99%", 22));

            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 3.49%", 24));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 3.49%", 30));
            list.Add(new KeyValuePair<string, int>("LoanPal 12 Year / 3.49%", 34));
            list.Add(new KeyValuePair<string, int>("LoanPal 15 Year / 3.49%", 7));

            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 3.99%", 25));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 3.99%", 1));
            list.Add(new KeyValuePair<string, int>("LoanPal 12 Year / 3.99%", 15));
            list.Add(new KeyValuePair<string, int>("LoanPal 15 Year / 3.99%", 36));
            list.Add(new KeyValuePair<string, int>("LoanPal 20 Year / 3.99%", 3));
            list.Add(new KeyValuePair<string, int>("LoanPal 25 Year / 3.99%", 9));

            list.Add(new KeyValuePair<string, int>("LoanPal 12 Year / 4.49%", 35));
            list.Add(new KeyValuePair<string, int>("LoanPal 15 Year / 4.49%", 37));
            list.Add(new KeyValuePair<string, int>("LoanPal 20 Year / 4.49%", 39));
            list.Add(new KeyValuePair<string, int>("LoanPal 25 Year / 4.49%", 10));


            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 4.99%", 26));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 4.99%", 5));
            list.Add(new KeyValuePair<string, int>("LoanPal 15 Year / 4.99%", 38));
            list.Add(new KeyValuePair<string, int>("LoanPal 20 Year / 4.99%", 2));
            list.Add(new KeyValuePair<string, int>("LoanPal 25 Year / 4.99%", 11));

            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 5.99%", 27));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 5.99%", 31));
            list.Add(new KeyValuePair<string, int>("LoanPal 20 Year / 5.99%", 6));
            list.Add(new KeyValuePair<string, int>("LoanPal 25 Year / 5.99%", 12));

            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 6.99%", 28));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 6.99%", 32));
            list.Add(new KeyValuePair<string, int>("LoanPal 25 Year / 6.99%", 13));

            list.Add(new KeyValuePair<string, int>("LoanPal 7 Year / 7.99%", 29));
            list.Add(new KeyValuePair<string, int>("LoanPal 10 Year / 7.99%", 33));


            int id = list.Where(x => x.Key == planname).FirstOrDefault().Value;

            return id;
        }


        public SunlightResponse GetSunlightloanstatus(Guid proposalId)
        {
            return _sunlightAdapter.Value.GetSunlightloanstatus(proposalId);
        }

        public SunlightResponse Sunlightsendloandocs(Guid proposalId)
        {
            return _sunlightAdapter.Value.Sunlightsendloandocs(proposalId);
        }

        public void UpdateCashPPW(double? cashPPW, double? lenderFee)
        {
            using (var dc = new DataContext())
            {
                if (cashPPW != null)
                {

                    //var financePlan = dc.FinancePlaneDefinitions.FirstOrDefault(x => x.Name == "Cash");
                    //if (financePlan != null)
                    //{
                    //    financePlan.PPW = cashPPW;
                    //    dc.SaveChanges();
                    //}
                    dc.FinancePlaneDefinitions.ToList().ForEach(c => c.PPW = cashPPW);
                    dc.SaveChanges();
                }

                if (lenderFee != null)
                {
                    //dc.FinancePlaneDefinitions.Where(y => !y.Name.Contains("Cash") && !y.Name.Contains("Sunnova")).ToList().ForEach(c => c.LenderFee = lenderFee);
                    dc.FinancePlaneDefinitions.Where(y => !y.Name.Contains("Sunnova")).ToList().ForEach(c => c.LenderFee = lenderFee);
                    dc.SaveChanges();
                }

            }
        }

        public IEnumerable<SmartBOARDCreditCheck> GetCreditCheckUrlForFinancePlanDefinitionAndPropertyID(Guid financePlanDefinitionId, Guid propertyID)
        {
            using (var dc = new DataContext())
            {
                var financePlan = dc.FinancePlaneDefinitions.FirstOrDefault(x => x.Guid == financePlanDefinitionId);

                if (financePlan != null)
                {
                    var property = dc.Properties.Include("PropertyBag").FirstOrDefault(x => x.Guid == propertyID && !x.IsDeleted);
                    var salesperson = dc.People.FirstOrDefault(x => x.Guid == SmartPrincipal.UserId && !x.IsDeleted);
                    if (property != null)
                    {
                        var metaData = financePlan.GetMetaData<FinancePlanDataModel>();

                        var creditCheckUrls = metaData?.SBMeta?.SmartBoardCreditCheckUrls ?? new List<SmartBOARDCreditCheck>();
                        foreach (var url in creditCheckUrls)
                        {
                            if (url.UseSMARTBoardAuthentication && url.CreditCheckUrl.Contains("{smartBoardID}"))
                            {
                                url.CreditCheckUrl = url.CreditCheckUrl.Replace("{smartBoardID}", property.SmartBoardId.ToString() ?? string.Empty);
                            }

                            if (url.CreditCheckUrl.Contains("{loanpaldata}"))
                            {
                                string loanpalurl = "??lname=" + property.GetMainOccupant().LastName + "&fname=" + property.GetMainOccupant().FirstName + "&street=" + property.Address1 + "&city=" + property.City + "&state=" + property.State + "&zip=" + property.ZipCode + "&email=" + property.GetMainEmailAddress() + "&phone=" + property.GetMainPhoneNumber()?.Replace("-", "") + "&srfn=" + salesperson.FirstName + "&srln=" + salesperson.LastName + "&sre=" + salesperson.EmailAddressString + "&loanterm=" + GetloantermID(financePlan.Name).ToString()
                                    //+ "&cost=" + property.Name + "&refnum=" + property.Name 
                                    + "&language=english";
                                loanpalurl = loanpalurl.Replace(" ", "%20");
                                url.CreditCheckUrl = url.CreditCheckUrl.Replace("{loanpaldata}", loanpalurl ?? string.Empty);
                            }

                            if (url.CreditCheckUrl.Contains("{sunlightdata}"))
                            {
                                string sunlighturl = _sunlightAdapter.Value.CreateSunlightAccount(property, financePlan);
                                url.CreditCheckUrl = url.CreditCheckUrl.Replace("{sunlightdata}", sunlighturl ?? string.Empty);
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
