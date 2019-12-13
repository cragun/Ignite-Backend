using System;

namespace DataReef.Core.Attributes
{
    /// <summary>
    /// Marker Attribute for entities to be trated as soft deletes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SoftDeleteAttribute : Attribute
    {
    }
}
