using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public static class FrameworkExtensions
{
    public static List<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type)
    {
        return type
                .GetProperties()
                .Where(p => p.IsDefined(typeof(T)))
                .ToList();
    }

    public static int MonthsBetween(this DateTime start, DateTime end)
    {
        return Math.Abs((end.Year - start.Year) * 12 + end.Month - start.Month + (end.Day >= start.Day ? 0 : -1));
    }

    public static byte[] ToCharArray(this Stream stream)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    public static string GetAWSPath(this string value)
    {
        var uri = new Uri(value);
        return uri.AbsolutePath.TrimStart('/');
    }

    public static string GetAWSProxifyUrl(this string url)
    {
        string proxy_path = ConfigurationManager.AppSettings["AWS_S3_ProxyUrl"];

        Regex regexA = new Regex(@"^https:\/\/(?<bucket>.+)\.s3(.+)?\.amazonaws\.com\/(?<filepath>.+)(?:\?.+)?$");
        Match matchA = regexA.Match(url);

        Regex regexB = new Regex(@"^https:\/\/s3-(?<region>.+)?\.amazonaws\.com\/(?<bucket>[^\/]+)\/(?<filepath>.+)(?:\?.*)?$");
        Match matchB = regexB.Match(url);

        if (matchA.Success)
            return $"{proxy_path}/{matchA.Groups["bucket"].Value}/{matchA.Groups["filepath"].Value}";
        else if (matchB.Success)
            return $"{proxy_path}/{matchB.Groups["bucket"].Value}/{matchB.Groups["filepath"].Value}";
        else
            return url;
    } 

    public static bool IsTrue(this string value)
    {
        return value == "1" || value?.ToLower() == "true";
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// Compares two lists and returns elements that are presend in the 2nd list but not in the 1st one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="initialList"></param>
    /// <param name="newList"></param>
    /// <returns></returns>
    public static IList<T> GetNewElements<T>(this IList<T> initialList, IList<T> newList)
    {
        if (initialList?.Any() != true)
        {
            return newList;
        }
        if (newList?.Any() != true)
        {
            return null;
        }

        var initialHash = new HashSet<T>(initialList);
        var newHash = new HashSet<T>(newList);
        var intersection = initialList.Intersect(newHash);
        return newList.Where(n => !intersection.Contains(n)).ToList();
    }
}
