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
    public class Segment
    {
        public GeoCoordinate StartCoordinate { get; set; }

        public GeoCoordinate EndCoordinate { get; set; }

        public PointF StartPoint { get; set; }

        public PointF EndPoint { get; set; }

        public RoofPanelEdgeType EdgeType { get; set; }

        public  int Index { get; set; }

        public Guid RoofPlaneID { get; set; }
        
        public double FireOffset { get; set; }

        public bool FireOffsetEnabled { get; set; }

        public double Length { get; set; }

        public PointF MidPoint
        {
            get
            {
                double x = (this.StartPoint.X + this.EndPoint.X) / 2.0;
                double y = (this.StartPoint.Y + this.EndPoint.Y) / 2.0;

                return new PointF((float)x,(float) y);

            }
            private set { }
        }

        public static Segment FromDataRow(DataRow row)
        {
            Segment s = new Segment();

            GeoCoordinate startCoordinate = new GeoCoordinate();
            startCoordinate.Longitude = (double)row["StartPointLongitude"];
            startCoordinate.Latitude = (double)row["StartPointLatitude"];

            GeoCoordinate endCoordinate = new GeoCoordinate();
            endCoordinate.Longitude = (double)row["EndPointLongitude"];
            endCoordinate.Latitude = (double)row["EndPointLatitude"];

            int pixelX1 = 0;
            int pixelY1 = 0;

            int pixelX2 = 0;
            int pixelY2 = 0;

            Microsoft.MapPoint.TileSystem.LatLongToPixelXY(startCoordinate.Latitude, startCoordinate.Longitude, 23, out pixelX1, out pixelY1);
            Microsoft.MapPoint.TileSystem.LatLongToPixelXY(endCoordinate.Latitude, endCoordinate.Longitude, 23, out pixelX2, out pixelY2);

            Point pt1 = new Point(pixelX1, pixelY1);
            Point pt2 = new Point(pixelX2, pixelY2);

            s.StartPoint = pt1;
            s.EndPoint = pt2;

            s.EdgeType = (RoofPanelEdgeType)row["EdgeType"];
            s.Index = (int)row["Index"];
            s.FireOffset = (double)row["FireOffset"];
            s.RoofPlaneID = new Guid(row["RoofPlaneID"].ToString());
            s.StartCoordinate = startCoordinate;
            s.EndCoordinate = endCoordinate;
            s.Length = s.StartCoordinate.GetDistanceTo(s.EndCoordinate) * 3.28084; //convert to feet

            return s;
        }

        public override string ToString()
        {
            string ret = string.Empty;

            List<float> points = new List<float>();
            points.Add(this.StartPoint.X);
            points.Add(this.StartPoint.Y);
            points.Add(this.EndPoint.X);
            points.Add(this.EndPoint.Y);

            points.Sort();

            ret = string.Format("{0},{1}-{2},{3}", Math.Round(points[0], 1), Math.Round(points[1], 1), Math.Round(points[2], 1), Math.Round(points[3], 1));

            return ret;
        }
    }
}
