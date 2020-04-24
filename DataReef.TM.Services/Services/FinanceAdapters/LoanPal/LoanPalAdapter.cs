using DataReef.Integrations.LoanPal;
using DataReef.TM.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReef.Integrations.LoanPal.Models.LoanPal;
using AutoMapper;
using DataReef.TM.Models.FinancialIntegration.LoanPal;
using DataReef.Core.Attributes;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.TM.Services.Services;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [Service(typeof(ILoanPalAdapter))]
    public class LoanPalAdapter : FinancialAdapterBase, ILoanPalAdapter
    {
        private readonly Lazy<ILoanPalBridge> _bridge;

        public LoanPalAdapter(Lazy<IOUSettingService> ouSettingService, Lazy<ILoanPalBridge> bridge) : base("LoanPal", ouSettingService)
        {
            _bridge = bridge;
        }

        public override TokenResponse AuthorizeAdapter(AuthenticationContext authenticationContext)
        {
            throw new NotImplementedException();
        }

        public override AuthenticationContext GetAuthenticationContext(Guid ouid)
        {
            throw new NotImplementedException();
        }

        public override string GetBaseUrl(Guid ouid)
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, string> GetCustomHeaders(TokenResponse tokenResponse)
        {
            throw new NotImplementedException();
        }

        public LoanPalApplicationResponse SubmitApplication(LoanPalApplicationRequest request)
        {
            var req = Mapper.Map<ApplicationRequest>(request);
            var response = _bridge.Value.SubmitApplication(req);

            SaveRequest(req, response, null, null, null);

            return Mapper.Map<LoanPalApplicationResponse>(response);
        }
    }
}
