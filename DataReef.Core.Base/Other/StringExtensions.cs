using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
public static class StringExtensions
{
    /// <summary>
    /// Split the string into First and Last Name
    /// </summary>
    /// <param name="value">Item1 is FirstName, Item2 is LastName</param>
    /// <returns></returns>
    public static Tuple<string, string> FirstAndLastName(this string value)
    {
        if (value == null)
            return null;

        var words = value.Split(' ');
        switch (words.Length)
        {
            case 1:
                // first last
                return new Tuple<string, string>(words[0], "");
            case 2:
                // first last
                return new Tuple<string, string>(words[0], words[1]);
            case 3:
                // first initial last
                return new Tuple<string, string>(words[0], words[2]);
            case 4:
                // first initial last suffix
                return new Tuple<string, string>(words[0], words[2]);
        }
        return new Tuple<string, string>("", "");
    }

    /// <summary>
    /// Method that removes everything after ?
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Everything before the ? character</returns>
    public static string TrimParams(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        return value.Substring(0, (value + "?").IndexOf('?')).ToLowerInvariant();
    }

    /// <summary>
    /// Method that takes a file path and extracts what's between the separator and the extension, including the separator (e.g. _01 from file_01.pdf)
    /// </summary>
    /// <param name="path">Full file path, or just file name</param>
    /// <param name="separator">The string to look for in the file name</param>
    /// <returns>The postfix, including the separator, or string.Empty if the separator is not found</returns>
    public static string GetPostFix(this string path, string separator)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        var charIndex = fileName.IndexOf(separator);
        // if the template name does not contain _, don't add any postFix
        return charIndex >= 0 ? fileName.Substring(charIndex) : string.Empty;
    }

    /// <summary>
    /// Populates the {Property} placeholders w/ the object's properties
    /// </summary>
    /// <param name="format">The string to format</param>
    /// <param name="data">Can be of known or anonymous type</param>
    /// <returns></returns>
    public static string FormatWith(this string format, object data)
    {
        return FormatWith(format, data, null);
    }

    /// <summary>
    /// Populates the {Property} placeholders w/ the object's properties
    /// </summary>
    /// <param name="format">The string to format</param>
    /// <param name="data">Can be of known or anonymous type</param>
    /// <param name="formatProvider">Format provider</param>
    /// <returns></returns>
    public static string FormatWith(this string format, object data, IFormatProvider formatProvider)
    {
        StringBuilder sb = new StringBuilder();
        Type type = data.GetType();
        Regex reg = new Regex(@"({)([^}]+)(})", RegexOptions.IgnoreCase);
        MatchCollection mc = reg.Matches(format);
        int startIndex = 0;
        foreach (Match m in mc)
        {
            Group g = m.Groups[2]; //it's second in the match between { and }
            int length = g.Index - startIndex - 1;
            sb.Append(format.Substring(startIndex, length));

            string toGet = String.Empty;
            string toFormat = String.Empty;
            int formatIndex = g.Value.IndexOf(":"); //formatting would be to the right of a :
            if (formatIndex == -1) //no formatting, no worries
            {
                toGet = g.Value;
            }
            else //pickup the formatting
            {
                toGet = g.Value.Substring(0, formatIndex);
                toFormat = g.Value.Substring(formatIndex + 1);
            }

            //first try properties
            PropertyInfo retrievedProperty = type.GetProperty(toGet);
            Type retrievedType = null;
            object retrievedObject = null;
            if (retrievedProperty != null)
            {
                retrievedType = retrievedProperty.PropertyType;
                retrievedObject = retrievedProperty.GetValue(data, null);
            }
            else //try fields
            {
                FieldInfo retrievedField = type.GetField(toGet);
                if (retrievedField != null)
                {
                    retrievedType = retrievedField.FieldType;
                    retrievedObject = retrievedField.GetValue(data);
                }
            }

            if (retrievedType != null && retrievedObject != null) //Cool, we found something
            {
                string result = String.Empty;
                if (toFormat == String.Empty) //no format info
                {
                    result = retrievedType.InvokeMember("ToString",
                      BindingFlags.Public | BindingFlags.NonPublic |
                      BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                      , null, retrievedObject, null) as string;
                }
                else //format info
                {
                    result = retrievedType.InvokeMember("ToString",
                      BindingFlags.Public | BindingFlags.NonPublic |
                      BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase
                      , null, retrievedObject, new object[] { toFormat, formatProvider }) as string;
                }
                sb.Append(result);
            }
            else //didn't find a property with that name, so be gracious and put it back
            {
                sb.Append("{");
                sb.Append(g.Value);
                sb.Append("}");
            }
            startIndex = g.Index + g.Length + 1;
        }
        if (startIndex < format.Length) //include the rest (end) of the string
        {
            sb.Append(format.Substring(startIndex));
        }
        return sb.ToString();
    }
}
