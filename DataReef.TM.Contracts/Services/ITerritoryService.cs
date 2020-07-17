using DataReef.Core.Classes;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.Reporting.Settings;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ITerritoryService : IDataService<Territory>
    {
        [OperationContract]
        Task<TerritorySummary> PopulateTerritorySummary(Territory territory, bool includePropertyCount = true);

        [OperationContract]
        ICollection<Territory> GetForCurrentUserAndOU(Guid ouID, Guid personID, string include = "", string exclude = "", string fields = "");

        [OperationContract]
        int GetPropertiesCount(Territory territory);

        [OperationContract]
        long GetPropertiesCountForWKT(string wkt);

        [OperationContract]
        SaveResult IsNameAvailable(Guid ouId, string name);

        [OperationContract]
        ICollection<Territory> GetByShapesVersion(Guid ouid, Guid? personID, ICollection<TerritoryShapeVersion> territoryShapeVersions, bool deletedItems = false, string include = "");

        [OperationContract]
        Territory SetArchiveStatus(Guid territoryID, bool archiveStatus);

        [OperationContract]
        ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForTerritory(Guid territoryId, OUReportingSettings reportSettings, DateTime? specifiedDay);

        [OperationContract]
        FavouriteTerritory InsertFavoriteTerritory(Guid territoryId, Guid personID);

        [OperationContract]
        void RemoveFavoriteTerritory(Guid territoryId, Guid personID);
    }
}