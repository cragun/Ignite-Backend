using System;

namespace DataReef.TM.Models.DTOs.Reports
{
    public class SimplePersonKPI
    {
        public SimplePersonKPI() { }

        public SimplePersonKPI(PersonKPI pKPI)
        {
            Name = pKPI.Name;
            Value = pKPI.Value;
            DateCreated = pKPI.DateCreated;
        }

        public string Name { get; set; }
        public int Value { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
