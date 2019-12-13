using DataReef.Core.Enums;
using System;

namespace DataReef.Core
{
 
    /// <summary>
    /// Tells the core service to place the object onto the queue specified
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RoutingAttribute : System.Attribute
    {
        public RoutingAttribute()
            : base()
        {

        }

        public CrudAction CrudAction { get; set; }

        public string ErrorQueueName { get; set; }

        public string Payload { get; set; }

        public string NextHopQueueName { get; set; }

        public string QueueName { get; set; }

    }
}
