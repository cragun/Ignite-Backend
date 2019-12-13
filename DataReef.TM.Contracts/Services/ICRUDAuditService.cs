using DataReef.TM.Models;
using System;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]

    public interface ICRUDAuditService : IDataService<CRUDAudit>
    {

        void UpdateEntity<T>(T original, T updated, string action) where T : EntityBase;

        void UpdateValue(Guid guid, string name, string entityName, string originalValue, string newValue, string action);

        void DeleteEntity<T>(T entity, string action) where T : EntityBase;

        void DeleteEntities<T>(T[] entities, string action) where T : EntityBase;
    }
}
