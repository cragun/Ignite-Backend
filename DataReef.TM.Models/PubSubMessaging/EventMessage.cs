using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReef.TM.Models.PubSubMessaging
{
    public class EventMessage
    {
        /// <summary>
        /// Name of the entity type that triggered the event. We'll use this property to link to the OU Setting
        /// </summary>
        public string EventSource { get; set; }

        public EntityBase EventEntity { get; set; }

        public EventActionType EventAction { get; set; }

        public Guid? OUID { get; set; }

        public Guid? EventEntityGuid { get; set; }

        /// <summary>
        /// If we already have the settings loaded, we'll reuse them and not retrieve them from DB again
        /// </summary>
        public List<OUSetting> OUSettings { get; set; }

        public string PayloadJSON { get; set; }

    }
}
