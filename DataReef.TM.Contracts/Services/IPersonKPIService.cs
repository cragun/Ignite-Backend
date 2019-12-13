using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DataViews.SelfTrackedKPIs;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPersonKPIService : IDataService<PersonKPI>
    {
        [OperationContract]
        ICollection<SelfTrackedStatistics> GetSelfTrackedStatisticsForOrganization(Guid ouId, DateTime? specifiedDay);

        [OperationContract]
        ICollection<SelfTrackedStatistics> GetSelfTrackedStatisticsForPerson(Guid ouId, DateTime? specifiedDay);

        [OperationContract]
        ICollection<InquiryStatisticsForPerson> GetSelfTrackedStatisticsForSalesPeopleTerritories(ICollection<Guid> personIds, IEnumerable<PersonReportingSettingsItem> reportItems, DateTime? specifiedDay, IEnumerable<Guid> excludedReps = null);

        [OperationContract]
        ICollection<PersonKPI> ListKPIsFromDate(DateTime date, bool includeDeleted = false);

        [OperationContract]
        string SaveKPIScreenShot(string screenshotBase64, DateTime date);

        [OperationContract]
        void ResetKPIs(DateTime date);
    }
}