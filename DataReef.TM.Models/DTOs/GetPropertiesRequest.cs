using System;
using System.Collections.Generic;
using DataReef.Integrations.Common.Geo;

namespace DataReef.TM.Models.DTOs
{
    public class GetPropertiesRequest
    {
        public List<PropertiesRequest> GeoPropertiesRequest { get; set; }

        public ListPropertiesRequest PropertiesRequest { get; set; }

        public Guid TerritoryID { get; set; }

        public AreaViewBounds AreaViewBounds { get; set; }
    }
}
