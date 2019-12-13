using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Core.Common
{
    public class CustomJsonDateFormatConverter : IsoDateTimeConverter
    {
        public CustomJsonDateFormatConverter(string format)
        {
            base.DateTimeFormat = format;
        }
    }
}
