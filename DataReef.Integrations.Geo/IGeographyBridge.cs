using DataReef.Integrations.Geo.DataViews;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.ApplicationServices;
using DataReef.Integrations.Common.Geo;

namespace DataReef.Integrations
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IGeographyBridge
    {
        [OperationContract]
        ICollection<Property> GetPropertiesForWellKnownText(string wkt, int? requestSize = null, int? requestPage = null, List<string> exludedLocationIds = null);

        [OperationContract]
        ICollection<Property> GetProperties(List<PropertiesRequest> propertiesRequests);
            
        [OperationContract]
        List<Shape> GetShapesForStates(List<string> states);

        [OperationContract]
        HighResImage GetHiResImageById(Guid id);

        [OperationContract]
        HighResImage GetHiResImageByLatLon(double lat, double lon);

        [OperationContract]
        void SaveHiResImage(HighResImage image);

        [OperationContract]
        void MigrateHiResImage(HighResImage image);

        [OperationContract]
        void InsertShapes(List<Shape> shapes);

        [OperationContract]
        string SaveHiResImagetest(HighResImage image);
    }
}
