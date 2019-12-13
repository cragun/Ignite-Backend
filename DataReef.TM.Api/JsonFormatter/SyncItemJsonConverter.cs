using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataReef.TM.Models;
using DataReef.TM.Models.Sync;

namespace DataReef.TM.Api.JsonFormatter
{
    public class SyncItemJsonConverter : JsonConverter
    {
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(SyncItem).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                // Load JObject from stream
                var jObject = JObject.Load(reader);

                var eTypeName = jObject.GetValue("EntityType").Value<string>();

                if (!eTypeName.StartsWith("DataReef.TM.Models."))
                    eTypeName = "DataReef.TM.Models." + eTypeName;

                var t = Type.GetType(eTypeName + ", DataReef.TM.Models");

                // Create target object based on JObject
                var target = new SyncItem();
                if (t != null)
                {
                    target.Entity = Activator.CreateInstance(t) as EntityBase;
                }

                // Populate the object properties
                serializer.Populate(jObject.CreateReader(), target);

                return target;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}