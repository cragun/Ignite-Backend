using System;
using System.Linq;
using Models = DataReef.TM.Models;
using System.Data.Entity.Spatial;
using DataReef.Integrations.Common.Geo;

public static class GeoPropertyExtensions
{
    public static Models.Property ToCoreProperty(this Property geoProperty, Guid territoryID)
    {
        var propertyID = Guid.NewGuid();

        var property = new DataReef.TM.Models.Property
        {
            Guid = propertyID,
            TerritoryID = territoryID,
            ExternalID = geoProperty.Id,
            Name = geoProperty.Name,
            HouseNumber = geoProperty.HouseNumber,
            Address1 = geoProperty.Address1,
            City = geoProperty.City,
            State = geoProperty.State,
            ZipCode = geoProperty.ZipCode,
            StreetName = geoProperty.StreetName,
            PlusFour = geoProperty.PlusFour,
            Latitude = geoProperty.Latitude,
            Longitude = geoProperty.Longitude,
            Attributes = geoProperty.Attributes?.Select(a => new Models.PropertyAttribute
            {
                PropertyID = propertyID,
                TerritoryID = a.TerritoryID,
                DisplayType = a.DisplayType,
                Value = a.Value,
                ExpirationDate = a.ExpirationDate,
                AttributeKey = a.AttributeKey,
                DateCreated = a.DateCreated
            }).ToList(),
            PropertyBag = geoProperty.PropertyBag?.Select(pb => new Models.Geo.Field
            {
                PropertyId = propertyID,
                DisplayName = pb.DisplayName,
                Value = pb.Value
            }).ToList(),
            Occupants =
                geoProperty.Occupants?.Select(o => new { OccupantID = Guid.NewGuid(), Occupant = o })
                    .Select(o => new Models.Geo.Occupant
                    {
                        Guid = o.OccupantID,
                        FirstName = o.Occupant.FirstName,
                        MiddleInitial = o.Occupant.MiddleInitial,
                        LastName = o.Occupant.LastName,
                        LastNameSuffix = o.Occupant.LastNameSuffix,
                        PropertyID = propertyID,
                        PropertyBag = o.Occupant.PropertyBag?.Select(pb => new Models.Geo.Field
                        {
                            OccupantId = o.OccupantID,
                            DisplayName = pb.DisplayName,
                            Value = pb.Value
                        }).ToList()
                    }).ToList(),
            IsGeoProperty = true
        };

        return property;
    }

    /// <summary>
    /// Gets the centroid coordinates and the radius
    /// </summary>
    /// <param name="wkt"></param>
    /// <returns>Lon, Lat, Radius</returns>
    public static Tuple<double, double, double> GetCentroidAndRadius(this string wkt)
    {
        var geom = DbGeometry.FromText(wkt, DbGeometry.DefaultCoordinateSystemId);
        var envelope = geom.Envelope;
        var geoCenter = DbGeography.FromText(geom.Centroid.WellKnownValue.WellKnownText);
        var topLeft = geom.Envelope.PointAt(1);
        var centerTop = DbGeography.PointFromText($"POINT ({geom.Centroid.XCoordinate} {topLeft.YCoordinate})", 4326);
        var radius = geoCenter.Distance(centerTop);
        return new Tuple<double, double, double>(geom.Centroid.XCoordinate ?? 0, geom.Centroid.YCoordinate ?? 0, radius ?? 0);
    }
}
