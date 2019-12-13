using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// The request class for creating a new account.
    /// </summary>
    public class CreateAccountRequest
    {
        /// <summary>
        /// If you use a unique identifier for this account in your own systems, you can specify that here. 
        /// This allows you to quickly tie your own data with what you create with the Genability APIs. 
        /// This must be unique within your accounts.
        /// </summary>
        public string providerAccountId { get; set; }


        /// <summary>
        /// This is the name of the account. We suggest using this field to identify the entity this account represents. 
        /// For a small business, this could the name of the business. For a larger business with multiple locations, this could be the name of the location, e.g. Starbucks, Briarwood Mall.
        /// </summary>
        public string accountName { get; set; }


        /// <summary>
        /// Holds any address and geo-location data this account has.
        /// </summary>
        public AccountAddress address { get; set; }

        /// <summary>
        /// The Account has a field named properties which is a collection of zero or more properties. Each entry has a keyName (these correspond 1 to 1 with property values), and a dataValue.
        /// </summary>
        public AccountProperties properties { get; set; }

    }
}
