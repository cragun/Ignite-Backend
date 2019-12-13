namespace DataReef.TM.Models.DataViews.Geo
{
    public class GeoPoint
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public bool HasValue()
        {
            return Latitude.HasValue && Latitude != 0
                && Longitude.HasValue && Longitude != 0;
        }
    }
}
