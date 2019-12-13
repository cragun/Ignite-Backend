using DataReef.TM.Models.DTOs.Reports;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IReportingServices
    {
        [OperationContract]
        ICollection<OrganizationReportRow> GetOrganizationReport(Guid startOUID, DateTime? specifiedDay = null);

        [OperationContract]
        ICollection<SalesRepresentativeReportRow> GetSalesRepresentativeReport(Guid startOUID, DateTime? specifiedDay = null);

        [OperationContract]
        ICollection<OrganizationSelfTrackedReportRow> GetOrganizationSelfTrackedReport(Guid startOUID, DateTime? specifiedDay);

        [OperationContract]
        ICollection<SalesRepresentativeSelfTrackedReportRow> GetSalesRepresentativeSelfTrackedReport(Guid startOUID, DateTime? specifiedDay);
    }
}
