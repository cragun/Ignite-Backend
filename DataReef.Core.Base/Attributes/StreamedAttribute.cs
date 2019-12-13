using System;

namespace DataReef.Core.Attributes
{
    /// <summary>
    /// Allows the sync package builder to send only a fraction of the existing data 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class StreamedAttribute : Attribute
    {
        public StreamedAttribute(int pageSize, string sortByPropertyName, bool sortDesc = true)
        {
            this.PageSize = pageSize;
            SortByPropertyName = sortByPropertyName;
            SortDesc = sortDesc;
        }

        /// <summary>
        /// Number of elements to be retreived
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Sort by property name. Always
        /// </summary>
        public string SortByPropertyName { get; set; }

        /// <summary>
        /// Sort direction. Default
        /// </summary>
        public bool SortDesc { get; set; }
    }
}
