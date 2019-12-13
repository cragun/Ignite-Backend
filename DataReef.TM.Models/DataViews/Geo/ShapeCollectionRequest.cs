using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DataViews.Geo
{
    public class ShapeCollectionRequest
    {
        public Guid ExternalID { get; set; }
        public List<Guid> ShapeIDs { get; set; }
        public bool? OnlyActive { get; set; }
    }
}
