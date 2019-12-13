using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// Holds any address and geo-location data an account has.
    /// </summary>
    public class AccountAddress
    {
        /// <summary>
        /// Full free text string representing address. This is the property that is usually passed in. Zip will suffice but the full address is better.
        /// </summary>
        public string addressString { get; set; }


        /// <summary>
        /// A name associated with the address (e.g. Headquarters). Not usually set.
        /// </summary>
        public string addressName { get; set; }

        /// <summary>
        /// First part of the address. Usually populated by our address validation from the addressString passed in.
        /// </summary>
        public string address1 { get; set; }

        /// <summary>
        /// Second part of the address. Usually populated by our address validation from the addressString passed in.
        /// </summary>
        public string address2 { get; set; }


        /// <summary>
        /// City of the address. Usually populated by Genability address validation from the addressString passed in.
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// State of the address. Usually populated by Genability address validation from the addressString passed in.
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// Postcode or ZIP code of the address. Usually populated by Genability address validation from the addressString passed in.
        /// </summary>
        public string zip { get; set; }

        /// <summary>
        /// Usually the ISO Country Code of the address. Usually populated by Genability address validation from the addressString passed in.
        /// </summary>
        public string country { get; set; }

        /// <summary>
        /// Longitude of the address. Genability populates this by geo-tagging the address.
        /// </summary>
        public string lon { get; set; }

        /// <summary>
        /// Latitude of the address. Genability populates this by geo-tagging the address.
        /// </summary>
        public string lat { get; set; }
    }
}
