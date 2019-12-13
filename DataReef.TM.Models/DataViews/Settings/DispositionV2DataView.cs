using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Settings
{
    public class DispositionV2DataView
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string IconName { get; set; }
        public string Color { get; set; }
        public string Action { get; set; }
        public string Parameters { get; set; }
        public List<DispositionV2DataView> Children { get; set; }

        public List<string> GetUniqueNames()
        {
            if (Children == null)
            {
                return new List<string> { Name };
            }

            var result = new List<string>();

            foreach (var child in Children)
            {
                result.AddRange(child.GetUniqueNames());
            }

            // remove duplicates
            var data = new HashSet<string>(result);

            return data.ToList();
        }
    }
}
