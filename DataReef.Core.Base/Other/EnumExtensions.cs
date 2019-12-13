using DataReef.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Other
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Extension method to retrieve the <see cref="TemplateNameAttribute"/> attribute value for an enum
        /// </summary>
        /// <typeparam name="T">Enum type (usually infered)</typeparam>
        /// <param name="value">enum value</param>
        /// <returns></returns>
        public static string GetTemplateName<T>(this T value) where T : struct, IConvertible
        {
            var type = typeof(T);

            var memInfo = type.GetMember(value.ToString());

            var attributes = memInfo[0].GetCustomAttributes(typeof(TemplateNameAttribute), false);

            if (attributes == null || attributes.Length == 0)
                return null;

            var attr = attributes[0] as TemplateNameAttribute;

            if (attr == null)
                return null;

            return attr.TemplateName;
        }
    }
}
