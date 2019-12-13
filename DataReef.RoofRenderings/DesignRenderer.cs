using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Data;

namespace DataReef.Imaging.Renderings
{
    public class DesignRenderer
    {

        public static Dictionary<RenderStyle,Image> AllRenderings (RenderingData data, int width, int height)
        {
            Dictionary<RenderStyle, Image> ret = new Dictionary<RenderStyle, Image>();

            ret.Add(RenderStyle.Area, RenderArea(data));
            ret.Add(RenderStyle.Azimuth, RenderArea(data));
            ret.Add(RenderStyle.Degrees, RenderArea(data));
            ret.Add(RenderStyle.Labeled, RenderArea(data));
            ret.Add(RenderStyle.Lengths, RenderArea(data));
            ret.Add(RenderStyle.PanelsShaded, RenderArea(data));
            ret.Add(RenderStyle.PanelsWireframe, RenderArea(data));
            ret.Add(RenderStyle.Pitch, RenderArea(data));
            ret.Add(RenderStyle.SimpleShaded, RenderArea(data));
            ret.Add(RenderStyle.WireFrame, RenderArea(data));

            return ret;

        }

        public static Image RenderDesign(RenderingData data,int width,int height,RenderStyle style)
        {
        
            if( style == RenderStyle.Lengths)
            {
                return RenderLengths(data);
            }
            if (style == RenderStyle.WireFrame)
            {
                return RenderWireFrames(data);
            }

            if (style == RenderStyle.SimpleShaded)
            {
                return RenderShaded(data);
            }

            if (style == RenderStyle.Area)
            {
                return RenderArea(data);
            }
      
            if (style == RenderStyle.Labeled)
            {
                return RenderLabels(data);
            }

            if (style == RenderStyle.Pitch)
            {
                return RenderPitch(data);
            }

            if (style == RenderStyle.Degrees)
            {
                return RenderDegrees(data);
            }

            if (style == RenderStyle.Azimuth)
            {
                return RenderAzimuth(data);
            }

            if (style == RenderStyle.PanelsWireframe)
            {
                return RenderWireFramesWithPanels(data);
            }

            if (style == RenderStyle.PanelsShaded)
            {
                return RenderWireFramesWithShadedPanels(data);
            }

            if (style == RenderStyle.Gradient)
            {
                return RenderGradient(data);
            }

            return null;
            
        }

        private static Image RenderGradient(RenderingData data)
        {

            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s => s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }

                LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(100, 100), Color.WhiteSmoke, Color.DarkGray);
                graphics.FillPolygon(brush, points.ToArray());


                Pen pen = new Pen(Brushes.Gray, 3);
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPolygon(pen, points.ToArray());

                //graphics.FillPolygon()


                PointF centroid = GetCentroid(points);

                string label = string.Format("{0}°", Math.Round(roofPlane.Pitch, 0));
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Black, centroid, sf);

            }

            return ret;
        }


        private static Image RenderLabels(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s => s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }
                Pen pen = new Pen(Brushes.Gray, 3);
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPolygon(pen, points.ToArray());


                PointF centroid = GetCentroid(points);

                string label = string.Format("{0}", roofPlane.Label);
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Black, centroid, sf);


            }


            return ret;
        }

        private static Image RenderAzimuth(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s => s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }
                Pen pen = new Pen(Brushes.Gray, 3);
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPolygon(pen, points.ToArray());


                PointF centroid = GetCentroid(points);

                int rotation = roofPlane.Azimuth - 90;
                if (rotation < 0) rotation += 360;
                if (rotation >= 360) rotation -= 360;


                bool isFlipped = false;

                if (roofPlane.Azimuth > 180)
                {
                    rotation += 180;
                    isFlipped = true;
                }

                string rightString = @"\u2192";
                string leftString = @"\u2190";
              

                string arrow = System.Text.RegularExpressions.Regex.Unescape(isFlipped ? leftString : rightString);
                string label = string.Format(@"{0} {1}°{2}", isFlipped ? arrow : string.Empty, roofPlane.Azimuth, isFlipped ? string.Empty : arrow).Trim();
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;


                GraphicsState state = graphics.Save();
                graphics.ResetTransform();

                graphics.TranslateTransform(centroid.X, centroid.Y);
                graphics.RotateTransform(rotation);
                // Draw the text at the origin.
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Black,new PointF(0,0), sf);
                // Restore the graphics state.
                graphics.Restore(state);



            }


            return ret;
        }



        private static Image RenderPitch(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s => s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }
                Pen pen = new Pen(Brushes.Gray, 3);
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPolygon(pen, points.ToArray());


                PointF centroid = GetCentroid(points);

                string label = string.Format("{0}°", Math.Round(roofPlane.Pitch, 0));
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Black, centroid, sf);

            }


            return ret;
        }


        private static Image RenderDegrees(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s => s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }
                Pen pen = new Pen(Brushes.Gray, 3);
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPolygon(pen, points.ToArray());


                PointF centroid = GetCentroid(points);

                string label = string.Format("{0}°", Math.Round( roofPlane.Pitch,0));
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Black, centroid, sf);


            }


            return ret;
        }



        private static Image RenderArea(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            

            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s => s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }
                Pen pen = new Pen(Brushes.Gray, 3);
                pen.LineJoin = LineJoin.Round;
                graphics.DrawPolygon(pen, points.ToArray());


                PointF centroid = GetCentroid(points);

                string label = string.Format("{0}", Math.Round(roofPlane.Area, 0));
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                graphics.DrawString(label, new Font("Arial", 12), Brushes.Black, centroid, sf);


            }
            

            return ret;
        }

        public static PointF GetCentroid(List<PointF> points)
        {
            float totalX = 0,  totalY = 0;
            foreach (PointF p in points)
            {
                totalX += p.X;
                totalY += p.Y;
            }
            float centerX = totalX / points.Count;
            float centerY = totalY / points.Count;

            return new PointF(centerX, centerY);
        }


       
        private static Pen PenForEdgeType(RoofPanelEdgeType type)
        {
            var kRidgeColor = Color.Crimson;
            var kEaveColor = Color.Black;
            var kHipColor = Color.Red;
            var kEdgeColor = Color.Green;
            var kValleyColor = Color.Gray;

            Pen ret = Pens.Black;

            if(type== RoofPanelEdgeType.Eave)
            {
                ret = new Pen(new SolidBrush(kEaveColor),4.0f);
            }
            else if (type == RoofPanelEdgeType.Ridge)
            {
                ret = new Pen(new SolidBrush(kRidgeColor),4.0f);
            }
            else if (type == RoofPanelEdgeType.Hip)
            {
                ret = new Pen(new SolidBrush(kHipColor), 2.0f);
            }
            else if (type == RoofPanelEdgeType.Edge)
            {
                ret = new Pen(new SolidBrush(kEdgeColor),4.0f);
            }
            else if (type == RoofPanelEdgeType.Valley)
            {
                ret = new Pen(new SolidBrush(kValleyColor), 2.0f);
                ret.DashStyle = DashStyle.Dash;
                ret.DashPattern = new float[] { 2.0F, 2.0F };
            }

            return ret;
        }
       

        private static Image RenderLengths(RenderingData data)
        {

            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                foreach (var segment in roofPlane.Segments)
                {
                    //System.Diagnostics.Debug.WriteLine(segment.ToString());

                    if (drawnSegments.ContainsKey(segment.ToString())) continue;
                    drawnSegments.Add(segment.ToString(), segment);

                    Pen pen = PenForEdgeType(segment.EdgeType);
                    graphics.DrawLine(pen, segment.StartPoint, segment.EndPoint);


                    PointF centroid = segment.MidPoint;

                    //float deltaX = Math.Max(segment.StartPoint.X, segment.EndPoint.X) - Math.Min(segment.StartPoint.X, segment.EndPoint.X);
                    //float deltaY = Math.Max(segment.StartPoint.Y, segment.EndPoint.Y) - Math.Min(segment.StartPoint.Y, segment.EndPoint.Y);

                    float deltaX = segment.EndPoint.X - segment.StartPoint.X;
                    float deltaY = segment.EndPoint.Y - segment.StartPoint.Y;



                    //atan2 for angle
                    double radians = Math.Atan2(deltaY, deltaX);

                    //radians into degrees
                    double angle = radians * (180 / Math.PI);

                    System.Diagnostics.Debug.WriteLine(string.Format("{0}:{1}", angle, segment.Length));

                    string label = string.Format(@"{0}", Math.Round(segment.Length,0) );
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;


                    GraphicsState state = graphics.Save();
                    graphics.ResetTransform();

                    graphics.TranslateTransform(centroid.X, centroid.Y);
                    graphics.RotateTransform((float)angle);
                    // Draw the text at the origin.
                    graphics.DrawString(label, new Font("Arial", 12), Brushes.Black, new PointF(0, 20), sf);
                    // Restore the graphics state.
                    graphics.Restore(state);


                }
            }

            return ret;
        }

        private static Image RenderWireFramesWithPanels(RenderingData data)
        {

            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen roofPlanePen = new Pen(Brushes.Black, 4);
            Pen panelPen = new Pen(Brushes.Black, 2);
            Brush panelFillBrush = Brushes.WhiteSmoke;

            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {

                foreach (var segment in roofPlane.Segments)
                {
                    if (drawnSegments.ContainsKey(segment.ToString())) continue;
                    drawnSegments.Add(segment.ToString(), segment);
                    graphics.DrawLine(roofPlanePen, segment.StartPoint, segment.EndPoint);
                }

                foreach (var panel in roofPlane.Panels)
                {
                
                    if (panel.Points.Count == 0) continue;
                    graphics.FillPolygon(panelFillBrush, panel.Points.ToArray());
                    graphics.DrawPolygon(panelPen, panel.Points.ToArray());

                }

            }

            return ret;
        }


        private static Image RenderWireFramesWithShadedPanels(RenderingData data)
        {

            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Pen roofPlanePen = new Pen(Brushes.Black, 4);
            Pen panelPen = new Pen(Brushes.White, 2);
            Brush panelFillBrush = Brushes.Black;

            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {

                foreach (var segment in roofPlane.Segments)
                {
                    if (drawnSegments.ContainsKey(segment.ToString())) continue;
                    drawnSegments.Add(segment.ToString(), segment);
                    graphics.DrawLine(roofPlanePen, segment.StartPoint, segment.EndPoint);
                }

                foreach (var panel in roofPlane.Panels)
                {
                    if (panel.Points.Count == 0) continue;
                    graphics.FillPolygon(panelFillBrush, panel.Points.ToArray());
                    graphics.DrawPolygon(panelPen, panel.Points.ToArray());
                }
            }

            return ret;
        }

        private static Image RenderWireFrames(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Dictionary<string, Segment> drawnSegments = new Dictionary<string, Segment>();

            foreach (var roofPlane in data.RoofPlanes)
            {
                foreach (var segment in roofPlane.Segments)
                {
                    if (drawnSegments.ContainsKey(segment.ToString())) continue;
                    drawnSegments.Add(segment.ToString(), segment);
                    Pen p = new Pen(Brushes.Black, 4);
                    graphics.DrawLine(p, segment.StartPoint, segment.EndPoint);

                }
            }

            return ret;
        }


        private static Image RenderShaded(RenderingData data)
        {
            Image ret = new Bitmap((int)data.Width, (int)data.Height);
            Graphics graphics = Graphics.FromImage(ret);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            graphics.FillRectangle(Brushes.White, new RectangleF(0, 0, (float)data.Width, (float)data.Height));

            Color color = ColorTranslator.FromHtml("#F9B125");
            Color lightColor = Color.FromArgb((int)(255 * .2), color);
            lightColor = Color.WhiteSmoke;

            foreach (var roofPlane in data.RoofPlanes)
            {
                List<PointF> points = new List<PointF>();

                foreach (var segment in roofPlane.Segments.OrderBy(s=>s.Index))
                {
                    points.Add(segment.StartPoint);
                    points.Add(segment.EndPoint);
                }
                Pen pen = new Pen(Brushes.Black, 3);

                if(roofPlane.Azimuth<180)
                {
                    graphics.FillPolygon(new SolidBrush(lightColor), points.ToArray());
                }

                graphics.DrawPolygon(pen, points.ToArray());


            }

            return ret;
        }

    }
}
