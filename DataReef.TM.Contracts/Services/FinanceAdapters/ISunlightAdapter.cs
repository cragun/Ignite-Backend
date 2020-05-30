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
        [OperationContract]
        string CreateSunlightApplicant(string fname, string lname, string email, string phone, string street, string city, string state, string zipcode);
    }
}
