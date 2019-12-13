using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Geo
{
    public class TerritoryIdAndWKTRequest
    {
        public TerritoryIdAndWKTRequest()
        { }

        public TerritoryIdAndWKTRequest(Territory territory)
        {
            TerritoryId = territory.Guid.ToString();
            WellKnownText = territory.WellKnownText;
        }

        public string WellKnownText { get; set; }

        public string TerritoryId { get; set; }
    }
}
