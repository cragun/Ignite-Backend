using DataReef.TM.Models.DTOs.Reports;
using DataReef.TM.Models.Enums;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    public interface IReportingServices
    {
        [OperationContract]
        ICollection<OrganizationReportRow> GetOrganizationReport(Guid startOUID, DateTime? specifiedDay = null, DateTime? StartRangeDay = null, DateTime? EndRangeDay = null);

        [OperationContract]
        ICollection<SalesRepresentativeReportRow> GetSalesRepresentativeReport(Guid startOUID, DateTime? specifiedDay = null, DateTime? StartRangeDay = null, DateTime? EndRangeDay = null, string proptype = null);

        [OperationContract]
        Task<ICollection<OrganizationSelfTrackedReportRow>> GetOrganizationSelfTrackedReport(Guid startOUID, DateTime? specifiedDay);

        [OperationContract]
        ICollection<SalesRepresentativeSelfTrackedReportRow> GetSalesRepresentativeSelfTrackedReport(Guid startOUID, DateTime? specifiedDay);
    }
}
