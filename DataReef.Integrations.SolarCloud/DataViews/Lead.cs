
using DataReef.Integrations.Common.Geo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.SolarCloud.DataViews
{
    public class Lead
    {
        public Lead()
        {
            this.Guid = Guid.NewGuid();
        }

        public Lead(Property prop)
        {
            Address = prop.Address1;
            City = prop.City;
            State = prop.State;
            ZipCode = prop.ZipCode;
            ExternalID = prop.Id.ToString();
        }

        /// <summary>
        /// Internally Set
        /// </summary>
        public Guid Guid { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Phone { get; set; }

        public string ExternalID { get; set; }

        public Guid TerritoryID { get; set; }

        public double JanConsumption { get; set; }
        public double FebConsumption { get; set; }
        public double MarConsumption { get; set; }
        public double AprConsumption { get; set; }
        public double MayConsumption { get; set; }
        public double JunConsumption { get; set; }
        public double JulConsumption { get; set; }
        public double AugConsumption { get; set; }
        public double SepConsumption { get; set; }
        public double OctConsumption { get; set; }
        public double NovConsumption { get; set; }
        public double DecConsumption { get; set; }


    }
}





