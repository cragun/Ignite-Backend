using System.ServiceModel;

namespace DataReef.TM.Contracts.Services.NoSQL
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]

    public interface INoSQLDataService
    {
        [OperationContract]
        string GetValue(string key, string tableName = null);

        [OperationContract]
        T GetValue<T>(string key, string tableName = null);

        [OperationContract]
        void PutValue(string value, string tableName = null);

        [OperationContract]
        void PutValue<T>(T value, string tableName = null);

        [OperationContract]
        bool IsHealthy();
    }
}
