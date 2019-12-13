using DataReef.TM.Models.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IAdministrationService
    {
        [OperationContract]
        ICollection<Guid> FindUser(string userNamePart);

        [OperationContract]
        ICollection<AuthenticationSummary> GetAuthenticationSummaries(DateTime? fromDate, DateTime? toDate);
    }
}
