using System.ServiceModel;
using DataReef.TM.Models;
using System.Collections.Generic;
using System;

namespace DataReef.TM.Contracts.Services
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes", typeof(KnownTypesProvider))]
    public interface ICurrentLocationService : IDataService<CurrentLocation>
    {
        /// <summary>
        /// Select the CurrentLocations for a person by Date
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="date"></param>
        /// <returns>collection of CurrentLocation</returns>
        [OperationContract]
        IEnumerable<CurrentLocation> GetCurrentLocationsForPersonAndDate(Guid personID, System.DateTime date);

        /// <summary>
        /// Get latest locations for given PersonIds
        /// </summary>
        /// <param name="personIds"></param>
        /// <returns></returns>
        [OperationContract]
        ICollection<CurrentLocation> GetLatestLocations(List<Guid> personIds);
    }
}