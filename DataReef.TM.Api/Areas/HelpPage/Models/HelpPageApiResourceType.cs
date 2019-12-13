namespace DataReef.TM.Api.Areas.HelpPage.Models
{
    /// <summary>
    /// Enum describing types of API resources
    /// </summary>
    public enum HelpPageApiResourceType
    {
        /// <summary>
        /// Not used
        /// </summary>
        None,
        /// <summary>
        /// A controller
        /// </summary>
        Controller,
        /// <summary>
        /// A complex object type
        /// </summary>
        ComplexType,
        /// <summary>
        /// A enum type
        /// </summary>
        Enum
    }
}