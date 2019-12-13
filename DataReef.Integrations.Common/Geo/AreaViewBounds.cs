namespace DataReef.Integrations.Common.Geo
{
    public class AreaViewBounds
    {
        public float Lat1 { get; set; } //top  
        public float Lat2 { get; set; } //bottom
        public float Lon1 { get; set; } //left
        public float Lon2 { get; set; } //right

        public string ToWKT()
        {
            return $"Polygon(({Lon1} {Lat1},{Lon2} {Lat1},{Lon2} {Lat2},{Lon1} {Lat2},{Lon1} {Lat1}))";
        }
    }
}
