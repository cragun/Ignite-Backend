using System;

namespace DataReef.Integrations.Common.Geo
{
    public class PropertyAttribute
    {
        public Guid Guid { get; set; }

        public string PropertyID { get; set; }

        public string Value { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string AttributeKey { get; set; }

        public string OwnerID { get; set; }

        public string DisplayType { get; set; }

        public DateTime DateCreated { get; set; }

        public Guid? TerritoryID { get; set; }

        //        public int ExpiryMinutes { get; set; }
    }
}
