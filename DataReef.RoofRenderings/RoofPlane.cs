using System;
using System.Collections.Generic;
using System.Data;
using System.Device.Location;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Imaging.Renderings
{
    public class RoofPlane
    {

        public RoofPlane()
        {
            this.Segments = new List<Segment>();
            this.Panels = new List<Panel>();
        }

        public string Label { get; set; }

        public Guid Guid { get; set; }

        public int Azimuth { get; set; }

        public GeoCoordinate CenterCoordinate { get; set; }

        public Point CenterPoint { get; set; }

        public double Tilt { get; set; }

        public double Pitch { get; set; }

        public double Area { get; set; }

        public ICollection<Segment> Segments { get; set; }

        public ICollection<Panel> Panels { get; set; }

        public static RoofPlane FromDataRow(DataRow row,DataSet ds)
        {
            RoofPlane ret = new RoofPlane();
            ret.Guid = new Guid(row["Guid"].ToString());
            ret.Azimuth = (int)row["Azimuth"];
            ret.Tilt = (double)row["Tilt"];
            ret.Pitch = (double)row["Pitch"];

            DataRow[] rows = ds.Tables["Edges"].Select("RoofPlaneID='" + ret.Guid.ToString() +  "'");

            foreach(DataRow erow in rows)
            {
                Segment seg = Segment.FromDataRow(erow);
                ret.Segments.Add(seg);
            }

            return ret;
        }

    }
}
