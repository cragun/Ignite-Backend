using DataReef.Core.Enums;
using System;
using System.Text.RegularExpressions;

public static class EnumExtensions
{
    public static string SplitCamelCase(this Enum e)
    {
        if (e == null)
            return null;

        var value = e.ToString();

        return string.Join(" ", Regex.Split(value, @"(?<!^)(?=[A-Z])"));
    }

    public static DeviceType GetDeviceType(this string userAgent)
    {
        var nativeApp = userAgent?.IndexOf("Legion") == 0 || userAgent?.IndexOf("Ignite") == 0;
        var isiPhone = userAgent?.IndexOf("(iPhone;") > 0;
        var isiPad = userAgent?.IndexOf("(iPad;") > 0;
        var deviceType = DeviceType.Unknown;
        if (nativeApp)
        {
            deviceType = isiPhone ? DeviceType.iPhone : isiPad ? DeviceType.iPad : DeviceType.Unknown;
        }
        else
        {
            deviceType = DeviceType.Web;
        }
        return deviceType;
    }
}
