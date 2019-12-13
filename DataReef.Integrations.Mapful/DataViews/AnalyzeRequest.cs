using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Mapful.DataViews
{
    public class AnalyzeRequest
    {
        public AnalyzeRequest()
        {
            //initialize bounding box w/ US bounding box
            // North Latitude: 71.538800 South Latitude: 18.776300 East Longitude: -66.885417 West Longitude: 
            //Top = 71.538800;
            //Bottom = 18.776300;
            //Left = 170.595700;
            //Right = -66.885417;
            //// master residential data
            DataSetID = new Guid("2de2d4f2-abe1-437a-82e5-dbc67cb37731");
        }

        public Guid DataSetID { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }

        public List<Filter> Filters { get; set; }
    }

    public class Filter
    {
        public string FieldID { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }

    }
}
