using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DataReef.Core.Other
{
    public class EmptyStringToJsonDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return Guid.Empty;
            }
            try
            {
                var value = reader.Value as string;
                if (Guid.TryParse(value, out Guid result))
                {
                    return result;
                }
            }
            catch { }

            return Guid.Empty;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(value);
            t.WriteTo(writer);
        }
    }
}
