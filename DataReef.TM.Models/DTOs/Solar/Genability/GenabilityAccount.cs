using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.DTOs.Solar.Genability
{
    /// <summary>
    /// An Account object is a place where you can store your customers' energy, cost, and savings data. 
    /// Accounts typically represent your actual customers and potential customers, and are most useful when you map an account to a facility, such as a home or business. 
    /// Genability recommends mapping an account to a site, and even if the site has multiple meters, although sometimes it's helpful to consider having an account for each individual utility meter location.
    /// Accounts are completely private and only visible to your organization. 
    /// </summary>
    public class GenabilityAccount:BaseResponse
    {

        /// <summary>
        /// This is the unique Genability ID of this Account object.
        /// </summary>
        public string accountId { get; set; }

        /// <summary>
        /// If you use a unique identifier for this account in your own systems, you can specify that here. 
        /// This allows you to quickly tie your own data with what you create with the Genability APIs. 
        /// This must be unique within your accounts.
        /// </summary>
        public string providerAccountId { get; set; }

        /// <summary>
        /// This is the name of the account. We suggest using this field to identify the entity this account represents. 
        /// For a small business, this could the name of the business. 
        /// For a larger business with multiple locations, this could be the name of the location, e.g. Starbucks, Briarwood Mall.
        /// </summary>
        public string accountName { get; set; }

        /// <summary>
        /// This allows grouping of accounts together. The customerOrgId should be an ID you use in your own systems to identify this account. Specifying a customerOrgId simplifies tying the account to your own data.
        /// </summary>
        public string customerOrgId { get; set; }

        /// <summary>
        /// This is the name of the account's organization. We suggest using this field to identify the name of the account's business. For example, Starbucks.
        /// </summary>
        public string customerOrgName { get; set; }

        /// <summary>
        /// Holds any address and geo-location data this account has.
        /// </summary>
        public AccountAddress address { get; set; }

        /// <summary>
        /// The timezone that this account is located in. They are in the tz database format.
        /// </summary>
        public string timeZone { get; set; }

        /// <summary>
        /// Use this to set the person who currently "owns" this account (otherwise its null).
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// This indicates the status of this account. Possible values are: ACTIVE (defaults to this), INACTIVE and DELETED.
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// This indicates the status of this account. Possible values are: ACTIVE (defaults to this), INACTIVE and DELETED.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// This is a list of properties that exist on this account.
        /// </summary>
        public List<AccountProperty> properties { get; set; }

        /// <summary>
        /// This is a list of tariffs that are on the account.
        /// </summary>
        public List<Tariff> tariffs { get; set; }

    }
}
