using DataReef.Core.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace DataReef.Core
{
    public class GuidRefJsonConverter : JsonConverter
    {
        private readonly bool includeObjectTypeInformation;

        public GuidRefJsonConverter(bool includeObjectTypeInformation = false)
        {
            this.includeObjectTypeInformation = includeObjectTypeInformation;
        }

        public override bool CanRead { get { return false; } }
        public override bool CanWrite { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DbEntity).IsAssignableFrom(objectType);
        }

        private HashSet<int> serializedObjects = new HashSet<int>();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (serializedObjects.Count() > 5000) serializedObjects.Clear();

            var entity = value as DbEntity;
            if (entity == null) return;

            JObject jo = new JObject();

            if (includeObjectTypeInformation)
            {
                // needed to deserialize into abstract/interface/base types
                // TODO the out of the box converters that come with Newtonsoft take this option into account move logic to a BaseCustomJsonConverter and user it in all the custom converters
                jo.Add("$type", value.GetType().FullName + ", " + value.GetType().Assembly.GetName().Name);
            }

            jo.Add("Guid", entity.Guid.ToString());
            jo.Add("Discriminator", entity.GetType().Name);

            if (serializedObjects.Add(entity.GetHashCode()))
            {
                foreach (PropertyInfo prop in value.GetType().GetProperties())
                {
                    if (prop.GetCustomAttribute<JsonIgnoreAttribute>() != null ||
                       !entity.ShouldSerializePropertyNamed(prop.Name))
                    {
                        continue;
                    }

                    if (prop.CanRead)
                    {
                        object propVal = prop.GetValue(value);

                        if (propVal != null && prop.Name != "Guid")
                        {
                            jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
                        }
                    }
                }

            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}