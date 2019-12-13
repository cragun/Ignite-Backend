namespace DataReef.Core.Attributes
{
    /// <summary>
    /// Service scope
    /// </summary>
    public enum ServiceScope
    {
        /// <summary>
        /// 
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Request,

        /// <summary>
        /// ToDo: remove this. We will not maintain session
        /// </summary>
        Session,

        /// <summary>
        /// 
        /// </summary>
        Application
    }
}