using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.IO;
using System.Linq;

public static class StringExtensions
{
    public static Image ToImage(this string base64)
    {
        var bytes = Convert.FromBase64String(base64);

        Image image;
        using (var ms = new MemoryStream(bytes))
        {
            image = Image.FromStream(ms);
        }

        return image;
    }

    public static Image ToImage(this byte[] data)
    {
        return Image.FromStream(new MemoryStream(data));
    }

    public static Image ToThumbnail(this string base64, int width, int height)
    {
        var image = base64.ToImage();
        var thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

        return thumbnail;
    }

    public static Image ToThumbnail(this Stream stream, int width, int height)
    {
        var image = Image.FromStream(stream);
        var thumbnail = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);

        return thumbnail;
    }

    public static Image ToThumbnail(this byte[] data, int width, int height)
    {
        return (new MemoryStream(data)).ToThumbnail(width, height);
    }

    public static string AsFileName(this string value)
    {
        value = value?.Replace("/", "-");
        return Path.GetInvalidFileNameChars().Aggregate(value, (current, c) => current.Replace(c.ToString(), string.Empty));
    }

    public static string UnionWKTs(this List<string> wkts)
    {
        if ((wkts?.Count ?? 0) == 0)
        {
            return null;
        }
        var first = wkts.First();
        var result = DbGeography.FromText(first);

        foreach (var wkt in wkts)
        {
            if (wkt == first)
            {
                continue;
            }
            result = result.Union(DbGeography.FromText(wkt));
        }
        return result.WellKnownValue.WellKnownText;
    }

}
