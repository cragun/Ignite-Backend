using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UrlHelpers
{
    /// <summary>
    /// Method that builds the query string based on an object's properties.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="separator"></param>
    /// <param name="toLowerValues"></param>
    /// <returns></returns>
    public static string ToQueryString(this object request, string separator = ",", bool toLowerValues = false)
    {
        if (request == null)
            throw new ArgumentNullException("request");

        // Get all properties on the object
        var properties = request.GetType().GetProperties()
            .Where(x => x.CanRead)
            .Where(x => x.GetValue(request)?.Equals(GetDefault(x.PropertyType)) == false)
            .ToDictionary(x => (x.GetCustomAttributes(typeof(JsonPropertyAttribute), false)?.FirstOrDefault() as JsonPropertyAttribute)?.PropertyName ?? x.Name,
                          x => x.GetValue(request, null));


        // Get names for all IEnumerable properties (excl. string)
        var propertyNames = properties
            .Where(x => !(x.Value is string) && x.Value is IEnumerable)
            .Select(x => x.Key)
            .ToList();

        // Concat all IEnumerable properties into a comma separated string
        foreach (var key in propertyNames)
        {
            var valueType = properties[key].GetType();
            var valueElemType = valueType.IsGenericType
                                    ? valueType.GetGenericArguments()[0]
                                    : valueType.GetElementType();
            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
            {
                var enumerable = properties[key] as IEnumerable;
                properties[key] = string.Join(separator, enumerable.Cast<object>());
            }
        }

        // Concat all key/value pairs into a string separated by ampersand
        return string.Join("&", properties
            .Select(x => string.Concat(
                Uri.EscapeDataString(x.Key), "=",
                Uri.EscapeDataString(toLowerValues ? x.Value.ToString().ToLower() : x.Value.ToString()))));
    }

    public static object GetDefault(Type t)
    {
        Func<object> f = GetDefault<object>;
        return f.Method.GetGenericMethodDefinition().MakeGenericMethod(t).Invoke(null, null);
    }

    private static T GetDefault<T>()
    {
        return default(T);
    }
}
