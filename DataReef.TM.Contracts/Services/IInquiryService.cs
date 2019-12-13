using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IInquiryService : IDataService<Inquiry>
    {
        [OperationContract]
        ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForOrganizationTerritories(ICollection<Guid> territoryIds, IEnumerable<OUReportingSettingsItem> reportItems, DateTime? specifiedDay = null, IEnumerable<Guid> excludedReps = null);

        [OperationContract]
        ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForSalesPeopleTerritories(ICollection<Guid> territoryIds, IEnumerable<PersonReportingSettingsItem> reportItems, DateTime? specifiedDay = null, IEnumerable<Guid> excludedReps = null);

        [OperationContract]
        ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForPerson(Guid personId, ICollection<string> dispositions, DateTime? specifiedDay);

        [OperationContract]
        InquiryStatisticsByDate GetWorkingRepsDisposition(IEnumerable<Guid> repIds, IEnumerable<string> dispositions, DateTime? specifiedDay);

        [OperationContract]
        string GetLatestPropertyDisposition(Guid propertyID, Guid? skipInquiryId = null);

        [OperationContract]
        bool IsInquiryFirstForUser(Guid inquiryId, Guid userId);
    }
}