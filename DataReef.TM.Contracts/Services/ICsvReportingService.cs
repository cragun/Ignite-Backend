using DataReef.TM.Models;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface ICsvReportingService
    {
        [OperationContract]
        byte[] GenerateOrganizationSelfTrackedReport(Guid startOUID, DateTime? specifiedDay, ReportingPeriod reportingPeriod);

        [OperationContract]
        byte[] GenerateSalesRepSelfTrackedReport(Guid ouID, DateTime? specifiedDay, ReportingPeriod reportingPeriod);

        [OperationContract]
        byte[] GeneratePropertyCsvReport(string wkt);
    }
}
