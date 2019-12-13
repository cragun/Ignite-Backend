using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Newtonsoft.Json;
using DataReef.Core;
using DataReef.Queues.Exceptions;
using Microsoft.Azure;

namespace DataReef.Queues.QueuesCore
{
    public abstract class QueueService<T> : IQueueService<T> where T : BaseQueueMessage, new()
    {
        private readonly Lazy<IQueue> queue;
        private readonly short queueServiceTimeoutSecconds;
        private readonly string queueStorageConnectionString;
        private readonly string queueName;

        protected virtual byte QueuePoisonMessageDequeCount { get { return 10; } }

        /// <summary>
        /// The queue service constructor
        /// </summary>
        /// <param name="queueName">The name of the queue</param>
        /// <param name="queueStorageConnectionString">The connection string pointing to the azure storage and account</param>
        /// <param name="queueServiceTimeoutSecconds">The timeout for initializing and starting interaction with the queue. Default is 5 seconds. Note: Not the operation timeout, just connection establishment.</param>
        protected QueueService(string queueName, string queueStorageConnectionString, short queueServiceTimeoutSecconds = 5)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new QueueServiceInitializationException("Queue name cannot be empty");

            if (string.IsNullOrWhiteSpace(queueStorageConnectionString))
                throw new QueueServiceInitializationException("Queue storage connection string cannot be empty");

            this.queueName = queueName;
            this.queueServiceTimeoutSecconds = queueServiceTimeoutSecconds;
            this.queueStorageConnectionString = queueStorageConnectionString;

            this.queue = new Lazy<IQueue>(InitializeQueue);
        }

        private IQueue InitializeQueue()
        {
            try
            {
                var connectionString = CloudConfigurationManager.GetSetting(queueStorageConnectionString);

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var cloudQueue = queueClient.GetQueueReference(queueName.ToLower(CultureInfo.InvariantCulture));

                cloudQueue.CreateIfNotExists(new QueueRequestOptions { RetryPolicy = new NoRetry(), ServerTimeout = new TimeSpan(0, 0, queueServiceTimeoutSecconds) });

                return new AzureQueue(cloudQueue);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to initialize queue! (name: " + queueName + ", connectionString: " + queueStorageConnectionString, ex);
#if DEBUG
                return new DebuggerQueue();
#endif
                throw;
            }
        }

        public P Converte<P>(IDictionary<string, object> dictionary) where P : class
        {
            var bindings = new List<MemberBinding>();
            foreach (var sourceProperty in typeof(P).GetProperties().Where(x => x.CanWrite))
            {
                var key = dictionary.Keys.SingleOrDefault(x => x.Equals(sourceProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(key)) continue;
                var propertyValue = dictionary[key];
                bindings.Add(Expression.Bind(sourceProperty, Expression.Constant(propertyValue)));
            }
            Expression memberInit = Expression.MemberInit(Expression.New(typeof(P)), bindings);
            return Expression.Lambda<Func<P>>(memberInit).Compile().Invoke();
        }

        public void Enqueue(T message)
        {
            var stringSerialized = SerializeString(message);

            var msg = string.IsNullOrWhiteSpace(stringSerialized) ?
                new CloudQueueMessage(SerializeBinary(message)) :
                new CloudQueueMessage(stringSerialized);

            queue.Value.AddMessage(msg);
        }

        public IEnumerable<QueueMessage<T>> DequeueMany(int invisibilityTimeoutSeconds = 30, int numberOfMessages = 20)
        {
            var messages = queue.Value.GetMessages(numberOfMessages, new TimeSpan(0, 0, invisibilityTimeoutSeconds));
            return messages.Select(m => new QueueMessage<T>(m, DeserializeFromMessage(m))).ToArray();
        }

        public QueueMessage<T> Dequeue(int invisibilityTimeoutSeconds = 30)
        {
            var message = queue.Value.GetMessage(TimeSpan.FromSeconds(invisibilityTimeoutSeconds));
            return new QueueMessage<T>(message, DeserializeFromMessage(message));
        }

        public void Delete(params QueueMessage<T>[] msgs)
        {
            DeleteMessage(msgs.Select(m => m.CloudQueueMessage).ToArray());
        }

        public void DeleteMessage(params CloudQueueMessage[] msgs)
        {
            foreach (var msg in msgs)
                queue.Value.DeleteMessage(msg);
        }

        public int DequeueAndProcess(Func<QueueMessage<T>, bool> processMessage, int invisibilityTimeoutSeconds = 30, int numberOfMessages = 20)
        {
            var messages = queue.Value.GetMessages(numberOfMessages, new TimeSpan(0, 0, invisibilityTimeoutSeconds)).ToList();

            var messageCount = 0;

            foreach (var message in messages)
            {
                try
                {
                    var messageData = DeserializeFromMessage(message);
                    if (processMessage(new QueueMessage<T>(message, messageData)))
                        DeleteMessage(message);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("DequeueAndProcess " + ex.Message + "|" + ex.StackTrace);
                    if (message.DequeueCount < QueuePoisonMessageDequeCount) throw;
                    // This message is queue poison remove it
                    DeleteMessage(message);
                    Trace.TraceError("Encountered queue poison! Message permanently removed from queue " + message.AsString);

                    throw;
                }
                messageCount++;
            }
            Trace.TraceInformation("Processed " + messageCount + " messages");
            return messageCount;
        }

        public bool HasMessages()
        {
            return this.queue.Value.PeekMessage() != null;
        }

        /// <summary>
        /// If it returns non empty string the serialization output will be used.
        /// This serialization method has top priority. 
        /// </summary>
        /// <param name="message">The serialization contract</param>
        /// <returns>A string containing the serialized object</returns>
        protected virtual string SerializeString(T message)
        {
            return JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new GuidRefJsonConverter(true) },
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        /// <summary>
        /// If it returns a non nullable object it will be used as the serialization provider.
        /// </summary>
        /// <param name="message">The serialization contract. Make sure all entities are realizable.</param>
        /// <returns>A binary array containing the serialized object</returns>
        protected virtual byte[] SerializeBinary(T message)
        {
            var bf = new BinaryFormatter();
            byte[] output;
            using (var ms = new MemoryStream())
            {
                ms.Position = 0;
                bf.Serialize(ms, message);
                output = ms.GetBuffer();
            }
            return output;
        }

        /// <summary>
        /// Deserialize the queue message according to the serialization logic.
        /// </summary>
        /// <param name="message">The <see cref="CloudQueueMessage"/> received by the queue.</param>
        /// <returns>The deserialized result.</returns>
        protected virtual T DeserializeFromMessage(CloudQueueMessage message)
        {
            return JsonConvert.DeserializeObject<T>(message.AsString, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            /*byte[] buffer = message.AsBytes;
            T returnValue;
            using (var ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                var bf = new BinaryFormatter();
                returnValue = (T)bf.Deserialize(ms);
            }
            return returnValue;*/
        }
    }
}