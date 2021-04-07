using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.FinancialIntegration.LoanPal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPropertyNotesAdapter
    {
        [OperationContract]
        NoteResponse GetLeadReferenceId(Property property, string apikey);
         
    }
}
