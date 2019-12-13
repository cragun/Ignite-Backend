namespace DataReef.Integrations.Geo.DataViews
{
    public class Shape
    {
        public string ShapeID { get; set; }
        public string State { get; set; }
        public string GeoID { get; set; }
        public string ShapeTypeID { get; set; }
        public string ShapeName { get; set; }
        public string ParentID { get; set; }
        public string ShapeReduced { get; set; }
        public float CentroidLat { get; set; }
        public float CentroidLon { get; set; }
        public float Radius { get; set; }
        public string Description { get; set; }
        public long ResidentCount { get; set; }

    }
}
