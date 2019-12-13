using System;

namespace DataReef.Integrations.Google.Attributes
{
    public class MetaAttribute : Attribute
    {
        /// <summary>
        /// Name of the column, used for linking main model objects to sheet columns
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Number of times the same object is repeteated
        /// </summary>
        public int Count { get; set; } = 1;
        /// <summary>
        /// Number of sheet columns for every object
        /// </summary>
        public int Size { get; set; } = 1;
        /// <summary>
        /// The index within an object.
        /// This is used when we have a few columns making a single object. 
        /// Within the object, we specify the index of each property
        /// </summary>
        public int ContextIndex { get; set; }

        public MetaAttribute(string name = null)
        {
            Name = name;
        }
    }
}
