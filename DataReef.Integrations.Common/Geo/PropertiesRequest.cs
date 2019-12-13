using System.Collections.Generic;

namespace DataReef.Integrations.Common.Geo
{
    public class PropertiesRequest
    {
        
        public string AreaWellKnownText { get; set; }
        public List<string> AreaWellKnownTextArray { get; set; }
        public AreaViewBounds AreaViewBounds { get; set; }
        public string StreetName { get; set; }
        public List<string> ExcludedLocationIDs { get; set; }
        public List<string> IncludedLocationIDs { get; set; }
        public List<string> IncludedEntities { get; set; }
        public List<PropertyDataFilter> IncludedEntitiesFilter { get; set; }
        public List<string> FilterLayers { get; set; }
        public bool? OnlyActive { get; set; }
        
        //The maximum number of results that can be retrieved on a single page. Default is 10000
        public int? RequestSize { get; set; }
        public int? RequestPage { get; set; }
    }
}
