using System;

namespace DataReef.TM.Api.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal class CrudApiActionAttribute : Attribute
    {
    }
}