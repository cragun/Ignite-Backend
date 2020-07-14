using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services.FinanceAdapters
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ISunlightAdapter
    {
        //    [OperationContract]
        //    string CreateSunlightApplicant(string fname, string lname, string email, string phone, string street, string city, string state, string zipcode);

        [OperationContract]
        string CreateSunlightAccount(Property property, FinancePlanDefinition financePlan);
        //string CreateSunlightAccount(Property property, FinancePlanDefinition financePlan, string loanAmount);

        [OperationContract]
        string GetSunlightToken();

        [OperationContract]
        string GetState(string shortState, string type);

        [OperationContract]
        SunlightResponse GetSunlightloanstatus(Guid proposal);

        [OperationContract]
        SunlightResponse Sunlightsendloandocs(Guid proposal);
    }
}
