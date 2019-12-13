using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataReef.Imaging.Renderings
{
    public class RenderingData
    {
        public RenderingData(int width, int height)
        {
            this.Padding = 40;
            this.RoofPlanes = new List<RoofPlane>();
            this.Width = width;
            this.Height = height;
        }

        public double Padding { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }


        public Image DownImage { get; set; }

        public Image NorthImage { get; set; }

        public Image EastImage { get; set; }

        public Image WestImage { get; set; }

        public Image SouthImage { get; set; }

        public string HomeOwnerName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }


        public ICollection<RoofPlane> RoofPlanes { get; set; }

        public  void LoadFromDataSet(DataSet dataSet)
        {
            foreach(DataRow row in dataSet.Tables["RoofPlanes"].Rows)
            {
                RoofPlane rp = RoofPlane.FromDataRow(row, dataSet);
                this.RoofPlanes.Add(rp);
            }

            this.Normalize();
        }

        public ICollection<Segment> DedupedSegments
        {
            get
            {

                Dictionary<string, Segment> segments = new Dictionary<string, Segment>();

                foreach(RoofPlane rp in this.RoofPlanes)
                {
                    foreach(Segment s in rp.Segments)
                    {
                        if (segments.ContainsKey(s.ToString())) continue;
                        segments.Add(s.ToString(), s);

                    }
                }

                return segments.Values;
            }
        }

        public void LoadFromJson(string json)
        {
            JObject jo = JObject.Parse(json);

            var planes = jo["roofPlanes"];

            foreach(var plane in planes.Children())
            {
                RoofPlane rp = new RoofPlane();
                this.RoofPlanes.Add(rp);
                rp.Area = double.Parse(plane["area"].ToString());
                rp.Pitch = double.Parse(plane["tilt"].ToString());
                rp.Azimuth = int.Parse(plane["azimuth"].ToString());
                rp.Label = plane["name"].ToString();

                var segments = plane["segments"];
                var panels = plane["panels"];

                foreach (var segment in segments.Children())
                {
                    Segment s = new Segment();
                    s.Length = double.Parse(segment["length"].ToString());
                    s.EdgeType =(RoofPanelEdgeType) Enum.Parse(typeof(RoofPanelEdgeType), segment["type"].ToString());
                    s.Index = int.Parse(segment["index"].ToString());

                    var startPoint = segment["startPoint"];
                    var sX = double.Parse(startPoint["x"].ToString());
                    var sY = double.Parse(startPoint["y"].ToString());
                    s.StartPoint = new PointF((float)sX, (float)sY);


                    var endPoint = segment["endPoint"];
                    var eX = double.Parse(endPoint["x"].ToString());
                    var eY = double.Parse(endPoint["y"].ToString());
                    s.EndPoint = new PointF((float)eX, (float)eY);

                    rp.Segments.Add(s);

                }


                foreach( var panel in panels )
                {
                    Panel p = new Panel();

                    var topLeft = panel["topLeft"];
                    p.X = double.Parse(topLeft["x"].ToString());
                    p.Y = double.Parse(topLeft["y"].ToString());
                    p.Width = double.Parse(panel["width"].ToString());
                    p.Height = double.Parse(panel["height"].ToString());
                    rp.Panels.Add(p);

                    var panelPoints = panel["points"];

                    foreach(var panelPoint in panelPoints)
                    {
                        float x = float.Parse( panelPoint["x"].ToString());
                        float y = float.Parse(panelPoint["y"].ToString());

                        PointF pf = new PointF();
                        pf.X = x;
                        pf.Y = y;

                        p.Points.Add(pf);
                    }
                }
            }


            this.Normalize();

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            double totalArea = this.RoofPlanes.Sum(r => r.Area);
            int totalFacets = this.RoofPlanes.Count;

            double predominantPitchDegrees = this.RoofPlanes.GroupBy(r => r.Pitch)
                                                    .Select(group => new
                                                    {
                                                        Pitch = group.Key,
                                                        Count = group.Count()
                                                    })
                                                    .OrderByDescending(g => g.Count)
                                                    .First()
                                                    .Pitch;

            //convert from degreees to rise/run
            int predominantPitch = (int)(12 * (Math.Tan(predominantPitchDegrees * Math.PI / 180.0)));

            var ridgesLength = this.DedupedSegments
                                            .Where(s => s.EdgeType == RoofPanelEdgeType.Ridge )
                                            .Sum(s => s.Length);

            var hips = this.DedupedSegments
                                            .Where(s => s.EdgeType == RoofPanelEdgeType.Hip)
                                            .Sum(s => s.Length);

            var unassignedLength = this.DedupedSegments
                                            .Where(s => s.EdgeType == RoofPanelEdgeType.Unknown)
                                            .Sum(s => s.Length);



            var ridgesAndHipsLength = this.DedupedSegments
                                            .Where(s => s.EdgeType == RoofPanelEdgeType.Ridge || s.EdgeType == RoofPanelEdgeType.Hip)
                                            .Sum(s => s.Length);

            var eavesLength = this.DedupedSegments
                                            .Where(s => s.EdgeType == RoofPanelEdgeType.Eave)
                                            .Sum(s => s.Length);

            var valleyLength = this.DedupedSegments
                                            .Where(s => s.EdgeType == RoofPanelEdgeType.Valley )
                                            .Sum(s => s.Length);




            var northArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 337.5 && rp.Azimuth < 22.5).Sum(rp => rp.Area);
            var northEastArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 22.5 && rp.Azimuth < 67.5).Sum(rp => rp.Area);
            var eastArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 67.5 && rp.Azimuth < 112.5).Sum(rp => rp.Area);
            var southEastArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 112.5 && rp.Azimuth < 157.5).Sum(rp => rp.Area);
            var southArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 157.5 && rp.Azimuth < 202.5).Sum(rp => rp.Area);
            var southWestArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 202.5 && rp.Azimuth < 247.5).Sum(rp => rp.Area);
            var westArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 247.5 && rp.Azimuth < 292.5).Sum(rp => rp.Area);
            var northWestArea = this.RoofPlanes.Where(rp => rp.Azimuth >= 292.5 && rp.Azimuth < 337.5).Sum(rp => rp.Area);
            var flatArea = this.RoofPlanes.Where(rp=>rp.Pitch ==0 ).Sum(rp => rp.Area);

            sb.AppendLine(string.Format("Total Area: {0} sq ft", (int)totalArea));
            sb.AppendLine(string.Format("Total Facts: {0}", (int)totalFacets));
            sb.AppendLine(string.Format("Predominat Pitch: {0}/12", (int)predominantPitch));

            sb.AppendLine(string.Format("Total Ridges : {0} ft", (int)ridgesLength));
            sb.AppendLine(string.Format("Total Hips : {0} ft", (int)hips));
            sb.AppendLine(string.Format("Total Ridges and Hips: {0} ft", (int)ridgesAndHipsLength));
            sb.AppendLine(string.Format("Total Eaves: {0} ft", (int)eavesLength));
            sb.AppendLine(string.Format("Total Valleys: {0} ft", (int)valleyLength));
            sb.AppendLine(string.Format("Total Unassigned: {0} ft", (int)unassignedLength));

            sb.AppendLine(string.Format("N Facing Facets: {0} sq ft - {1}", (int)northArea,(northArea / totalArea).ToString("p") ));
            sb.AppendLine(string.Format("NE Facing Facets: {0} sq ft - {1}", (int)northEastArea, (northEastArea / totalArea).ToString("p")));
            sb.AppendLine(string.Format("E Facing Facets: {0} sq ft - {1}", (int)eastArea, (eastArea / totalArea).ToString("p")));
            sb.AppendLine(string.Format("SE Facing Facets: {0} sq ft - {1}", (int)southEastArea, (southEastArea / totalArea).ToString("p")));
            sb.AppendLine(string.Format("S Facing Facets: {0} sq ft - {1}", (int)southArea, (southArea / totalArea).ToString("p")));
            sb.AppendLine(string.Format("SW Facing Facets: {0} sq ft - {1}", (int)southWestArea, (southWestArea / totalArea).ToString("p")));
            sb.AppendLine(string.Format("W Facing Facets: {0} sq ft - {1}", (int)westArea, (westArea / totalArea).ToString("p")));
            sb.AppendLine(string.Format("NW Facing Facets: {0} sq ft - {1}", (int)northWestArea, (northWestArea / totalArea).ToString("p")));

            var pitches = this.RoofPlanes.GroupBy(r => r.Pitch)
                                                    .Select(group => new
                                                    {
                                                        Pitch = group.Key,
                                                        Area = group.Sum(rp => rp.Area),
                                                        Count =group.Count()
                                                    })
                                                    .OrderByDescending(g => g.Area);

            foreach(var pitch in pitches)
            {
                sb.AppendLine(string.Format("Pitch: {0} - {1} facets - {2} sq ft", (int)pitch.Pitch, pitch.Count, (int)pitch.Area));
            }

            return sb.ToString();

        }
        
        private void Normalize()
        {

            List<PointF> points = new List<PointF>();
            foreach(RoofPlane rp in this.RoofPlanes)
            {
                foreach(Segment s in rp.Segments)
                {
                    points.Add(s.StartPoint);
                    points.Add(s.EndPoint);
                }
            }

            if (points.Count == 0) return;

            double minY = points.OrderBy(pt => pt.Y).First().Y;
            double minX = points.OrderBy(pt => pt.X).First().X;
            double maxY = points.OrderByDescending(pt => pt.Y).First().Y;
            double maxX = points.OrderByDescending(pt => pt.X).First().X;

            double deltaY = maxY - minY;
            double deltaX = maxX - minX;

            double canvasWidth = this.Width - (this.Padding * 2);
            double canvasHeight = this.Height - (this.Padding * 2);

            double wFactor = this.Width / deltaX;
            double hFactor = this.Height / deltaY;

            double canvasRatio = this.Width / this.Height; //>1 = landscape;  <1 = portrait
            double drawingRatio = deltaX / deltaY; //>1 = landscape;  <1 = portrait

            double drawingWidth = 0;
            double drawingHeight = 0;

            if ((canvasRatio <= 1 && drawingRatio <= 1) || (canvasRatio > 1 && drawingRatio > 1)) //drawing and canvas match
            {
                drawingWidth = canvasWidth;
                drawingHeight = canvasWidth * deltaY / deltaX;

                if (drawingHeight > canvasHeight)
                {
                    drawingHeight = canvasHeight;
                    drawingWidth = canvasHeight * deltaX / deltaY;
                }
            }
            else if (canvasRatio > 1)  // canvas is landscape and drawing is portrait
            {
                drawingHeight = canvasHeight;
                drawingWidth = canvasHeight * deltaX / deltaY;
            }
            else //drawing is landscape and canvas is portrait
            {
                drawingWidth = canvasWidth;
                drawingHeight = canvasWidth * deltaY / deltaX;

                if (drawingHeight > canvasHeight)
                {
                    drawingHeight = canvasHeight;
                    drawingWidth = canvasHeight * deltaX / deltaY;
                }
            }

            double yMultiplier = deltaY / drawingHeight;
            double xMultiplier = deltaX / drawingWidth;

            Image ret = new Bitmap((int)this.Width, (int)this.Height);
            Graphics graphics = Graphics.FromImage(ret);

            foreach(RoofPlane rp in this.RoofPlanes)
            {
                foreach(Segment s in rp.Segments)
                {
                    double x1 = (double)((s.StartPoint.X - minX) / xMultiplier) + this.Padding;
                    double y1 = (double)((double)((s.StartPoint.Y - minY) / yMultiplier) + this.Padding);

                    double x2 = (double)((s.EndPoint.X - minX) / xMultiplier) + this.Padding;
                    double y2 = (double)((double)((s.EndPoint.Y - minY) / yMultiplier) + this.Padding);

                    PointF pt1 = new PointF((float)x1, (float)y1);
                    PointF pt2 = new PointF((float)x2, (float)y2);

                    s.StartPoint = pt1;
                    s.EndPoint = pt2;

                }

                foreach( Panel p in rp.Panels)
                {
                    double x = p.X;
                    double y = p.Y;

                    double x1 = (double)((x - minX) / xMultiplier) + this.Padding;
                    double y1 = (double)((y- minY) / yMultiplier) + this.Padding;

                    p.X = x1;
                    p.Y = y1;

                    p.Height = p.Height * yMultiplier;
                    p.Width = p.Width * xMultiplier;

                    List<PointF> normalizedPoints = new List<PointF>();
                  
                    foreach(PointF point in p.Points)
                    {
                        double x2 = (double)(((point.X - minX) - 0) / xMultiplier) + this.Padding;
                        double y2 = (double)(((point.Y - minY) - 0) / yMultiplier) + this.Padding;
                   
                        normalizedPoints.Add(new PointF((float)x2, (float)y2));

                    }

                    p.Points = normalizedPoints;


                }

            }
        }
    }
}
