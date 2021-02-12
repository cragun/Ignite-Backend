using System;
using System.Linq;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.DataAccess.Database;
using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using System.ServiceModel.Activation;
using System.ServiceModel;
using DataReef.Core.Services;
using System.Collections.Generic;
using DataReef.Core.Infrastructure.Repository;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class MediaItemService : DataService<MediaItem>, IMediaItemService
    {

        public MediaItemService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
        }

        public override MediaItem Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            // first try to retrieve an OUMediaItem that matches the uniqueId.
            // if we find one, replace the uniqueId w/ the MediaId.
            using (var dataContext = new DataContext())
            {
                var oumi = dataContext
                            .OUMediaItems.AsNoTracking()
                            .FirstOrDefault(omi => omi.Guid == uniqueId);
                if (oumi != null)
                {
                    uniqueId = oumi.MediaID;
                }
            }

            return base.Get(uniqueId, include, exclude, fields, deletedItems);
        }

        public override ICollection<MediaItem> GetMany(IEnumerable<Guid> uniqueIds, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            using (var dataContext = new DataContext())
            {
                var ids = dataContext
                            .OUMediaItems
                            .Where(omi => uniqueIds.Contains(omi.Guid))
                            .Select(omi => omi.MediaID)
                            .ToList();

                if (ids.Count > 0)
                {
                    uniqueIds = ids;
                }
            }
            return base.GetMany(uniqueIds, include, exclude, fields, deletedItems);
        }

        public override System.Collections.Generic.ICollection<MediaItem> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {

            Guid rootOUID = SmartPrincipal.OuId;

            return GetMediaItems(rootOUID, deletedItems);
        }

        public override MediaItem Insert(MediaItem entity)
        {
            var resultData = base.Insert(entity);

            using (var dataContext = new DataContext())
            {
                var ouMediaItem = new OUMediaItem { OUID = resultData.OUID, MediaID = resultData.Guid };
                dataContext.OUMediaItems.Add(ouMediaItem);
                dataContext.SaveChanges();
            }

            return resultData;
        }

        /// <summary>
        /// If type == -1, it will try to get the media items separately, for each media type.
        /// If it doesn't find any media item for current OUID, it will try to get from the parent, and so on.
        /// </summary>
        /// <param name="ouID"></param>
        /// <param name="deletedItems"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ICollection<MediaItem> GetMediaItems(Guid ouID, bool deletedItems = false, int type = -1)
        {
            var ret = new List<MediaItem>();

            var mediaTypes = Enum
                                .GetValues(typeof(MediaType))
                                .Cast<int>()
                                .ToDictionary(v => v, v => false);

            using (DataContext dc = new DataContext())
            {
                var allAncestorIDs = dc
                                .Database
                                .SqlQuery<Guid>($"select * from OUTreeUP('{ouID}')")
                                .ToList();

                foreach (var id in allAncestorIDs)
                {
                    if (type < 0)
                    {
                        var mediaItemsLeft = mediaTypes
                                                .Where(m => !m.Value)
                                                .ToList();

                        foreach (var mediaType in mediaItemsLeft)
                        {
                            var items = dc
                                .Database
                                .SqlQuery<MediaItem>("exec proc_OUMediaItems {0}, {1}, {2}", id, deletedItems ? "1" : "0", mediaType.Key)
                                .ToList();

                            if (items.Count > 0)
                            {
                                mediaTypes[mediaType.Key] = true;
                                ret.AddRange(items);
                            }
                        }

                        if (!mediaTypes.Any(m => !m.Value))
                        {
                            break;
                        }
                    }
                    else
                    {

                        var data = dc
                            .Database
                            .SqlQuery<MediaItem>("exec proc_OUMediaItems {0}, {1}, {2}", id, deletedItems ? "1" : "0", type)
                            .ToList();

                        ret.AddRange(data);

                        if (ret.Count > 0)
                        {
                            break;
                        }
                    }
                }

            }

            return ret;
        }



    }
}
