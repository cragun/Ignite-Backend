using DataReef.Integrations.LoanPal.Models;
using DataReef.Integrations.LoanPal.Models.LoanPal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.ApplicationServices;

namespace DataReef.Integrations.LoanPal
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ILoanPalBridge
    {
        LoanCalculatorResponse CalculateLoan(LoanCalculatorRequest request);

        ApplicationResponse SubmitApplication(ApplicationRequest request);
    }
}
