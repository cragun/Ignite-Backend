using Newtonsoft.Json;
using System.Drawing;


namespace DataReef.Integrations.Pictometry
{
    public class SearchResult
    {
        public string OrientationString { get; set; }

        public ImageOrientation Orientation { get; set; }

        public double Top { get; set; }

        public double Left { get; set; }

        public double Bottom { get; set; }

        public double Right { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public double MapUnitsPerPixelX { get; set; }

        public double MapUnitsPerPixelY { get; set; }

        public double SkewX { get; set; }

        public double SkewY { get; set; }

        public string MetadataUrl { get; set; }

        public int Resolution { get; set; }

        public string Date { get; set; }

        [JsonIgnore]
        public string Url { get; set; }

        [JsonIgnore]
        public Point SearchPoint { get; set; }
    }
}
