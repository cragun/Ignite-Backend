using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataReef.Geography
{
    public class GeoRequest
    {
        private List<DataFilter> filters = new List<DataFilter>();
        private List<string> tags = new List<string>();

        public string QueryID { get; set; } //used to uniquely identify the query regardless of lat,lon.  used for caching base
        public Guid? LayerID { get; set; }
        public PredicateOperators PredicateOperator { get; set; }
        public bool ReturnOnlyFiltered { get; set; }
        public string DataSetID { get; set; }
        public string SubdivideByDimensionID { get; set; }

        public string BoundingWellKnownText { get; set; }

        public List<string> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

        public List<DataFilter> Filters
        {
            get { return this.filters; }
            set { this.filters = value; }
        }

        public List<string> Excluded { get; set; }
    }

    public enum PredicateOperators
    {
        OR = 0,
        AND = 1
    }
}
