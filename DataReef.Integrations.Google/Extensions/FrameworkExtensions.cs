using DataReef.Integrations.Google.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal static class FrameworkExtensions
{
    internal static List<PropertyInfo> GetMetaProperties(this Type type)
    {
        return type
                .GetProperties()
                .Where(p => p.IsDefined(typeof(MetaAttribute)))
                .ToList();
    }

    internal static IList<string> ToStringsList(this IList<object> values)
    {
        return values?.Select(v => v.ToString()).ToList();
    }
}
