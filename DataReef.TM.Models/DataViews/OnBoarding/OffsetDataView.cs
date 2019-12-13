using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.OnBoarding
{
    public class OffsetDataView
    {
        [Edge(Index = 0, Type = RoofPanelEdgeType.Ridge)]
        public int Ridge { get; set; }

        [Edge(Index = 1, Type = RoofPanelEdgeType.Eave)]
        public int Eave { get; set; }

        [Edge(Index = 2, Type = RoofPanelEdgeType.Edge)]
        public int Edge { get; set; }

        [Edge(Index = 4, Type = RoofPanelEdgeType.Hip)]
        public int Hip { get; set; }

        [Edge(Index = 3, Type = RoofPanelEdgeType.Valley)]
        public int Valley { get; set; }

        public string ToLegionJSON()
        {
            var properties = typeof(OffsetDataView)
                                .GetProperties()
                                .ToList();

            var data = properties.Select(p =>
            {
                var edgeAttrib = p.GetCustomAttributes(typeof(EdgeAttribute), false).FirstOrDefault() as EdgeAttribute;
                return new EdgeDataView(edgeAttrib, p.Name, p.GetValue(this).ToString());
            });

            return JsonConvert.SerializeObject(data);
        }
    }

    public class EdgeDataView : OrgSettingDataView
    {
        public EdgeDataView(EdgeAttribute attrib, string name, string value)
            : base(SettingValueType.Number)
        {
            Index = attrib.Index.ToString();
            Type = ((int)attrib.Type).ToString();
            DisplayName = Name = name;
            Value = value;
            DetailName = "inches";
        }
    }

    public class EdgeAttribute : Attribute
    {
        public int Index { get; set; }
        public RoofPanelEdgeType Type { get; set; }
    }
}
