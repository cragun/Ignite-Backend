using System.Data.SqlTypes;
using Microsoft.SqlServer.Types;

namespace DataReef.TM.Services.Helpers
{
    public static class GeographyHelper
    {
        public static string FixWkt(string wkt)
        {
            var geom = SqlGeography.STGeomFromText(new SqlChars(wkt), 4326).MakeValid();
            if (geom.EnvelopeAngle() >= 90)
            {
                geom.ReorientObject();
            }

            return geom.ToString();
        }
    }
}
