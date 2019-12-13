using System.Collections.Generic;

namespace DataReef.Integrations.Common.Geo
{
    public class PropertyDataFilter
    {
        public string Name { get; set; }
        public Dictionary<string, string> Filters { get; set; }
        public bool ExcludeWhenNotMatching { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}