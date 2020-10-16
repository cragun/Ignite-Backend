using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations;
using DataReef.Integrations.PushNotification;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Reporting.Settings;
using DataReef.TM.Services.InternalServices.Geo;
using DataReef.TM.Services.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class TerritoryService : DataService<Territory>, ITerritoryService
    {
        private readonly IGeoProvider _geoProvider;
        private readonly IAssignmentService _assignmentsService;
        private readonly Lazy<IAPNPushBridge> _pushBridge;
        private readonly Lazy<IDeviceService> _deviceService;
        private readonly Lazy<IGeographyBridge> _geoNewBridge;
        private readonly Lazy<IInquiryService> _inquiryService;
        private readonly Lazy<IOUSettingService> _ouSettingService;

        public TerritoryService(ILogger logger,
            IAssignmentService assignmentsService,
            IGeoProvider geoProvider,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IAPNPushBridge> pushBridge,
            Lazy<IDeviceService> deviceService,
            Lazy<IGeographyBridge> geoNewBridge,
            Lazy<IInquiryService> inquiryService,
            Lazy<IOUSettingService> ouSettingService)
            : base(logger, unitOfWorkFactory)
        {
            _geoProvider = geoProvider;
            _assignmentsService = assignmentsService;
            _pushBridge = pushBridge;
            _deviceService = deviceService;
            _geoNewBridge = geoNewBridge;
            _inquiryService = inquiryService;
            _ouSettingService = ouSettingService;
        }

        public async Task<TerritorySummary> PopulateTerritorySummary(Territory territory, bool includePropertyCount = true)
        {
            if (territory == null) return null;

            using (var dc = new DataContext())
            {
                var territorySummaryTask = dc.Database.SqlQuery<TerritorySummary>("exec proc_TerritoryAnalytics {0}", territory.Guid).FirstAsync();
                var territorySummary = territorySummaryTask.Result;
                territorySummary.TerritoryID = territory.Guid;

                if (includePropertyCount)
                {
                    //  have to do this for existing territories
                    if (!territory.PropertyCount.HasValue)
                    {
                        var dbTerritory = dc.Territories.Include(t => t.Shapes).FirstOrDefault(t => t.Guid == territory.Guid);
                        if (dbTerritory != null)
                        {
                            var propertyCount = GetPropertiesCount(territory);
                            dbTerritory.PropertyCount = propertyCount;
                            dc.SaveChanges();

                            territory.PropertyCount = propertyCount;
                        }
                    }

                    territorySummary.PropertyCount = territory.PropertyCount ?? 0;
                }

                return await Task.FromResult(territorySummary);
            }
        }

        public async Task<ICollection<Territory>> PopulateTerritoriesSummary(ICollection<Territory> territories)
        {
            var tasks = new List<Task<TerritorySummary>>();
            var t = new List<Task>();
            t.AddRange(tasks);

            foreach (var territory in territories)
            {
                var task = PopulateTerritorySummary(territory);
                tasks.Add(task);
            }

            await Task.WhenAll(t);

            var summaries = tasks.Select(tsk => tsk.Result);

            foreach (var territory in territories)
            {
                territory.Summary = summaries.FirstOrDefault(s => s.TerritoryID == territory.Guid);
            }

            return territories;
        }

        public ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForTerritory(Guid territoryId, OUReportingSettings reportSettings, DateTime? specifiedDay)
        {
            var result = new List<InquiryStatisticsForOrganization>();

            var territory = Get(territoryId);
            if(territory != null)
            {
                //if no settings are supplied, try to get them from the db for the ou
                if (reportSettings == null)
                {
                    reportSettings =
                            _ouSettingService
                            .Value
                            .GetSettingsByOUID(territory.OUID)
                            ?.FirstOrDefault(s => s.Name == OUSetting.OU_Reporting_Settings)
                            ?.GetValue<OUReportingSettings>();
                }


                result = _inquiryService.Value.GetInquiryStatisticsForOrganizationTerritories(new List<Guid> { territoryId }, reportSettings.ReportItems, specifiedDay).ToList();
            }

            return result;
        }

        public override Territory Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            Territory territory = base.Get(uniqueId, include, exclude, fields, deletedItems);
            if (territory == null) return null;

            territory.Summary = PopulateTerritorySummary(territory).Result;

            if (include.IndexOf("OU", StringComparison.OrdinalIgnoreCase) >= 0 && territory.OU != null)
            {
                OUService.PopulateOUSummary(territory.OU);
            }

            return territory;
        }

        public ICollection<Territory> GetForCurrentUserAndOU(Guid ouID, Guid personID, string include = "", string exclude = "", string fields = "")
        {
            IEnumerable<Guid> territoryIds;
            using (DataContext dc = new DataContext())
            {
                territoryIds = dc.Territories.Where(t => t.OUID == ouID && t.Assignments.Any(ta => ta.PersonID == personID) && !t.IsArchived).Select(t => t.Guid).ToList();
            }
            if (territoryIds == null || !territoryIds.Any()) return new List<Territory>();
            var territories = base.GetMany(territoryIds, include, exclude, fields);
            if (territories == null) return new List<Territory>();

            territories = PopulateTerritoriesSummary(territories).Result;

            foreach (var territory in territories)
            {
                if (include.IndexOf("OU", StringComparison.OrdinalIgnoreCase) >= 0 && territory.OU != null)
                {
                    OUService.PopulateOUSummary(territory.OU);
                }
            }

            return territories;
        }

        public override ICollection<Territory> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            var territories = base
                    .List(deletedItems, pageNumber, itemsPerPage, filter, include, exclude, fields)
                    .Where(t => !t.IsArchived) as ICollection<Territory>;


            territories = PopulateTerritoriesSummary(territories).Result;

            foreach (Territory territory in territories)
            {
                if (include.IndexOf("OU", StringComparison.OrdinalIgnoreCase) >= 0 && territory.OU != null)
                {
                    OUService.PopulateOUSummary(territory.OU);
                }
            }

            return territories;
        }

        public ICollection<Territory> GetByShapesVersion(Guid ouid, Guid? personID, ICollection<TerritoryShapeVersion> territoryShapeVersions, bool deletedItems = false, string include = "")
        {

            using (var context = new DataContext())
            {
                var ousQuery = context.OUs.FirstOrDefault(t => t.Guid == ouid);
                var ouTerritoriesQuery = context.Territories.Where(t => t.OUID == ouid);

                if (personID.HasValue)
                {
                    if (!context.OUAssociations.Any(ou => ou.PersonID == personID && ou.OUID == ouid && !ou.IsDeleted && (ou.RoleType == OURoleType.Owner || ou.RoleType == OURoleType.SuperAdmin)))
                    {
                        ouTerritoriesQuery = ouTerritoriesQuery.Where(t => t.Assignments.Any(ass => ass.PersonID == personID.Value));
                    }
                }

                AssignIncludes(include, ref ouTerritoriesQuery);
                ouTerritoriesQuery = ApplyDeletedFilter(deletedItems, ouTerritoriesQuery);
                var ouTerritories = ouTerritoriesQuery.ToList();

                if (!personID.HasValue)
                {
                    var isSuperAdmin = context.OUAssociations.Include(oa => oa.OURole).Where(oa => !oa.IsDeleted && oa.PersonID == SmartPrincipal.UserId && oa.OURole.RoleType == OURoleType.SuperAdmin).FirstOrDefault();

                    if (isSuperAdmin != null)
                    {
                        personID = SmartPrincipal.UserId;
                    }
                }

                var favouriteTerritories = context.FavouriteTerritories.Where(f => f.PersonID == personID).ToList();
                foreach (var territory in ouTerritories.ToList())
                {
                    var favourite = favouriteTerritories?.FirstOrDefault(s => s.TerritoryID == territory.Guid);

                    if (favourite != null)
                        territory.IsFavourite = true;
                    else
                        territory.IsFavourite = false;

                    if (territory.Status == TerritoryStatus.Master && ousQuery.IsTerritoryAdd == false)
                    {
                        ouTerritories.Remove(territory);
                    }
                }


                if (territoryShapeVersions != null)
                {
                    var territoryIds = territoryShapeVersions.Select(s => s.TerritoryID).ToList();
                    var territoriesShapeQuery = context.Territories.Where(t => territoryIds.Contains(t.Guid));
                    AssignIncludes(include, ref territoriesShapeQuery);
                    territoriesShapeQuery = ApplyDeletedFilter(deletedItems, territoriesShapeQuery);
                    var shapeTerritories = territoriesShapeQuery.ToList();

                    ouTerritories = ouTerritories.Union(shapeTerritories).ToList();

                    foreach (var territory in ouTerritories)
                    {
                        var territoryShape = territoryShapeVersions?.FirstOrDefault(s => s.TerritoryID == territory.Guid);

                        if (territory.ShapesVersion == territoryShape?.Version)
                            territory.WellKnownText = null;
                    }
                }

                //ouTerritories = PopulateTerritoriesSummary(ouTerritories).Result.ToList();

                foreach (var territory in ouTerritories.Where(territory => include.IndexOf("OU", StringComparison.OrdinalIgnoreCase) >= 0 && territory.OU != null))
                {
                    OUService.PopulateOUSummary(territory.OU);
                }

                return ouTerritories;
            }
        }

        public Territory SetArchiveStatus(Guid territoryID, bool archiveStatus)
        {
            using (var repository = UnitOfWorkFactory())
            {
                var territory = repository.Get<Territory>().FirstOrDefault(t => t.Guid == territoryID);
                if (territory != null)
                {
                    territory.IsArchived = archiveStatus;
                    territory.Updated(SmartPrincipal.UserId);

                    repository.Update(territory);
                    repository.SaveChanges();

                    return territory;
                }
                else
                {
                    throw new Exception("Territory with the specified ID not found.");
                }
            }
        }

        private static void ValidateShapes(Territory entity)
        {
            if (entity.Shapes == null || !entity.Shapes.Any())
            {
                throw new Exception("You must select at least one shape");
            }
            else if (entity.Shapes.Select(s => s.ShapeTypeID).Distinct().Count() > 1)
            {
                throw new Exception("All the shapes must be at the same level");
            }
        }

        public override Territory Insert(Territory entity)
        {
            ValidateShapes(entity);
            // The client will start sending the PropertyCount
            // and if they do, there's no point in getting it again
            if (entity.PropertyCount == 0)
            {
                entity.PropertyCount = GetPropertiesCount(entity);
            }

            // Insert User Drawn Shapes in ES.
            // InsertUserDrawnShapesInES(entity);

            Territory ret = base.Insert(entity);

            if (ret.SaveResult.Success)
            {
                // send mail
                entity.Summary = PopulateTerritorySummary(entity).Result;
                var assignments = entity.Assignments;
                if (assignments != null && assignments.Count > 0)
                {
                    _assignmentsService.SendTerritoryAssignmentNotification(assignments);
                }
            }

            // strip the WKT and the Shapes before sending the result back.
            ret.WellKnownText = null;
            ret.Shapes = null;

            return ret;
        }

        public override Territory Update(Territory entity)
        {
            var shapeHasChanged = false;
            ValidateShapes(entity);

            var newAssignments = new Assignment[0];

            using (var dataContext = new DataContext())
            {
                Territory item;
                // retrieve current shapes if the entity has shapes.
                if (entity.Shapes != null)
                {
                    item = dataContext
                            .Territories
                            .Include(t => t.Shapes)
                            .FirstOrDefault(t => t.Guid == entity.Guid);
                }
                else
                {
                    item = dataContext
                            .Territories
                            .FirstOrDefault(t => t.Guid == entity.Guid);
                }

                if (item != null)
                {
                    List<Guid> itemShapeIds = item.Shapes != null ? item.Shapes.Select(s => s.ShapeID).ToList() : new List<Guid>();

                    bool anyEntityShapeChanges = (entity.Shapes != null);

                    List<Guid> entityShapeIds = anyEntityShapeChanges ? entity.Shapes.Select(s => s.ShapeID).ToList() : new List<Guid>();

                    var shapeIds = new HashSet<Guid>(itemShapeIds);

                    if (!shapeIds.SetEquals(entityShapeIds))
                    {
                        shapeHasChanged = true;
                        var fixedWkt = dataContext.Database.SqlQuery<string>(string.Format("SELECT dbo.fixWKT('{0}')", entity.WellKnownText)).FirstOrDefault();

                        var geo = DbGeography.FromText(fixedWkt);

                        // get all deleted properties that belong to this territory
                        // and their location is contained in the Shape's WKT
                        var propertiesToUndelete = dataContext
                                        .Properties
                                        .Include(p => p.Inquiries)
                                        .Where(p => p.IsDeleted == true
                                           && p.Latitude.HasValue
                                           && p.Longitude.HasValue
                                           && p.TerritoryID == entity.Guid
                                           && SqlSpatialFunctions.Filter(geo, SqlSpatialFunctions.PointGeography(p.Latitude, p.Longitude, DbGeography.DefaultCoordinateSystemId)) == true)
                                        .ToList();

                        if (propertiesToUndelete.Count > 0)
                        {
                            foreach (var prop in propertiesToUndelete)
                            {
                                prop.IsDeleted = false;

                                if (prop.Inquiries != null)
                                {
                                    foreach (var inq in prop.Inquiries)
                                    {
                                        inq.IsDeleted = false;
                                    }
                                }
                            }
                            dataContext.SaveChanges();
                        }
                    }
                }

                var existingTerritoryAssignments = dataContext
                                                    .Assignments
                                                    .Where(a => a.TerritoryID == entity.Guid)
                                                    .ToArray();

                if (entity.Assignments != null)
                {
                    newAssignments = entity.Assignments
                                        .Where(newa => !existingTerritoryAssignments.Any(exa => exa.Guid == newa.Guid))
                                        .ToArray();
                }
            }

            if (entity.Shapes != null)
            {
                entity.PropertyCount = GetPropertiesCount(entity);
            }

            var ret = base.Update(entity);

            try
            {
                UpdateNavigationProperties(entity);
            }
            catch (Exception) { }

            if (ret.SaveResult.Success)
            {
                // send mail
                ret.Summary = PopulateTerritorySummary(ret).Result;
                if ((newAssignments != null) && (newAssignments.Length > 0))
                {
                    _assignmentsService.SendTerritoryAssignmentNotification(newAssignments);
                }
            }

            if (shapeHasChanged)
            {
                _deviceService.Value.PushToSubscribers<Territory, Territory>(ret.Guid.ToString(), ret.Guid.ToString(), DataAction.Update, alert: $"Territory {ret.Name} has updated!");
            }

            return ret;
        }

        public override ICollection<SaveResult> DeleteMany(Guid[] uniqueIds)
        {
            var ret = base.DeleteMany(uniqueIds);

            var ids = ret.Select(sr => sr.EntityUniqueId.Value.ToString()).ToList();

            foreach (var id in ids)
            {
                _deviceService.Value.PushToSubscribers<Territory, Territory>(id, id, DataAction.Delete, alert: string.Empty);
            }

            return ret;
        }

        public int GetPropertiesCount(Territory territory)
        {
            int result = 0;
            if (territory == null) return 0;

            var territoryShapes = territory.Shapes;

            if (territoryShapes == null)
            {
                using (var dataContext = new DataContext())
                {
                    // first we get all the properties saved locally that are not from the Geo server
                    result = dataContext.Properties.Count(p => p.TerritoryID == territory.Guid && !p.IsDeleted && string.IsNullOrEmpty(p.ExternalID));

                    territoryShapes = dataContext.TerritoryShapes.Where(ts => ts.TerritoryID == territory.Guid && !ts.IsDeleted).ToList();
                }
            }

            if (territoryShapes.Any())
            {
                // get the properties count from Geo based on WKT because this is a User Drawn Territory.
                if (territoryShapes.Any(ts => !ts.IsDeleted && ts.ShapeTypeID == BaseShape.UserDrawnTerritoryShapeTypeID))
                {
                    result += (int)_geoProvider.PropertiesCountForWKT(territory.WellKnownText);
                }
                else
                {
                    var shapeIds = territoryShapes.Where(s => !s.IsDeleted).Select(s => s.ShapeID).ToList();
                    // we add the properties from the Geo server to the count
                    result += (int)_geoProvider.PropertiesCountInShapeIds(shapeIds);
                }
            }

            return result;
        }

        public long GetPropertiesCountForWKT(string wkt)
        {
            return _geoProvider.PropertiesCountForWKT(wkt);
        }

        private void InsertUserDrawnShapesInES(Territory entity)
        {
            var userDrawnShapes = entity
                                    .Shapes?
                                    .Where(s => s.ShapeTypeID == BaseShape.UserDrawnTerritoryShapeTypeID)?
                                    .ToList();

            if ((userDrawnShapes?.Count ?? 0) == 0)
            {
                return;
            }

            var geoShapes = userDrawnShapes
                        .Select(s => s.ToGeoShape())
                        .ToList();
            _geoNewBridge.Value.InsertShapes(geoShapes);

            foreach (var item in userDrawnShapes)
            {
                item.ExternalID = item.Guid.ToString();
            }
        }


        /// <summary>
        /// This method verifies if there's an territory in this OU, with the given name.
        /// If there is, it will suggest the next one using this pattern: "{OU Name} {Number}"
        /// </summary>
        /// <param name="ouId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SaveResult IsNameAvailable(Guid ouId, string name)
        {
            var result = new SaveResult();
            var lowerName = name.ToLower();

            using (DataContext dc = new DataContext())
            {
                // verify if name is available
                var exists = dc
                    .Territories
                    .Where(t => t.Name != null)
                    .Any(t => t.OUID == ouId && t.Name.ToLower().StartsWith(lowerName));

                result.Success = !exists;

                if (!exists)
                {
                    return result;
                }

                // verify if OUID is valid
                var ou = dc
                    .OUs
                    .FirstOrDefault(o => o.Guid == ouId);

                if (ou == null)
                {
                    result.Exception = "OU does not exist!";
                    return result;
                }

                var ouName = ou.Name + " ";

                // get all the TerritoryNames that start with OUName
                // Will get them in descending order, to get the highest number
                var territories = dc
                    .Territories
                    .Where(t => t.Name != null && t.Name.StartsWith(ouName))
                    .OrderByDescending(t => t.Name)
                    .Select(t => t.Name)
                    .ToList();

                int number = 0;
                // try to parse the number
                foreach (var item in territories)
                {
                    var parts = item.Split(' ');

                    if (parts.Length < 2)
                    {
                        continue;
                    }

                    var stringNumber = parts[parts.Length - 1];

                    if (Int32.TryParse(stringNumber, out number))
                    {
                        break;
                    }
                }
                result.ExceptionMessage = string.Format("{0}{1}", ouName, number + 1);

                return result;
            }
        }

        /// <summary>
        /// This method insert Territory as a Favorite 
        /// </summary>
        public FavouriteTerritory InsertFavoriteTerritory(Guid territoryID, Guid personID)
        {
            using (var dc = new DataContext())
            {
                var FavouriteTerritory = dc.FavouriteTerritories.FirstOrDefault(x => x.PersonID == personID && x.TerritoryID == territoryID);

                if (FavouriteTerritory != null)
                    throw new ApplicationException("Already Favourited");

                var territory = new FavouriteTerritory
                {
                    TerritoryID = territoryID,
                    PersonID = personID,
                    isFavourite = true,
                    CreatedByID = SmartPrincipal.UserId
                };

                dc.FavouriteTerritories.Add(territory);
                dc.SaveChanges();

                return territory;
            }
        }

        /// <summary>
        /// This method remove Territory as a Favorite 
        /// </summary>
        public void RemoveFavoriteTerritory(Guid territoryID, Guid personID)
        {
            using (var dc = new DataContext())
            {
                var FavouriteTerritory = dc.FavouriteTerritories.FirstOrDefault(x => x.PersonID == personID && x.TerritoryID == territoryID);

                if (FavouriteTerritory == null)
                    throw new ApplicationException("Territory not found");

                dc.FavouriteTerritories.Remove(FavouriteTerritory);
                dc.SaveChanges();
            }
        } 
    }
}
