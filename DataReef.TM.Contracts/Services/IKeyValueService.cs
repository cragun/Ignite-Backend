using System.ServiceModel;
using DataReef.TM.Models;
using System.Runtime.Serialization;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface IKeyValueService : IDataService<KeyValue>
    {
        /// <summary>
        /// Updates or inserts the KeyValue. Uses the Key and the ObjectID as a compound key
        /// </summary>
        /// <param name="kv"></param>
        /// <returns></returns>
        [OperationContract]
        KeyValue Upsert(KeyValue kv);

    }
}