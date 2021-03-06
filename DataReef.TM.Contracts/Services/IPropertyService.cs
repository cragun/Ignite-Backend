using DataReef.TM.Models.DataViews;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using DataReef.TM.Models.DTOs;
using Property = DataReef.TM.Models.Property;
using DataReef.TM.Models.Solar;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.SmartBoard;
using System.Threading.Tasks;
using DataReef.TM.Models;
using DataReef.TM.Models.DTOs.Solar.Finance;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IPropertyService : IDataService<Property>
    {
        [OperationContract]
        ICollection<Property> GetOUPropertiesByStatusPaged(Guid ouID, string disposition, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "");

        [OperationContract]
        ICollection<Property> GetOUPropertiesPaged(Guid ouID, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "");

        [OperationContract]
        ICollection<Property> GetOUPropertiesWithProposal(Guid ouID, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "");

        [OperationContract]
        ICollection<Property> GetTerritoryPropertiesByStatusPaged(Guid territoryID, string disposition, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "");

        [OperationContract]
        ICollection<Property> GetTerritoryPropertiesWithProposal(Guid territoryID, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "");

        [OperationContract]
        ICollection<CustomerLite> ListCustomersLite(Guid ouID, bool deep, int pageNumber = 0, DateTime? startDate = null, DateTime? endDate = null);

        [OperationContract]
        ICollection<Property> GetProperties(GetPropertiesRequest propertiesRequest);


        [OperationContract]
        ICollection<Property> GetPropertiesSearch(Guid territoryid, string searchvalue);

        [OperationContract]
        void SyncPrescreenBatchPropertiesAttributes(Guid prescreenBatchId);

        [OperationContract]
        void SyncInstantPrescreenPropertyAttributes(Guid prescreenInstantId);

        [OperationContract]
        Property SyncProperty(Guid propertyID, string include = "");

        [OperationContract]
        Task<Property> PropertyBagsbyID(Guid propertyID);

        [OperationContract]
        SolarTariff GetTariffByGenabilityProviderAccountID(string id);

        [OperationContract]
        ICollection<Integrations.Common.Geo.Property> GetPropertiesInShape(string wkt, int maxResults = 10000);

        [OperationContract]
        CanCreateAppointmentResponse CanAddAppointmentOnProperty(CanCreateAppointmentRequest request);

        [OperationContract]
        SBPropertyDTO CreatePropertyFromSmartBoard(SBCreatePropertyRequest request, string apiKey);

        [OperationContract]
        Task<IEnumerable<Territories>> GetTerritoriesList(Guid propertyid, string apiKey);

        [OperationContract]
        Task<IEnumerable<Territories>> GetTerritoryListbyApikey(string apiKey, double Lat, double Long);

        [OperationContract]
        Task<IEnumerable<Territories>> GetTerritoryListbyApikeyMaster(string apiKey, double Lat, double Long);

        [OperationContract]
        Task<IEnumerable<TerritoryApikey>> TerritoryNApikey(double Lat, double Long);

        [OperationContract]
        SBPropertyDTO EditPropertyNameFromSB(long igniteID, SBPropertyNameDTO Request);

        [OperationContract]
        Task<EsIDResponse> GetEsidByAddress(Guid propertyid);

        [OperationContract]
        Task<bool> IsPropertyAvailable(long igniteId);

        [OperationContract]
        List<Territory> GetTerritoriesFromAddress(Property req);
    }
}
