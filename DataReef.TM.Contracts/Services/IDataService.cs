using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace DataReef.TM.Contracts.Services
{
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    [ServiceContract]
    public interface IDataService<T> where T : EntityBase
    {
        [OperationContract]
        [AnonymousAccess]
        ICollection<T> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "");

        [OperationContract]
        [AnonymousAccess]
        T Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false);

        [OperationContract]
        ICollection<T> GetMany(IEnumerable<Guid> uniqueIds, string include = "", string exclude = "", string fields = "", bool deletedItems = false);

        [OperationContract]
        ICollection<Guid> GetDeletedIDs(IEnumerable<Guid> uniqueIds);

        /// <summary>
        /// Always adds a new entity of type T to the database.
        /// </summary>
        [OperationContract]
        T Insert(T entity);

        /// <summary>
        /// Always adds the list of entities as new objects to the database.
        /// </summary>
        [OperationContract]
        ICollection<T> InsertMany(ICollection<T> entities);

        /// <summary>
        /// Always updates by overwriting an existing entity of type T.
        /// </summary>
        [OperationContract]
        T Update(T entity);

        [OperationContract]
        ICollection<T> UpdateMany(ICollection<T> entities);

        [OperationContract]
        SaveResult Delete(Guid uniqueId);

        [OperationContract]
        ICollection<SaveResult> DeleteMany(Guid[] uniqueIds);

        [OperationContract]
        SaveResult Activate(Guid uniqueId);

        [OperationContract]
        ICollection<SaveResult> ActivateMany(Guid[] uniqueIds);

    }
}