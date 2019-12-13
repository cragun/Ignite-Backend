using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Sync;
using DataReef.TM.Contracts.Services;
using DataReef.Sync.Contracts;
using DataReef.Sync.Contracts.Exceptions;
using DataReef.Sync.Contracts.Models;


namespace DataReef.TM.Api.Controllers
{
    /// <summary>
    /// Syncronization controller for devices offline changes
    /// </summary>
    [RoutePrefix("api/v1/sync")]
    public class SyncController : ApiController
    {
        private readonly ISyncDataService syncDataService; //Core

        private readonly ISyncDeltaService syncDeltaService; //SyncService

        private const int maxDeltas = 1000;  //todo: throttle via configuration

        /// <summary>
        /// Public constructor to be called by the DI container
        /// </summary>
        /// <param name="syncService"></param>
        /// <param name="syncDeltaService"></param>
        public SyncController(ISyncDataService syncService, ISyncDeltaService syncDeltaService)
        {
            this.syncDataService = syncService;
            this.syncDeltaService = syncDeltaService;
        }

        /// <summary>
        /// returns all the data ( not deltas ) for the user. all sync deltas are subsequently removed.  This method should be called when the device has no data, or if the sync server tells the device to do a full sync
        /// </summary>
        /// <returns></returns>
        [Route("~/api/v1/fullsync")]
        [HttpGet]
        public InitDataPacket FullSync()
        {
            syncDeltaService.ClearDeltas(SmartPrincipal.UserId, SmartPrincipal.DeviceId);
            var initDataPackage = syncDataService.GetAll();
            return initDataPackage;
        }

        /// <summary>
        /// Start a sync session.
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpPost]
        public SyncResponse BeginSync()
        {
            //tell sync server prepare (and record) all deltas for a new sync session.  those deltas will be wiped out upon calling "end"
            Trace.TraceInformation("Begin sync session. UserID:" + SmartPrincipal.UserId + " DeviceID: " + SmartPrincipal.DeviceId);
            try
            {
                var session = syncDeltaService.BeginSync(SmartPrincipal.UserId, SmartPrincipal.DeviceId);

                return new SyncResponse
                {
                    SessionGuid = session.Guid,
                    DeltaCount = session.DeltaCount,
                    SyncStatus = SyncStatus.Ok,
                    Message = "New sync session created",
                };
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = "Could not create a valid session"
                });
            }
        }

        /// <summary>
        /// Updates data in the middle of a sync session.  
        /// </summary>
        /// <returns></returns>
        [Route("{sessionGuid:Guid}")]
        [HttpPut]
        public SyncPutResponse Put([FromUri]Guid sessionGuid, [FromBody] SyncPutRequest syncPutRequest)
        {
            try
            {
                Trace.TraceInformation("Begin UpdateSync:PUT session:" + sessionGuid + " UserID: " + SmartPrincipal.UserId + " DeviceID: " + SmartPrincipal.DeviceId);
                var session = syncDeltaService.GetSyncSession(SmartPrincipal.UserId, SmartPrincipal.DeviceId, sessionGuid);

                IEnumerable<SyncItemStatus> syncItemStatuses;


                if (syncPutRequest == null || syncPutRequest.SyncItems == null || !syncPutRequest.SyncItems.Any())
                {
                    Trace.TraceWarning("Empty Sync PUT payload");
                    syncItemStatuses = null;
                }
                else
                {
                    syncItemStatuses = syncDataService.Update(syncPutRequest.SyncItems);
                }

                return new SyncPutResponse
                {
                    SyncStatus = SyncStatus.Ok,
                    SessionGuid = session.Guid,
                    SyncStatuses = syncItemStatuses
                };
            }
            catch (InvalidSessionException)
            {
                Trace.TraceError("Invalid session " + sessionGuid + ". User " + SmartPrincipal.UserId);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    ReasonPhrase = "Invalid session guid",
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates data in the middle of a sync session.  
        /// </summary>
        /// <returns></returns>
        [Route("{sessionGuid:Guid}")]
        [HttpGet]
        public SyncGetResponse Get([FromUri] Guid sessionGuid)
        {
            Trace.TraceInformation("Begin UpdateSync:GET session:" + sessionGuid + " UserID: " + SmartPrincipal.UserId +
                                   " DeviceID: " + SmartPrincipal.DeviceId);
            try
            {
                //todo: future support for pagination
                IEnumerable<Delta> deltas = syncDeltaService.GetDeltas(SmartPrincipal.UserId, SmartPrincipal.DeviceId, sessionGuid, 1, maxDeltas).ToList();

                if (deltas.Count() > maxDeltas)
                    return new SyncGetResponse
                    {
                        SessionGuid = sessionGuid,
                        SyncStatus = SyncStatus.ForceFullSync,
                        Message = "Too many changes. "
                    };

                var returnMessage = new SyncGetResponse
                {
                    SyncStatus = SyncStatus.Ok,
                    SessionGuid = sessionGuid,
                    SyncItems = syncDataService.GetServerSyncPacket(deltas.Select(delta => new SyncItem
                    {
                        DataAction = delta.DataAction,
                        EntityGuid = delta.EntityGuid,
                        EntityType = delta.EntityType,
                    })).ToList()
                };

                return returnMessage;

            }
            catch (InvalidSessionException)
            {
                Trace.TraceError("Invalid session " + sessionGuid + ". User " + SmartPrincipal.UserId);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    ReasonPhrase = "Invalid session guid",
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                });
            }
        }

        /// <summary>
        /// Clear the current session
        /// </summary>
        /// <param name="sessionGuid"></param>
        /// <returns></returns>
        [Route("{sessionGuid:Guid}")]
        [HttpDelete]
        public SyncResponse Delete([FromUri] Guid sessionGuid)
        {
            try
            {
                var success = syncDeltaService.EndSync(SmartPrincipal.UserId, SmartPrincipal.DeviceId, sessionGuid);

                return new SyncResponse
                {
                    SessionGuid = sessionGuid,
                    SyncStatus = success ? SyncStatus.Ok : SyncStatus.Error,
                    DeltaCount = 0
                };
            }
            catch (InvalidSessionException)
            {
                Trace.TraceError("Invalid session " + sessionGuid + ". User " + SmartPrincipal.UserId);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.PreconditionFailed)
                {
                    ReasonPhrase = "Invalid session guid",
                });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message + "|" + ex.StackTrace);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    ReasonPhrase = ex.Message
                });
            }
        }


        #region RequestResponse Helpers

#if DEBUG
        [Route("sample/put")]
        [HttpGet]
        public SyncItem[] GetPutSample()
        {
            return new[]
            {
                new SyncItem
                {
                    DataAction = DataAction.Insert,
                    EntityType = "Note",
                    EntityGuid = new Guid("6EA628A5-CFE8-4204-8C00-B454EF84434A"),
                    Version = 2
                },
                new SyncItem
                {
                    DataAction = DataAction.Insert,
                    EntityType = "Note",
                    EntityGuid = new Guid("1EA628A5-CFE8-4204-8C00-B454EF84434A"),
                    Version = 2
                },
            };
        }
#endif
        #endregion
    }
}
