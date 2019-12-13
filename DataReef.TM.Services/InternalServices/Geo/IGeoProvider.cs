using DataReef.TM.Models.DataViews.Geo;
using System;
using System.Collections.Generic;
using DataReef.TM.Models.DTOs.Mortgage;

namespace DataReef.TM.Services.InternalServices.Geo
{
    public interface IGeoProvider
    {
        long PropertiesCountInShapeIds(List<Guid> shapeIds);

        long PropertiesCountForWKT(string wkt);

        List<ShapeSummary> BulkPropertiesCount(ICollection<ShapeCollectionRequest> req);

        bool CanDeleteShape(Guid shapeId, List<Guid> shapeIds);

        MortgageSource GetMortgageDetails(MortgageRequest mortgageRequest);
    }
}
