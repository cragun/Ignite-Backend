using System.Collections.Generic;

namespace DataReef.Integrations.Common.Geo
{
    public class Occupant
    {
        public Occupant()
        {
            this.PropertyBag = new List<Field>();
        }

        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        public string LastName { get; set; }

        public string LastNameSuffix { get; set; }

        public string PropertyID { get; set; }

        public string Guid { get; set; }

        public List<Field> PropertyBag { get; set; }

        public string GetFullName()
        {
            return $"{FirstName} {(!string.IsNullOrEmpty(MiddleInitial) ? MiddleInitial + ". " : " ")}{LastName}";
        }

    }

}
