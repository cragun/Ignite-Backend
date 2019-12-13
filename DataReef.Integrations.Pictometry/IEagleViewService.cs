using System.Collections.Generic;
using System.Drawing;
using System.ServiceModel;

namespace DataReef.Integrations.Pictometry
{
    [ServiceContract]
    public interface IEagleViewService
    {
        [OperationContract]
        List<SearchResult> GetLinksForLatLon(double lat, double lon);

        [OperationContract]
        Image GetImageForSearchResult(SearchResult result, int width, int height);

        [OperationContract]
        void PopulateMetaDataForSearchResult(SearchResult result);

        [OperationContract]
        byte[] GetImageBytesForSearchResult(SearchResult result, int width, int height);
    }
}