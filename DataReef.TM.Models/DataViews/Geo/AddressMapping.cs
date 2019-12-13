namespace DataReef.Geography
{  
    public class AddressMapping
    {
        /// <summary>
        /// The Guid of the location on the Geo server
        /// </summary>
        public string LocationID { get; set; }

        /// <summary>
        /// The id of the location that is external to the Geo server (it comes from the source where Geo server gets the data)
        /// </summary>
        public string ExternalLocationID { get; set; }
    }
}