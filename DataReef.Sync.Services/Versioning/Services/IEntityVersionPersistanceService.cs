using System;
using DataReef.Sync.Services.Versioning.Models;

namespace DataReef.Sync.Services.Versioning.Services
{
    public interface IEntityVersionPersistanceService
    {
        void PersistEntityVersion(VersioningEntity versioningEntity);
        VersioningEntity RetreaveEntityVersion(Guid entityId, int version);
    }
}