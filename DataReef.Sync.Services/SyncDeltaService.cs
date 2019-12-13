using System;
using System.Collections.Generic;
using DataReef.Sync.Contracts;
using DataReef.Sync.Contracts.Exceptions;
using DataReef.Sync.Contracts.Models;
using DataReef.Sync.Services.Database;
using System.Linq;

namespace DataReef.Sync.Services
{
    public class SyncDeltaService : ISyncDeltaService
    {
        public SyncSession BeginSync(Guid userGuid, Guid deviceGuid)
        {
            using (var context = new SyncDataContext())
            {
                var openSessions = context.SyncSessions
                    .Where(s => s.UserGuid == userGuid
                                && s.DeviceGuid == deviceGuid
                                && (s.Status == SessionStatus.InProgress || s.Status == SessionStatus.Started));

                foreach (var openSession in openSessions)
                {
                    openSession.Status = SessionStatus.Droped;
                    openSession.DateEnd = DateTime.UtcNow;
                }

                var session = new SyncSession
                    {
                        DateCreated = DateTime.UtcNow,
                        Guid = Guid.NewGuid(),
                        Status = SessionStatus.Started,
                        UserGuid = userGuid,
                        DeviceGuid = deviceGuid,
                        DeltaCount = 0
                    };

                context.SyncSessions.Add(session);

                var deltas = context.Deltas.Where(d => d.UserGuid == userGuid && d.DeviceGuid == deviceGuid);
                foreach (var delta in deltas)
                {
                    delta.SyncSessionGuid = session.Guid;
                    session.DeltaCount++;
                }

                context.SaveChanges();

                return session;
            }
        }

        public IEnumerable<Delta> GetDeltas(Guid userGuid, Guid deviceGuid, Guid sessionGuid, int pageNumber, int itemsPerPage)
        {
            using (var context = new SyncDataContext())
            {
                var session = GetSession(context, userGuid, deviceGuid, sessionGuid);

                var deltas = context.Deltas.Where(d => d.SyncSessionGuid == session.Guid).OrderBy(i => i.Id)
                             .Skip((pageNumber - 1) * itemsPerPage).Take(itemsPerPage).ToList();

                return deltas;
            }
        }

        public bool EndSync(Guid userGuid, Guid deviceGuid, Guid sessionGuid)
        {
            using (var context = new SyncDataContext())
            {
                var session = GetSession(context, userGuid, deviceGuid, sessionGuid);
                session.Status = SessionStatus.Ended;
                session.DateEnd = DateTime.UtcNow;

                var deltas = context.Deltas.Where(d => d.SyncSessionGuid == session.Guid).ToList();
                context.Deltas.RemoveRange(deltas);

                context.SaveChanges();
            }
            return true;
        }

        public int ClearDeltas(Guid userGuid, Guid deviceGuid)
        {
            using (var context = new SyncDataContext())
            {
                //1. Get any existing session for user-device
                var openSessions = context.SyncSessions
                    .Where(s => s.UserGuid == userGuid
                                && s.DeviceGuid == deviceGuid
                                && (s.Status == SessionStatus.InProgress || s.Status == SessionStatus.Started));

                //2. Drop sessions
                foreach (var openSession in openSessions)
                {
                    openSession.Status = SessionStatus.Droped;
                    openSession.DateEnd = DateTime.UtcNow;
                }

                //3. Delete deltas
                var deltas = context.Deltas.Where(s => s.UserGuid == userGuid && s.DeviceGuid == deviceGuid).ToList();
                context.Deltas.RemoveRange(deltas);
                context.SaveChanges();

                return deltas.Count();
            }
        }

        public SyncSession GetSyncSession(Guid userGuid, Guid deviceGuid, Guid sessionGuid)
        {
            using (var context = new SyncDataContext())
            {
                return GetSession(context, userGuid, deviceGuid, sessionGuid);
            }
        }

        private static SyncSession GetSession(SyncDataContext context, Guid userGuid, Guid deviceGuid, Guid sessionGuid)
        {
            var session = context.SyncSessions.FirstOrDefault(s => s.Guid == sessionGuid && s.UserGuid == userGuid
                                                                                           && s.DeviceGuid == deviceGuid
                                                                                           && (s.Status == SessionStatus.Started || s.Status == SessionStatus.InProgress));
            if (session == null)
                throw new InvalidSessionException(sessionGuid);

            session.DeltaCount = context.Deltas.Count(d => d.SyncSessionGuid == session.Guid);
            return session;
        }
    }
}
