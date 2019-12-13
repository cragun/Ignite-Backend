
using DataReef.TM.Models.Geo;
namespace DataReef.TM.Models.DataViews.Geo
{
    public class GeoField
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public string Group { get; set; }

        public Field ToDatabaseEntity(System.Guid guid, System.Guid? occupantId = null)
        {
            return new Field
            {
                Guid = System.Guid.NewGuid(),
                Name = this.Name,
                DisplayName = this.DisplayName,
                Value = this.Value,
                OccupantId = occupantId
            };
        }
    }
}
