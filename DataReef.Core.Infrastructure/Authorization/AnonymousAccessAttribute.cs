using System;

namespace DataReef.Core.Infrastructure.Authorization
{
    /// <summary>
    /// Allows anonymous access to service contracts that are marked with this attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class AnonymousAccessAttribute : Attribute
    {

    }
}