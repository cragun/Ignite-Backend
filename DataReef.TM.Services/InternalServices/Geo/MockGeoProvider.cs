using DataReef.TM.Models;
using DataReef.TM.Models.DataViews.Geo;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataReef.TM.Models.DTOs.Mortgage;

namespace DataReef.TM.Services.InternalServices.Geo
{
    public class MockGeoProvider : IGeoProvider

    {
        public MockGeoProvider()
        {

        }

        public int NumberOfHomesInShape(string shapeWKT)
        {
            return 250;
        }

        public long PropertiesCountInShapeIds(List<Guid> shapeIds)
        {
            return 100;
        }

        public ICollection<GeoProperty> GetPropertiesInShape(string shapeWKT)
        {
            List<GeoProperty> ret = new List<GeoProperty>();

            for (int x = 0; x < 250; x++)
            {
                GeoProperty rr = new GeoProperty();
                int houseNumber = 1000 + (x * 2);
                rr.Address1 = string.Format("{0} W Center St", houseNumber);
                rr.Guid = Guid.NewGuid();
                rr.City = "Provo";
                rr.State = "UT";
                rr.ZipCode = "84062";
                rr.StreetName = "W Center";
                rr.HouseNumber = "1111";
                rr.Name = "Smith";
                rr.PlusFour = "1234";
                ret.Add(rr);
            }

            return ret;
        }

        public Task<int> PrescreenedPropertiesCountForTerritory(Territory territory)
        {
            throw new NotImplementedException();
        }

        public Task<List<IdAndCountResponse>> PrescreenedPropertiesCountForTerritories(List<Territory> territories)
        {
            throw new NotImplementedException();
        }   

        public bool CanDeleteShape(Guid shapeId, List<Guid> shapeIds)
        {
            throw new NotImplementedException();
        }

        public MortgageSource GetMortgageDetails(MortgageRequest mortgageRequest)
        {
            throw new NotImplementedException();
        }

        public List<ShapeSummary> BulkPropertiesCount(ICollection<ShapeCollectionRequest> req)
        {
            throw new NotImplementedException();
        }

        public long PropertiesCountForWKT(string wkt)
        {
            throw new NotImplementedException();
        }
    }
}
