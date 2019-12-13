using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Queue;

namespace DataReef.Queues.QueuesCore
{
    internal class DebuggerQueue : IQueue
    {
        const string Message = "A call to a queue was attempted but queue is in debug mode. ";
        
        public CloudQueueMessage GetMessage(TimeSpan? visibilityTimeout)
        {
            Trace.TraceInformation(Message + " GetMessage");
            return new CloudQueueMessage("Debug message");
        }

        public IEnumerable<CloudQueueMessage> GetMessages(int numberOfMessages, TimeSpan? timeSpan)
        {

            Trace.TraceInformation(Message + " GetMessages");
            return new List<CloudQueueMessage> { new CloudQueueMessage("Debug message") };
        }

        public void AddMessage(CloudQueueMessage message)
        {
            Trace.TraceInformation(Message + " AddMessage");
        }

        public void DeleteMessage(CloudQueueMessage message)
        {
            Trace.TraceInformation(Message + " DeleteMessage");
        }

        public CloudQueueMessage PeekMessage()
        {
            Trace.TraceInformation(Message + " PeekMessage");
            return null;
        }
    }
}