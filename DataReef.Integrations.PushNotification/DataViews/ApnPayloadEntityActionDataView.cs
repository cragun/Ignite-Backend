using DataReef.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.Integrations.PushNotification.DataViews
{
    public class ApnPayloadEntityActionDataView : ApnPayloadBaseDataView
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DataAction EntityAction { get; set; }
        public string EntityType { get; set; }
        public string EntityID { get; set; }
        public string EntityParentID { get; set; }

        public ApnPayloadEntityActionDataView()
        { }

        public ApnPayloadEntityActionDataView(DataAction action, string type, string id, string parentId = null, string alert = null)
        {
            EntityAction = action;
            EntityType = type;
            EntityID = id;
            EntityParentID = parentId;
            APS = new ApnAPS(alert);
        }

        public void SetAlert(string alert)
        {
            APS = new ApnAPS(alert);
        }

        public static ApnPayloadEntityActionDataView Create<T>(DataAction action, string id, string parentId = null, string alert = null)
        {
            return new ApnPayloadEntityActionDataView
            {
                EntityAction = action,
                EntityID = id,
                EntityType = typeof(T).Name,
                APS = new ApnAPS(alert),
                EntityParentID = parentId
            };
        }

        public static ApnPayloadEntityActionDataView Update<T>(string id, string parentId = null, string alert = null)
        {
            return Create<T>(DataAction.Update, id, parentId, alert);
        }

        public static ApnPayloadEntityActionDataView Insert<T>(string id, string parentId = null, string alert = null)
        {
            return Create<T>(DataAction.Insert, id, parentId, alert);
        }

        public static ApnPayloadEntityActionDataView Delete<T>(string id, string parentId = null, string alert = null)
        {
            return Create<T>(DataAction.Delete, id, parentId, alert);
        }
    }
}
