using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.Geo.DataViews
{
    public class HighResImage
    {
        public HighResImage()
        {
            Guid = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }
        public Guid Guid { get; set; }
        public DateTime DateCreated { get; set; }
        public Guid? CreatedBy { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double MapUnitsPerPixelX { get; set; }
        public double MapUnitsPerPixelY { get; set; }
        public double SkewX { get; set; }
        public double SkewY { get; set; }
        public int Resolution { get; set; }
        public string Source { get; set; }
        public bool IsDeleted { get; set; }
    }
}
