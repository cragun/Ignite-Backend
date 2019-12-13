using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Imaging.Renderings
{
    public class Panel
    {

        public Panel()
        {
            this.Points = new List<PointF>();
        }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Azimuth { get; set; }
     

        public List<PointF> Points { get; set; }

        public List<PointF> TranslatedPoints
        {
            get
            {
                List<PointF> ret = new List<PointF>();

                ret.Add(new PointF(0, 0));
                ret.Add(new PointF((float)this.Width, 0));
                ret.Add(new PointF((float)this.Width, (float)this.Height));
                ret.Add(new PointF(0, (float)this.Height));

                return ret;
            }
        }
    }
}
