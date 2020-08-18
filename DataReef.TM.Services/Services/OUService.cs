using CsvHelper;
using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.DataAccess.Repository.StoredProcedures;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DataViews.OnBoarding;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.OUs;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using DataReef.TM.Models.PubSubMessaging;
using DataReef.TM.Models.Reporting.Settings;
using DataReef.TM.Services.Extensions;
using DataReef.TM.Services.InternalServices.Geo;
using DataReef.TM.Services.InternalServices.Settings.EventHandlers;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services
{
    // todo: move the whole OU hierarchy and users to a graph database and refactor this mess...

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class OUService : DataService<OU>, IOUService
    {

        private readonly IOUAssociationService _associationService;
        private readonly IFinancePlanDefinitionService _financePlanDefinitionService;
        private readonly ITerritoryService _territoryService;
        private readonly IInquiryService _inquiryService;
        private readonly IGeoProvider _geoProvider;
        private readonly Lazy<IGeographyBridge> _geoBridge;
        private readonly Lazy<IUserInvitationService> _userInvitationService;
        private readonly Lazy<IOUSettingService> _settingsService;
        private readonly Lazy<IDataContextFactory> _dataContextFactory;
        private readonly Lazy<BlobService> _blobService;
        private readonly Lazy<ICRUDAuditService> _auditService;
        private readonly Lazy<IPersonKPIService> _personKPIService = null;

        public OUService(ILogger logger,
                        IOUAssociationService associationService,
                        ITerritoryService territoryService,
                        IInquiryService inquiryService,
                        IFinancePlanDefinitionService financePlanDefinitionService,
                        IGeoProvider geoProvider,
                        Lazy<IGeographyBridge> geoBridge,
                        Lazy<IUserInvitationService> userInvitationService,
                        Lazy<IOUSettingService> settingsService,
                        Lazy<BlobService> blobService,
                        Lazy<IDataContextFactory> dataContextFactory,
                        Func<IUnitOfWork> unitOfWorkFactory,
                        Lazy<ICRUDAuditService> auditService,
                        Lazy<IPersonKPIService> personKPIService) : base(logger, unitOfWorkFactory)
        {
            _associationService = associationService;
            _territoryService = territoryService;
            _inquiryService = inquiryService;
            _financePlanDefinitionService = financePlanDefinitionService;
            _geoProvider = geoProvider;
            _geoBridge = geoBridge;
            _userInvitationService = userInvitationService;
            _settingsService = settingsService;
            _blobService = blobService;
            _dataContextFactory = dataContextFactory;
            _auditService = auditService;
            _personKPIService = personKPIService;
        }

        /// <summary>
        /// Method for deleting CustomFields and CustomValues.
        /// </summary>
        /// <typeparam name="F">A deletable entity.</typeparam>
        /// <param name="uniqueIds">The array of the deletable entities id's. </param>
        /// <returns>List of SaveReults.</returns>
        internal ICollection<SaveResult> DeleteCustomFieldsOrValues<F>(Guid[] uniqueIds) where F : EntityBase
        {
            var results = new List<SaveResult>();

            try
            {
                using (var dataContext = new DataContext())
                {
                    SaveResult result;
                    var set = dataContext.Set<F>();
                    var entities = set.Where(eb => uniqueIds.Contains(eb.Guid)).ToArray();

                    foreach (var uniqueId in uniqueIds)
                    {
                        var entity = entities.FirstOrDefault(e => e.Guid == uniqueId);

                        if (entity != null)
                        {
                            entity.IsDeleted = true;

                            result = SaveResult.SuccessfulDeletion;
                            result.EntityUniqueId = uniqueId;
                        }
                        else
                        {
                            result = new SaveResult
                            {
                                Action = DataAction.None,
                                EntityUniqueId = uniqueId,
                                Success = false
                            };
                        }
                        results.Add(result);
                    }

                    dataContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                foreach (var saveResult in results)
                {
                    saveResult.Success = false;
                }

                results.Add(SaveResult.FromException(ex, DataAction.Delete));
            }

            return results;
        }

        internal static void PopulateOUSummary(OU ou)
        {
            using (DataContext dc = new DataContext())
            {
                OUSummary summary = dc.Database.SqlQuery<OUSummary>("exec proc_OUAnalytics {0}", ou.Guid).First();
                ou.Summary = summary;
            }
        }

        public ICollection<InquiryStatisticsForOrganization> GetInquiryStatisticsForOrganization(Guid ouId, OUReportingSettings reportSettings, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay, IEnumerable<Guid> excludedReps = null)
        {
            var result = new List<InquiryStatisticsForOrganization>();

            //if no settings are supplied, try to get them from the db for the ou 
            if (reportSettings == null)
            {
                reportSettings =
                        _settingsService
                        .Value
                        .GetSettingsByOUID(ouId)
                        ?.FirstOrDefault(s => s.Name == OUSetting.OU_Reporting_Settings)
                        ?.GetValue<OUReportingSettings>();
            }

            if (reportSettings == null)
            {
                return result;
            }

            List<Guid> territoryIds;
            using (DataContext dc = new DataContext())
            {
                // getting inquiry statistics for the specified OU and all it's sub-OUs
                var ouIds = GetOUAndChildrenGuids(ouId);
                territoryIds = dc
                            .Territories
                            .Where(t => ouIds.Contains(t.OUID)
                                    && !t.IsDeleted
                                    && !t.IsArchived)
                            .Select(t => t.Guid)
                            .ToList();
                //var children = string.Join(",", ouIds);
                //var terrIds = string.Join(",", territoryIds);
            }
            if (!territoryIds.Any())
            {
                foreach (var repItem in reportSettings.ReportItems)
                {
                    result.Add(new InquiryStatisticsForOrganization { Name = repItem.ColumnName, Actions = new InquiryStatisticsByDate(), People = new InquiryStatisticsByDate() });
                }

                return result;
            }

            result = _inquiryService.GetInquiryStatisticsForOrganizationTerritories(territoryIds, reportSettings.ReportItems, specifiedDay, StartRangeDay, EndRangeDay, excludedReps).ToList();
            return result;
        }

        public ICollection<InquiryStatisticsForPerson> GetInquiryStatisticsForSalesPeople(Guid ouId, OUReportingSettings reportSettings, DateTime? specifiedDay, DateTime? StartRangeDay, DateTime? EndRangeDay, IEnumerable<Guid> excludedReps = null)
        {
            var result = new List<InquiryStatisticsForPerson>();

            //if no settings are supplied, try to get them from the db for the ou
            if (reportSettings == null)
            {
                reportSettings =
                        _settingsService
                        .Value
                        .GetSettingsByOUID(ouId)
                        ?.FirstOrDefault(s => s.Name == OUSetting.OU_Reporting_Settings)
                        ?.GetValue<OUReportingSettings>();
            }

            if (reportSettings == null)
            {
                return result;
            }

            var ouIds = GetOUAndChildrenGuids(ouId);
            if (!ouIds.Any()) return result;

            List<Guid> territoryIds;
            List<Guid> associatedPersonIds;
            using (DataContext dc = new DataContext())
            {
                territoryIds = dc.Territories.Where(t => ouIds.Contains(t.OUID) && !t.IsArchived).Select(t => t.Guid).ToList();
                associatedPersonIds = dc.OUAssociations.Where(a => !a.IsDeleted && ouIds.Contains(a.OUID)).Select(a => a.PersonID).ToList();
            }

            //return empty result if there's nothing to show for it
            if (territoryIds?.Any() != true && associatedPersonIds?.Any() != true)
            {
                foreach (var repItem in reportSettings.PersonReportItems)
                {
                    result.Add(new InquiryStatisticsForPerson { Name = repItem.ColumnName, Actions = new InquiryStatisticsByDate(), DaysActive = new InquiryStatisticsByDate() });
                }

                return result;
            }

            if (associatedPersonIds?.Any() == true)
            {
                result = _personKPIService.Value.GetSelfTrackedStatisticsForSalesPeopleTerritories(associatedPersonIds, reportSettings.PersonReportItems, specifiedDay, StartRangeDay, EndRangeDay, excludedReps).ToList();
            }

            if (territoryIds?.Any() == true)
            {
                var inquiries = _inquiryService.GetInquiryStatisticsForSalesPeopleTerritories(territoryIds, reportSettings.PersonReportItems, specifiedDay, StartRangeDay, EndRangeDay, excludedReps).ToList();

                foreach (var inquiryResult in inquiries)
                {
                    var matchingItem = result.FirstOrDefault(x => x.PersonId == inquiryResult.PersonId && x.Name == inquiryResult.Name);
                    //data for the same column exists. merge results
                    if (matchingItem != null)
                    {
                        matchingItem.Actions.AllTime += inquiryResult.Actions.AllTime;
                        matchingItem.Actions.Today += inquiryResult.Actions.Today;
                        matchingItem.Actions.ThisWeek += inquiryResult.Actions.ThisWeek;
                        matchingItem.Actions.ThisMonth += inquiryResult.Actions.ThisMonth;
                        matchingItem.Actions.ThisYear += inquiryResult.Actions.ThisYear;
                        matchingItem.Actions.SpecifiedDay += inquiryResult.Actions.SpecifiedDay;
                        matchingItem.Actions.RangeDay += inquiryResult.Actions.RangeDay;
                        matchingItem.Actions.ThisQuarter += inquiryResult.Actions.ThisQuarter;

                        matchingItem.DaysActive.AllTime = Math.Max(matchingItem.DaysActive.AllTime, inquiryResult.DaysActive.AllTime);
                        matchingItem.DaysActive.Today = Math.Max(matchingItem.DaysActive.Today, inquiryResult.DaysActive.Today);
                        matchingItem.DaysActive.ThisWeek = Math.Max(matchingItem.DaysActive.ThisWeek, inquiryResult.DaysActive.ThisWeek);
                        matchingItem.DaysActive.ThisMonth = Math.Max(matchingItem.DaysActive.ThisMonth, inquiryResult.DaysActive.ThisMonth);
                        matchingItem.DaysActive.ThisYear = Math.Max(matchingItem.DaysActive.ThisYear, inquiryResult.DaysActive.ThisYear);
                        matchingItem.DaysActive.SpecifiedDay = Math.Max(matchingItem.DaysActive.SpecifiedDay, inquiryResult.DaysActive.SpecifiedDay);
                        matchingItem.DaysActive.RangeDay = Math.Max(matchingItem.DaysActive.SpecifiedDay, inquiryResult.DaysActive.RangeDay);
                        matchingItem.DaysActive.ThisQuarter = Math.Max(matchingItem.DaysActive.SpecifiedDay, inquiryResult.DaysActive.ThisQuarter);
                    }
                    else
                    {
                        //data for the column does not exist. add it directly to the result
                        result.Add(inquiryResult);
                    }
                }
            }

            return result;
        }


        public OU OUBuilder(OU ou, string include = "", string exclude = "", string fields = "", bool ancestors = false, bool includeDeleted = false)
        {
            PopulateOUSummary(ou);

            if (include.IndexOf("territories", StringComparison.OrdinalIgnoreCase) >= 0
                && ou.Territories != null
                && ou.Territories.Count > 0)
            {
                ou.Territories = ou.Territories.Where(t => !t.IsArchived).ToList();
                foreach (var item in ou.Territories)
                {
                    item.Summary = _territoryService.PopulateTerritorySummary(item).Result;
                }
            }

            var includeItems = include.Split(",".ToCharArray());

            var settingsOUIDs = new List<Guid>();
            var needToAddChildrenSettings = includeItems.Contains("Children.Settings", StringComparer.OrdinalIgnoreCase) && ou.Children != null;

            if (includeItems.Contains("settings", StringComparer.OrdinalIgnoreCase))
            {
                settingsOUIDs.Add(ou.Guid);
            }
            if (needToAddChildrenSettings)
            {
                settingsOUIDs.AddRange(ou.Children.Select(c => c.Guid));
            }
            if (settingsOUIDs.Any())
            {
                var allSettings = _settingsService.Value.GetOuSettingsMany(settingsOUIDs);
                ou.Settings = allSettings.Find(ou.Guid);

                if (needToAddChildrenSettings)
                {
                    foreach (var child in ou.Children)
                    {
                        child.Settings = allSettings.Find(child.Guid);
                    }
                }
            }

            if (ancestors)
            {
                ou.Ancestors = GetAncestors(ou.Guid);
            }

            return ou;
        }

        public ICollection<OUAssociation> PopulateAssociationsOUs(ICollection<OUAssociation> associations)
        {
            foreach (var association in associations)
            {
                if (association.OU != null)
                {
                    association.OU = OUBuilder(association.OU);
                }
            }
            return associations;
        }

        public override OU Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            return GetOU(uniqueId, include, exclude, fields, deletedItems);
        }

        private OU GetOU(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false, bool includeAncestors = false)
        {
            // remove "Settings" from include, because we'll load them in OuBuilder
            var includes = include.Split(',').ToList();

            if (includes.Any(i => i.Equals("children.settings", StringComparison.OrdinalIgnoreCase)))
            {
                includes.Add("Children");
            }

            var getInclude = string.Join(",", includes
                                            .Where(i => !i.Equals("settings", StringComparison.OrdinalIgnoreCase)
                                                     && !i.Equals("children.settings", StringComparison.OrdinalIgnoreCase)));

            OU ou = base.Get(uniqueId, getInclude, exclude, fields, deletedItems);
            ou = OUBuilder(ou, include, exclude, fields, includeAncestors, deletedItems);

            if (ou.Children != null)
            {
                foreach (var child in ou.Children)
                {
                    PopulateOUSummary(child);
                }
            }

            return ou;
        }

        public IEnumerable<SBOURoleDTO> GetAllRoles(string apiKey)
        {
            using (var dc = new DataContext())
            {
                return dc.OURoles.Select(x => new SBOURoleDTO
                {
                    Guid = x.Guid,
                    Name = x.Name
                }).ToList();
            }
        }

        public SBOUDTO GetSmartboardOus(Guid ouID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                // validate apiKey
                var sbSettings = _settingsService
                                    .Value
                                    .GetSettingsByOUID(ouID)
                                    ?.FirstOrDefault(x => x.Name == SolarTrackerResources.SelectedSettingName)
                                    ?.GetValue<ICollection<SelectedIntegrationOption>>()?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    return null;
                }
                var ou = dc.
                    OUs
                    ?.Include(o => o.Children)
                    ?.Include(o => o.Children.Select(x => x.Children))
                    ?.Include(o => o.Territories)
                    ?.FirstOrDefault(o => o.Guid == ouID && !o.IsDeleted && !o.IsDisabled && !o.IsArchived);

                return ou == null ? null : new SBOUDTO(ou);
            }
        }

        public IEnumerable<SBOUDTO> GetSmartboardAllOus(string apiKey)
        {
            using (var dc = new DataContext())
            {
                var matchingSettings = dc
                    .OUSettings?
                    .Where(x => x.Name == SolarTrackerResources.SelectedSettingName)?
                    .ToList();

                if (matchingSettings.Any() != true)
                {
                    return null;
                }

                Guid? rootOUID = null;
                foreach (var setting in matchingSettings)
                {
                    var sbSetting = setting?.GetValue<ICollection<SelectedIntegrationOption>>()?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;
                    if (sbSetting?.ApiKey == apiKey)
                    {
                        rootOUID = setting.OUID;
                        break;
                    }
                }

                if (!rootOUID.HasValue)
                {
                    return null;
                }

                var ous = dc.
                    OUs
                    ?.Include(o => o.Children)
                    ?.Include(o => o.Children.Select(x => x.Children))
                    ?.Include(o => o.Territories)
                    ?.Where(o => o.Guid == rootOUID && !o.IsDeleted && !o.IsDisabled && !o.IsArchived)
                    ?.ToList();

                if (ous?.Any() != true)
                {
                    return new List<SBOUDTO>();
                }
                return ous.Select(o => new SBOUDTO(o));
            }
        }

        public override ICollection<OU> GetMany(IEnumerable<Guid> uniqueIds, string include, string exclude, string fields, bool deletedItems = false)
        {
            var ous = base.GetMany(uniqueIds, include, exclude, fields, deletedItems).ToList();

            if (ous != null)
            {
                for (int i = 0; i < ous.Count; i++)
                {
                    ous[i] = OUBuilder(ous[i], include, fields);
                }
            }
            return ous;

        }

        /// <summary>
        /// Return only the OUS that the user has access to
        /// </summary>
        /// <param name="deletedItems"></param>
        /// <param name="pageNumber"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="filter"></param>
        /// <param name="include"></param>
        /// <param name="exclude"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public override ICollection<OU> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            string f = String.Format("PersonID={0}", SmartPrincipal.UserId);

            List<OUAssociation> asses = _associationService
                                            .List(false, 1, 100, f)
                                            .ToList();

            if (include.Contains("OU"))
            {
                PopulateAssociationsOUs(asses);
            }

            List<Guid> ouGuids = new List<Guid>();

            using (DataContext dc = new DataContext())
            {
                foreach (OUAssociation ass in asses)
                {
                    List<OU> ous = dc
                                    .Database
                                    .SqlQuery<OU>("exec [proc_SelectOUHierarchy] {0}", ass.OUID)
                                    .Where(o => (deletedItems || (!deletedItems && !o.IsDeleted)) && !o.IsArchived)
                                    .ToList();

                    foreach (OU ou in ous)
                    {
                        ouGuids.Add(ou.Guid);
                    }
                }
            }

            return this.GetMany(ouGuids, include, exclude, fields);

        }

        public OU GetByShapesVersion(Guid ouid, ICollection<OuShapeVersion> ouShapeVersions, bool deletedItems = false, string include = "")
        {
            var ou = Get(ouid, include, deletedItems: deletedItems);
            ou.WellKnownText = null;

            if (ou.Children != null && ouShapeVersions != null)
            {
                foreach (var childOu in ou.Children)
                {
                    var ouShape = ouShapeVersions.FirstOrDefault(s => s.Ouid == childOu.Guid);

                    if (childOu.ShapesVersion == ouShape?.Version)
                        childOu.WellKnownText = null;
                }
            }

            ou.Children = ou.Children?.Where(c => !c.IsArchived)?.ToList();

            return ou;
        }

        public override OU Insert(OU entity)
        {
            ValidateShapes(entity);

            // as per the discussion w/ Denisa, the client might send settings (as part of RestKIT implementation)
            // but the server should always discard them
            entity.Settings = null;

            if (entity.ParentID.HasValue)
            {
                var parent = Get(entity.ParentID.Value);
                entity.RootOrganizationID = parent != null ? parent.RootOrganizationID : entity.ParentID;
            }

            //the client sometimes doesn't set the RoleType on the association. do it here
            if (entity.Associations?.Any() == true)
            {
                using (var dc = new DataContext())
                {
                    var roleIds = entity.Associations.Select(x => x.OURoleID);
                    var roles = dc.OURoles.Where(x => roleIds.Contains(x.Guid)).ToList();

                    foreach (var assoc in entity.Associations)
                    {
                        var role = roles.FirstOrDefault(x => x.Guid == assoc.OURoleID);
                        if (role != null)
                        {
                            assoc.RoleType = role.RoleType;
                        }
                    }
                }
            }

            OU ou = base.Insert(entity);

            string include = string.Empty;


            try
            {
                include = UpdateNavigationProperties(entity);
            }
            catch (Exception)
            {
            }

            OU ret = base.Get(entity.Guid, include);

            if (ret != null)
            {
                ret.SaveResult = ou.SaveResult;
            }
            else
            {
                ret = ou;
            }

            base.ProcessApiWebHooks(ret.Guid, ApiObjectType.Organization, EventDomain.Organization, EventAction.Created, ret.Guid);

            return ret;
        }

        public override OU Update(OU entity)
        {
            ValidateShapes(entity);

            // as per the discussion w/ Denisa, the client might send settings (as part of RestKIT implementation)
            // but the server should always discard them
            entity.Settings = null;

            //the client sometimes doesn't set the RoleType on the association. do it here
            if (entity.Associations?.Any() == true)
            {
                using (var dc = new DataContext())
                {
                    var roleIds = entity.Associations.Select(x => x.OURoleID);
                    var roles = dc.OURoles.Where(x => roleIds.Contains(x.Guid)).ToList();

                    foreach (var assoc in entity.Associations)
                    {
                        var role = roles.FirstOrDefault(x => x.Guid == assoc.OURoleID);
                        if (role != null)
                        {
                            assoc.RoleType = role.RoleType;
                        }
                    }
                }
            }

            var ret = base.Update(entity);

            // this method will set the root Org Id to all children, if they don't already have it set.
            entity.PrepareNavigationProperties();

            //base.ProcessApiWebHooks(entity, EventDomain.Organization, EventAction.Changed, ret.Guid);

            try
            {
                UpdateNavigationProperties(entity);
            }
            catch (Exception) { }

            return ret;
        }

        private void ValidateShapes(OU entity)
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

        public override SaveResult Delete(Guid uniqueId)
        {
            var results = DeleteMany(new Guid[] { uniqueId });
            return results.FirstOrDefault();
        }

        public override ICollection<SaveResult> DeleteMany(Guid[] uniqueIds)
        {
            var results = new List<SaveResult>();
            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    var ous = uow.Get<OU>()
                        .Include(ou => ou.Associations)
                        .Include(ou => ou.Territories.Select(t => t.Assignments))
                        .Include(ou => ou.Children)
                        .Where(ou => uniqueIds.Contains(ou.Guid))
                        .ToList();
                    foreach (var ou in ous)
                    {
                        if (!ou.IsDeletableByClient)
                        {
                            results.Add(new SaveResult()
                            {
                                Success = false,
                                Action = DataAction.Delete,
                                ExceptionMessage = "You don't have delete rights on this OU!"
                            });
                        }
                        else
                        {
                            var result = TryEntitySoftDelete(ou.Guid, ou);
                            results.Add(result);

                            // cascade delete
                            var guids = GetOUAndChildrenGuids(ou.Guid);

                            // remove current OU
                            guids = guids
                                        .Distinct()
                                        .Except(new Guid[] { ou.Guid })
                                        .ToList();

                            if (guids.Count > 0)
                            {
                                var data = DeleteMany(guids.ToArray());

                                // Delete UserInvitations for deleted OU
                                var userInvitations = uow.Get<UserInvitation>()
                                    .Where(ui => ui.OUID == ou.Guid && ui.IsDeleted == false)
                                    .ToList();

                                uow.DeleteMany(userInvitations);
                                uow.DeleteMany(ou.Associations);
                                uow.DeleteMany(ou.Territories);
                                if (ou.Territories?.SelectMany(t => t.Assignments) != null)
                                {
                                    uow.DeleteMany(ou.Territories.SelectMany(t => t.Assignments).ToList());
                                }
                            }
                        }
                    }

                    uow.SaveChanges(new DataSaveOperationContext
                    {
                        DataAction = DataAction.Delete
                    });
                }
            }
            catch (Exception ex)
            {
                foreach (var saveResult in results)
                    saveResult.Success = false;

                results.Add(SaveResult.FromException(ex, DataAction.Delete));
            }

            return results;
        }


        public override SaveResult Activate(Guid uniqueId)
        {
            var results = ActivateMany(new Guid[] { uniqueId });
            return results.FirstOrDefault();
        }

        public override ICollection<SaveResult> ActivateMany(Guid[] uniqueIds)
        {
            var results = new List<SaveResult>();
            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    var ous = uow.Get<OU>()
                        .Include(ou => ou.Associations)
                        .Include(ou => ou.Territories.Select(t => t.Assignments))
                        .Include(ou => ou.Children)
                        .Where(ou => uniqueIds.Contains(ou.Guid))
                        .ToList();
                    foreach (var ou in ous)
                    {
                        var result = TryEntitySoftActivate(ou.Guid, ou);
                        results.Add(result);

                        //// cascade activate
                        //var guids = GetOUAndChildrenGuids(ou.Guid);
                        var guids = ou.Children.Select(o => o.Guid).ToList();

                        // remove current OU
                        guids = guids
                                        .Distinct()
                                        .Except(new Guid[] { ou.Guid })
                                        .ToList();

                        if (guids.Count > 0)
                        {
                            var data = ActivateMany(guids.ToArray());

                            // Activate UserInvitations for Activated OU
                            var userInvitations = uow.Get<UserInvitation>()
                                .Where(ui => ui.OUID == ou.Guid && ui.IsDeleted == true)
                                .ToList();

                            uow.ActivateMany(userInvitations);
                            uow.ActivateMany(ou.Associations);
                            uow.ActivateMany(ou.Territories);
                            if (ou.Territories?.SelectMany(t => t.Assignments) != null)
                            {
                                uow.ActivateMany(ou.Territories.SelectMany(t => t.Assignments).ToList());
                            }
                        }
                    }

                    uow.SaveChanges(new DataSaveOperationContext
                    {
                        DataAction = DataAction.Update
                    });
                }
            }
            catch (Exception ex)
            {
                foreach (var saveResult in results)
                    saveResult.Success = false;

                results.Add(SaveResult.FromException(ex, DataAction.Delete));
            }

            return results;
        }

        public ICollection<OU> ListAllForPerson(Guid personID)
        {

            List<OU> ret = new List<OU>();

            using (DataContext dc = new DataContext())
            {
                ret = dc
                        .Database
                        .SqlQuery<OU>("exec proc_OUsForPerson {0}", personID)
                        .Where(o => !o.IsArchived)
                        .ToList();

                foreach (OU ou in ret)
                {
                    OUService.PopulateOUSummary(ou);
                }
            }

            return ret;
        }

        public IEnumerable<SBOU> GetOusList(string apikey)
        {
            using (var dc = new DataContext())
            {
                var allAncestorIDs = dc
                                .Database
                                .SqlQuery<SBOU>($"select guid as OUID,Name as ParentTree, (select Replace(parents, 'DataReef Solar > ' , '') from [dbo].[GetOUTreeParentName](guid)) as Name from ous where isdeleted = 0 and guid not in (select ouid from ousettings where name = 'Integrations.Options.Selected' and isdeleted = 0)")
                                .ToList();

                return allAncestorIDs.OrderBy(x => x.Name);
            }
        }


        public string InsertApikeyForOU(SBOUID request, string apikey)
        {
            string ret = "";
            try
            {
                using (var uow = UnitOfWorkFactory())
                {
                    var valueObj = new SelectedIntegrationOption
                    {
                        Id = "smartBOARD-integration",
                        Name = "SmartBOARD",
                        Data = new IntegrationOptionData
                        {
                            SMARTBoard = new SMARTBoardIntegrationOptionData
                            {
                                BaseUrl = ConfigurationManager.AppSettings["Integrations.SMARTBoard.BaseURL"],
                                ApiKey = request.Apikey,
                                HomeUrl = "/leads/view/{0}",
                                CreditApplicationUrl = "SB_CreditApplicationUrl"
                            }
                        }
                    };
                    var newSetting = new OUSetting
                    {
                        OUID = request.OUID,
                        Value = JsonConvert.SerializeObject(valueObj),
                        CreatedByID = SmartPrincipal.UserId,
                        Name = "Integrations.Options.Selected",
                        Group = OUSettingGroupType.DealerSettings,
                        Inheritable = true,
                        ValueType = SettingValueType.JSON,
                    };
                    uow.Add(newSetting);
                    uow.SaveChanges();
                }

                ret = "Success";

            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }

            return ret;
        }

        public ICollection<OU> ListAllForPersonAndSetting(Guid personID, string settingName)
        {
            List<OU> ret = new List<OU>();
            using (DataContext dc = new DataContext())
            {
                ret = dc
                        .Database
                        .SqlQuery<OU>("exec proc_OUsForPerson {0}", personID)
                        .Where(o => !o.IsArchived)
                        .ToList();

                return ret.Where(o => dc.OUSettings.Any(os => os.OUID == o.Guid && os.Name == settingName)).ToList();


            }
        }

        public List<OUsAndRoleTree> GetOUsRoleTree(Guid personID)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var ouAssociations = uow
                                        .Get<OUAssociation>()
                                        .Where(oua => !oua.IsDeleted && oua.PersonID == personID)
                                        .OrderByDescending(oua => oua.OU.ParentID.HasValue)
                                        .ToList();
                var result = new List<OUsAndRoleTree>();

                foreach (var assoc in ouAssociations)
                {
                    var hasOUAsChild = result.Any(r => r.HasChildOU(assoc.OUID));

                    if (hasOUAsChild)
                    {
                        result.ForEach(r => r.UpdateRole(assoc.OUID, assoc.OURoleID, assoc.RoleType));
                        continue;
                    }

                    var children = _dataContextFactory
                                .Value
                                .GetDataContext()
                                .Database
                                .SqlQuery<OUTreePath>("SELECT * FROM OUTreePath({0})", assoc.OUID)
                                .Where(o => o.GetGuid() != assoc.OUID)
                                .ToList();
                    if (children.Any())
                    {
                        result.Add(new OUsAndRoleTree(assoc, children));
                    }
                }
                return result;
            }
        }

        public ICollection<Guid> ListRootGuidsForPerson(Guid personID)
        {
            string f = String.Format("PersonID={0}", personID);
            List<OUAssociation> asses = _associationService.List(false, 1, 100, f).ToList();

            List<Guid> rootGuids = new List<Guid>();
            List<Guid> finalGuids = new List<Guid>();

            using (DataContext dc = new DataContext())
            {
                foreach (OUAssociation ass in asses)
                {

                    if (rootGuids.Contains(ass.OUID)) continue;

                    bool isChild = false;

                    foreach (Guid g in rootGuids)
                    {
                        if (g == ass.OUID) continue;


                        isChild = dc.Database.SqlQuery<int>("exec [proc_OUExistsInHierarchy] {0},{1}", ass.OUID, g).Single() != 0;
                        if (isChild) break; // dont even add, and stop comparing to others cuz its a child


                        if (isChild == false) isChild = dc.Database.SqlQuery<int>("exec [proc_OUExistsInHierarchy] {1},{0}", ass.OUID, g).Single() != 0;
                        if (isChild) //remove the existing cuz its a child
                        {
                            rootGuids.Remove(g);
                            rootGuids.Add(ass.OUID);
                            break;
                        }
                    }

                    if (!isChild) rootGuids.Add(ass.OUID);
                }


                //lastly now compare all of them to each other
                foreach (Guid g in rootGuids)
                {
                    bool isChild = false;

                    foreach (Guid gg in rootGuids)
                    {
                        if (g == gg) continue;

                        isChild = dc.Database.SqlQuery<int>("exec [proc_OUExistsInHierarchy] {0},{1}", g, gg).Single() != 0;
                        if (isChild) break;
                    }

                    if (!isChild) finalGuids.Add(g);

                }
                return finalGuids;
            }
        }

        /// <summary>
        /// Get Root OUs for a person
        /// </summary>
        /// <param name="personID"></param>
        /// <param name="include"></param>
        /// <param name="exclude"></param>
        /// <param name="fields"></param>
        /// <param name="query">Partial OU name</param>
        /// <returns></returns>
        public ICollection<OU> ListRootsForPerson(Guid personID, string include, string exclude, string fields, string query = null)
        {
            var finalGuids = ListRootGuidsForPerson(personID);

            if (!string.IsNullOrWhiteSpace(query))
            {
                finalGuids = GetChildOUIDs(finalGuids, query);
            }

            var result = GetMany(finalGuids, include, exclude, fields);
            // if query is present, remove the parents because they mess up the serialization
            if (!string.IsNullOrWhiteSpace(query))
            {
                foreach (var ou in result)
                {
                    ou.Parent = null;
                }
            }
            return result;
        }

        [Obsolete("please use PersonService.GetMine")]
        public ICollection<Person> GetMembers(OUMembersRequest request)
        {
            var organizationIds = GetHierarchicalOrganizationGuids(request.OUIDs, request.ExcludeOUs);

            using (DataContext dc = new DataContext())
            {
                var people = dc
                                .OUAssociations
                                .Where(oua => organizationIds.Contains(oua.OUID))
                                .Select(oua => oua.Person)
                                .Distinct();

                if (!request.IsDeleted)
                {
                    people = people.Where(p => p.IsDeleted == false);
                }

                AssignIncludes<Person>(request.Include, ref people);
                AssignFilters<Person>(request.Filter, ref people);


                request.SortColumn = string.IsNullOrEmpty(request.SortColumn) ? "FirstName" : request.SortColumn;
                request.SortOrder = string.IsNullOrEmpty(request.SortOrder) ? "asc" : request.SortOrder;

                people = people.OrderBy(string.Format("{0} {1}", request.SortColumn, request.SortOrder));

                var result = people
                            .Skip((request.PageNumber - 1) * request.ItemsPerPage)
                            .Take(request.ItemsPerPage)
                            .ToList();

                PersonService.PopulateSummary(result);


                //AssignSerializationNavigation<Person>(request.Include, result);
                //AssignSerializationFields<Person>(request.Fields, result);

                return result;
            }
        }

        public ICollection<Guid> ConditionalGetActiveOUAssociationIDsForCurrentAndSubOUs(Guid ouID, bool deepSearch)
        {
            List<Guid> ouAssociationIds;
            var organizationIds = deepSearch ? GetHierarchicalOrganizationGuids(new List<Guid> { ouID }) : new List<Guid> { ouID };

            using (DataContext dc = new DataContext())
            {
                ouAssociationIds = dc.OUAssociations.Where(oua => !oua.IsDeleted && !oua.Person.IsDeleted &&
                                                                                            organizationIds.Contains(oua.OUID))
                                             .Select(oua => oua.Guid).Distinct().ToList();
            }

            return ouAssociationIds;
        }

        public ICollection<Guid> ConditionalGetActivePeopleIDsForCurrentAndSubOUs(Guid ouID, bool deepSearch)
        {
            List<Guid> peopleIds;
            var organizationIds = deepSearch ? GetHierarchicalOrganizationGuids(new List<Guid> { ouID }) : new List<Guid> { ouID };

            using (DataContext dc = new DataContext())
            {
                peopleIds = dc.OUAssociations.Where(oua => !oua.IsDeleted && !oua.Person.IsDeleted &&
                                                                                            organizationIds.Contains(oua.OUID))
                                             .Select(oua => oua.PersonID).Distinct().ToList();
            }

            return peopleIds;
        }

        public ICollection<Guid> ConditionalGetActiveUserIDsForCurrentAndSubOUs(Guid ouID, bool deepSearch)
        {
            List<Guid> userIds;
            var organizationIds = deepSearch ? GetHierarchicalOrganizationGuids(new List<Guid> { ouID }) : new List<Guid> { ouID };

            using (DataContext dc = new DataContext())
            {
                userIds = dc.Users.Where(u => u.IsDeleted == false && u.Person.IsDeleted == false &&
                                         u.Person.OUAssociations.Any(oua => organizationIds.Contains(oua.OUID)))
                                  .Select(u => u.Guid).Distinct().ToList();
            }
            return userIds;
        }

        public ICollection<WebHook> GetWebHooks(Guid ouID, EventDomain domainFlags)
        {

            List<WebHook> ret = new List<WebHook>();

            using (DataContext dc = new DataContext())
            {
                ret = dc.Database.SqlQuery<WebHook>("proc_WebHooks @OUID,@EventFlags",
                    new SqlParameter("@OUID", ouID),
                    new SqlParameter("@EventFlags", domainFlags)).ToList();
            }

            return ret;

        }

        /// <summary>
        /// Process eventmessage
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        public bool ProcessEvent(EventMessage eventMessage)
        {
            var ouSettings = eventMessage.OUSettings;
            if (ouSettings == null)
            {
                if (eventMessage.OUID.HasValue)
                {
                    ouSettings = OUSettingService.GetOuSettings(eventMessage.OUID.Value);
                }
            }
            var handlerSettings = ouSettings?.FirstOrDefault(ous => ous.Name == OUSetting.Legion_EventMessageHandlers);

            var allHandlers = handlerSettings?.GetValue<List<OUEventHandlerDataView>>();

            var matchingHandlers = allHandlers?
                                        .Where(h => h.EventSource == eventMessage.EventSource &&
                                                  (!h.EventAction.HasValue ||
                                                        (h.EventAction.HasValue && ((h.EventAction.Value & eventMessage.EventAction) != 0))
                                                  )
                                              )?
                                                .ToList();

            if ((matchingHandlers?.Count ?? 0) == 0)
            {
                // no handlers to match against
                return false;
            }

            var result = false;

            foreach (var handlerDV in matchingHandlers)
            {
                try
                {
                    var type = Type.GetType(handlerDV.HandlerClassFullName);
                    var handler = (IEventHandler)Activator.CreateInstance(type);
                    result = result || (handler?.HandleEventMessage(eventMessage, handlerDV) ?? false);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }


        public ICollection<Territory> GetOUTreeTerritories(Guid ouID, string territoryName, bool deletedItems = false)
        {
            List<Territory> ret = new List<Territory>();

            using (DataContext dc = new DataContext())
            {
                ret = dc
                    .Database
                    .SqlQuery<Territory>("exec proc_GetOUTreeTerritories {0}, {1}, {2}", ouID, territoryName, deletedItems ? "1" : "0")
                    .Where(t => !t.IsArchived)
                    .ToList();
            }

            return ret;
        }

        public ICollection<Guid> GetHierarchicalOrganizationGuids(ICollection<Guid> parentOUs, ICollection<Guid> excludeOUs = null)
        {
            List<Guid> response = new List<Guid>();

            using (DataContext dataContext = new DataContext())
            {
                foreach (var parentGuid in parentOUs)
                {
                    var ids = dataContext
                                .Database
                                .SqlQuery<OU>("exec [proc_SelectOUHierarchy] {0}", parentGuid)
                                .Where(o => !o.IsDeleted && !o.IsArchived)
                                .Select(o => o.Guid)
                                .ToList();

                    if (excludeOUs != null && ids != null)
                    {
                        ids = ids
                                .Where(i => !excludeOUs.Contains(i))
                                .ToList();
                    }

                    response.AddRange(ids);
                }
            }
            return response.Distinct().ToList();
        }

        public List<GuidNamePair> GetAllSubOUIdsAndNamesOfSpecifiedOus(string ouIDs)
        {
            var ouIDsList = ouIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(ouid => new Guid(ouid)).ToList();

            using (DataContext dataContext = new DataContext())
            {
                var ouids = new List<Guid>();
                foreach (var ouid in ouIDsList)
                {
                    var ids = dataContext
                                .Database
                                .SqlQuery<OU>("exec [proc_SelectOUHierarchy] {0}", ouid)
                                .Where(o => !o.IsDeleted && !o.IsArchived)
                                .Select(o => o.Guid)
                                .ToList();

                    ouids.AddRange(ids);
                }

                return dataContext
                            .OUs
                            .Where(o => !o.IsDeleted && ouids.Contains(o.Guid))
                            .ToList()
                            .Select(o => new GuidNamePair { Guid = o.Guid, Name = o.Name })
                            .ToList();
            }
        }

        public static ICollection<Guid> GetOUTree(Guid ouid)
        {
            using (var dc = new DataContext())
            {
                return
                    dc
                        .Database
                        .SqlQuery<Guid>("select Guid from dbo.OUTree({0})", ouid)
                        .ToList();
            }
        }

        public ICollection<Guid> GetChildOUIDs(ICollection<Guid> parentIDs, string query)
        {
            var result = new List<Guid>();

            using (var uow = UnitOfWorkFactory())
            {
                var allIds = new List<Guid>();

                foreach (var ouid in parentIDs)
                {
                    var childIds = GetOUTree(ouid);
                    childIds.Remove(ouid);
                    allIds.AddRange(childIds);
                }

                allIds = allIds
                            .Distinct()
                            .ToList();

                result = uow
                            .Get<OU>()
                            .Where(o => allIds.Contains(o.Guid) && o.Name.Contains(query) && !o.IsArchived)
                            .Select(o => o.Guid)
                            .ToList();
            }
            return result;
        }

        public ICollection<Guid> GetOUAndChildrenGuids(Guid ouid)
        {
            var ouAndChildrenGuidsSp = new OUAndChildrenGuids(ouid);
            using (var uow = UnitOfWorkFactory())
            {
                uow.InvokeStoredProcedure(ouAndChildrenGuidsSp);
                var ouGuids = ouAndChildrenGuidsSp.Result.ToList();

                return ouGuids;
            }
        }

        public OUChildrenAndTerritories GetOUWithChildrenAnTerritories(Guid ouID)
        {
            using (var dc = new DataContext())
            {
                var userAssociations = dc.OUAssociations.Include(oa => oa.OURole).Where(oa => !oa.IsDeleted && oa.PersonID == SmartPrincipal.UserId).ToList();
                var canViewAll = false;
                var found = false;
                foreach (var ua in userAssociations)
                {
                    var childrenGuids = GetOUAndChildrenGuids(ua.OUID);
                    if (childrenGuids?.Any(x => x == ouID) == true)
                    {
                        found = true;
                        if (ua.OURole.RoleType == OURoleType.Manager || ua.OURole.RoleType == OURoleType.SuperAdmin || ua.OURole.RoleType == OURoleType.Owner)
                        {
                            canViewAll = true;
                        }
                    }
                }

                if (!found)
                {
                    return null;
                }

                var ou = dc
                    .OUs
                    .Include(o => o.Children)
                    .Include(o => o.Territories)
                    .Include(o => o.Territories.Select(x => x.Assignments))
                    .FirstOrDefault(o => !o.IsDeleted && o.Guid == ouID);

                if (ou == null)
                {
                    return null;
                }



                var matchingTerritories = canViewAll
                    ? ou.Territories.Where(t => !t.IsDeleted)
                    : ou.Territories.Where(t => !t.IsDeleted && !t.IsArchived && t.Assignments?.Any(a => a.PersonID == SmartPrincipal.UserId) == true);

                return new OUChildrenAndTerritories
                {
                    OUID = ouID,
                    Children = ou.Children.Where(o => !o.IsDeleted && !o.IsArchived).Select(o => new EntityWithShape(o)),
                    Territories = matchingTerritories.Select(t => new EntityWithShape(t))
                };

            }
        }

        public ICollection<FinancePlanDefinition> GetFinancePlanDefinitions(Guid ouid, string include = "", string exclude = "", string fields = "")
        {
            using (var dataContext = new DataContext())
            {
                // get all ancestor OUIDs (including ouid)
                var allAncestorIDs = dataContext
                                .Database
                                .SqlQuery<Guid>($"select * from OUTreeUP('{ouid}')")
                                .ToList();

                // get Financing Options OU Settings for all ancestors
                var allOUSettings = dataContext
                                .OUSettings
                                .Where(ous => allAncestorIDs.Contains(ous.OUID) && ous.Name == OUSetting.Financing_Options && !ous.IsDeleted)
                                .ToList()
                                .OrderBy(ous => allAncestorIDs.IndexOf(ous.OUID))
                                .ToList();

                // convert ousettings to a dictionary of OUID : FinancingSettingDataView List
                var financingOptions = allOUSettings
                                        .Select(s => new { ouid = s.OUID, setts = s.GetValue<List<FinancingSettingsDataView>>() })
                                        .Where(s => s.setts?.Count > 0)
                                        .ToList();

                List<FinancePlanDefinition> result = null;

                // if non of the ancestors (including current OU) have a Financing Option setting
                // we return all the finance plans
                if (allOUSettings == null || allOUSettings?.Count == 0)
                {
                    result = _financePlanDefinitionService
                                .List(itemsPerPage: 3000, include: include, exclude: exclude, fields: fields)
                                .ToList();
                }
                else
                {
                    var finOptions = financingOptions.FirstOrDefault();

                    var planIds = finOptions
                                    .setts
                                    .Where(s => finOptions.ouid == ouid
                                                || (finOptions.ouid != ouid
                                                     && s.GetIsEnabled())
                                           )
                                    .Select(fo => fo.PlanID)
                                    .ToList();

                    result = _financePlanDefinitionService.GetMany(planIds, include, exclude, fields).ToList();
                }

                //var settings = _settingsService.Value.GetSettings(ouid, null);

                //var ids = settings.GetByKey<List<Guid>>(OUSetting.Financing_PlansOrder) ?? new List<Guid>();
                //result = _financePlanDefinitionService.GetMany(ids, include, exclude, fields).ToList();

                // when including Provider, the service automatically adds FinancePlanDefinitions.
                // We remove it below so API could nicely serialize the response
                foreach (var item in result)
                {
                    if (item.Provider != null)
                    {
                        item.Provider.FinancePlanDefinitions = null;
                    }
                }
                return result.OrderBy(a => a.Name).ToList();
            }
        }

        public float GetTokenPriceInDollars(Guid ouid)
        {
            using (DataContext dataContext = new DataContext())
            {
                var rootOU = dataContext.OUs.SingleOrDefault(ou => ou.Guid == ouid);
                if (rootOU.RootOrganizationID.Value != ouid) rootOU = dataContext.OUs.SingleOrDefault(ou => ou.Guid == rootOU.RootOrganizationID.Value);
                return rootOU.TokenPriceInDollars ?? 0;
            }
        }

        public void MoveOU(Guid ouID, Guid newParentOUID)
        {
            using (DataContext dataContext = new DataContext())
            {
                OU ou = dataContext.OUs.FirstOrDefault(o => o.Guid.Equals(ouID));
                OU newParent = dataContext.OUs.FirstOrDefault(o => o.Guid.Equals(newParentOUID));

                if (ou != null && !ou.IsDeleted && !ou.ParentID.Equals(newParentOUID) && newParent != null && !newParent.IsDeleted)
                {
                    ou.ParentID = newParentOUID;
                    dataContext.SaveChanges();
                }
            }
        }

        public LookupDataView GetOnboardingLookupData(Guid? parentId)
        {
            var dt = SmartPrincipal.DeviceType;
            var ret = new LookupDataView();

            using (var dc = new DataContext())
            {
                //TODO: need to only populate panels & inverters from the parent

                if (parentId.HasValue)
                {
                    ret.States = dc
                                .OUShapes
                                .Where(s => s.OUID == parentId
                                            && s.ShapeTypeID == "state"
                                            && s.Name != "New OUShape")
                                .Select(s => s.Name)
                                .ToList();

                    ret.Ancestors = GetAncestors(parentId.Value, dc);

                    var ancestorIds = ret
                                        .Ancestors
                                        .Select(a => a.Guid)
                                        .ToList();

                    ret.AncestorsCustomProposalFlow = dc
                                .OUs
                                .Include("FinanceAssociations.FinancePlanDefinition.Provider")
                                .Where(o => ancestorIds.Contains(o.Guid))
                                .Where(o => o.FinanceAssociations.Any(fa => fa.FinancePlanDefinition.Provider.ProposalFlowType != FinanceProviderProposalFlowType.None))
                                .ToList()?
                                .OrderBy(o => ancestorIds.IndexOf(o.Guid))?
                                .FirstOrDefault()?
                                .FinanceAssociations?
                                .FirstOrDefault(fa => fa.FinancePlanDefinition?.Provider?.ProposalFlowType != FinanceProviderProposalFlowType.None)?
                                .FinancePlanDefinition?
                                .Provider?
                                .ProposalFlowType ?? FinanceProviderProposalFlowType.None;

                    var settings = _settingsService.Value.GetSettings(parentId.Value, null);

                    //ret.HasTenantAncestor = dc
                    //            .OUSettings
                    //            .Any(os => ancestorIds.Contains(os.OUID) && os.Name == OUSetting.Solar_IsTenant && !os.IsDeleted);

                    ret.ParentSettings = settings;

                    var panelIds = new List<Guid>();
                    var inverterIds = new List<Guid>();
                    var financePlans = new List<FinancingSettingsDataView>();

                    // only read the Ids if it has a Tenant Ancestor
                    //if (ret.HasTenantAncestor)
                    //{
                    //    panelIds = settings.GetByKey<List<Guid>>(OUSetting.Solar_Panels) ?? panelIds;
                    //    inverterIds = settings.GetByKey<List<Guid>>(OUSetting.Solar_Inverters) ?? inverterIds;
                    //    financePlans = settings.GetByKey<List<FinancingSettingsDataView>>(OUSetting.Financing_Options) ?? financePlans;
                    //}
                    var financePlanIds = financePlans
                                            .Select(fp => fp.PlanID)
                                            .ToList();

                    ret.Panels = dc
                                .SolarPanel
                                .Where(sp => !sp.IsDeleted
                                            // if any of ancestors has a list of panels, restrict the result to that list
                                            // otherwhise, return them all
                                            && (
                                                (panelIds.Count > 0 && panelIds.Contains(sp.Guid))
                                                || (panelIds.Count == 0))
                                                )
                                .GroupBy(sp => sp.Description)
                                .Select(g => g.FirstOrDefault())
                                .ToList()
                                .Select(sp => new GuidNamePair(sp.Guid, $"{sp.Name} - {sp.Description} [{sp.Watts} watts]"))
                                .OrderBy(gnp => gnp.Name)
                                .ToList();

                    ret.Inverters = dc
                                    .Inverters
                                    .Where(i => !i.IsDeleted
                                            && (
                                                (inverterIds.Count > 0 && inverterIds.Contains(i.Guid))
                                                || (inverterIds.Count == 0))
                                                )
                                    .GroupBy(i => i.Model)
                                    .Select(g => g.FirstOrDefault())
                                    .ToList()
                                    .Select(sp => new GuidNamePair(sp.Guid, $"{sp.Name}"))
                                    .OrderBy(gnp => gnp.Name)
                                    .ToList();

                    ret.FinancePlans = dc
                                    .FinancePlaneDefinitions
                                    .Include(fp => fp.Provider)
                                    .Where(pd => (!pd.IsDeleted && !pd.IsDisabled)
                                                && ((pd.Type == FinancePlanType.Cash || pd.Type == FinancePlanType.Mortgage)
                                                || ((financePlanIds.Count > 0 && financePlanIds.Contains(pd.Guid)) || financePlanIds.Count == 0)))
                                    .ToList()
                                    .Select(sp => new FinancePlanDataView
                                    {
                                        Guid = sp.Guid,
                                        Name = sp.Name,
                                        FlowType = sp.Provider.ProposalFlowType,
                                        DefaultDealerFee = sp.DealerFee,
                                        IntegrationProvider = sp.IntegrationProvider,
                                        MetaData = sp.MetaDataJSON,
                                    })
                                    .ToList();
                }

                ret.Templates = new List<GuidNamePair>
                {
                    new GuidNamePair{
                        Guid = Guid.Parse("b41eda2d-416b-4ba2-8c24-c83eeee65d35"),
                        Name = "Generic Proposal"
                    },
                    new GuidNamePair {
                        Guid = Guid.Parse("95f874d5-92d0-4fc5-9704-47956c7549cb"),
                        Name = "Solcius"
                    }
                };
            }

            return ret;
        }

        public void CreateNewOU(OnboardingOUDataView req)
        {
            // TODO: add validations
            // Use the generic proposal template guid
            req.ProposalTemplateID = new Guid("b41eda2d-416b-4ba2-8c24-c83eeee65d35");
            var parent = Get(req.ParentID);

            using (var dc = new DataContext())
            {
                var existingContractor = req.ValidateContractorID(req.ParentID, dc);
                if (!string.IsNullOrEmpty(existingContractor))
                {
                    throw new ApplicationException($"There's another OU ({existingContractor}) with that contractor ID! Please choose another Contractor ID.");
                }

                //if (req.BasicInfo.IsSolarTenant)
                //{
                //    var contractorIDExists = dc
                //                            .OUSettings
                //                            .Any(s => s.Name == OUSetting.Solar_ContractorID && s.Value == req.BasicInfo.OUName);
                //    if (contractorIDExists)
                //    {
                //        throw new ApplicationException("There's another tenant with that name! Please choose another one.");
                //    }
                //}

                using (var transaction = dc.Database.BeginTransaction())
                {
                    try
                    {
                        // Create the OU
                        var ou = new OU
                        {
                            AccountID = parent.AccountID,
                            ParentID = req.ParentID,
                            Name = req.BasicInfo.OUName,
                            ActivityTypes = parent.ActivityTypes,
                            RootOrganizationID = parent?.RootOrganizationID,
                            RootOrganizationName = parent?.RootOrganizationName,
                            CreatedByID = SmartPrincipal.UserId,
                            CreatedByName = "InternalPortal",
                            IsTerritoryAdd = req.BasicInfo.IsTerritoryAdd,
                            MinModule = req.BasicInfo.MinModule
                        };

                        dc.OUs.Add(ou);

                        var wkt = HandleOUStates(ou.Guid, req.BasicInfo.States, dc);
                        if (!string.IsNullOrWhiteSpace(wkt))
                        {
                            ou.WellKnownText = wkt;
                        }

                        var existingSettings = dc
                                        .OUSettings
                                        .Where(s => s.OUID == ou.Guid)
                                        .ToList();

                        req.HandleLogoImage(ou.Guid, existingSettings, _blobService.Value);

                        var settings = req.HandleSettings(ou.Guid, existingSettings, _auditService, _blobService);

                        if (settings.Count > 0)
                        {
                            dc.OUSettings.AddRange(settings);
                        }

                        dc.SaveChanges();

                        var firstName = req?.BasicInfo?.OwnerFirstName;

                        if (!string.IsNullOrEmpty(firstName) && firstName != "none")
                        {
                            var invite = new UserInvitation
                            {
                                FromPersonID = SmartPrincipal.UserId,
                                EmailAddress = req.BasicInfo.OwnerEmail,
                                FirstName = req.BasicInfo.OwnerFirstName,
                                LastName = req.BasicInfo.OwnerLastName,
                                OUID = ou.Guid,
                                RoleID = OURole.OwnerRoleID,
                                CreatedByID = SmartPrincipal.UserId,
                            };
                            _userInvitationService.Value.Insert(invite, dc);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void EditOUSettings(Guid ouID, OnboardingOUSettingsDataView req)
        {
            using (var dc = new DataContext())
            {
                var ou = dc
                            .OUs
                            .FirstOrDefault(o => o.Guid == ouID
                                              && !o.IsDeleted);

                if (ou == null)
                {
                    throw new ApplicationException("OU does not exist!");
                }

                var existingSettings = dc
                    .OUSettings
                    .Where(s => s.OUID == ou.Guid)
                    .ToList();

                var settings = req.HandleSettingsLite(ou.Guid, existingSettings, _auditService, _blobService);

                if (settings.Count > 0)
                {
                    dc.OUSettings.AddRange(settings);
                }

                dc.SaveChanges();
            }
        }

        public void EditOU(Guid ouid, OnboardingOUDataView req)
        {
            using (var dc = new DataContext())
            {
                var ou = dc
                            .OUs
                            .FirstOrDefault(o => o.Guid == ouid
                                              && !o.IsDeleted);

                if (ou == null)
                {
                    throw new ApplicationException("OU does not exist!");
                }

                var existingContractor = req.ValidateContractorID(ou.Guid, dc);

                if (!string.IsNullOrEmpty(existingContractor))
                {
                    throw new ApplicationException($"There's another OU ({existingContractor}) with that contractor ID! Please choose another Contractor ID.");
                }

                ou.Name = req.BasicInfo.OUName;
                ou.IsTerritoryAdd = req.BasicInfo.IsTerritoryAdd;
                ou.MinModule = req.BasicInfo.MinModule;
                ou.Updated(SmartPrincipal.UserId, "InternalPortal");

                var wkt = HandleOUStates(ouid, req.BasicInfo.States, dc);
                if (!string.IsNullOrWhiteSpace(wkt))
                {
                    ou.WellKnownText = wkt;
                    ou.ShapesVersion += 1;
                }

                var existingSettings = dc
                    .OUSettings
                    .Where(s => s.OUID == ou.Guid)
                    .ToList();

                req.HandleLogoImage(ou.Guid, existingSettings, _blobService.Value);

                var settings = req.HandleSettings(ou.Guid, existingSettings, _auditService, _blobService);

                if (settings.Count > 0)
                {
                    dc.OUSettings.AddRange(settings);
                }

                dc.SaveChanges();
            }
        }

        /// <summary>
        /// Method that verifies if the states provided need to update the ouid
        /// </summary>
        /// <param name="ouid"></param>
        /// <param name="states"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        private string HandleOUStates(Guid ouid, List<string> states, DataContext dc)
        {
            var ouShapeNames = new HashSet<string>(dc
                                                    .OUShapes
                                                    .Where(ous => ous.OUID == ouid && !ous.IsDeleted)
                                                    .Select(ous => ous.Name)
                                                    .ToList());
            if (ouShapeNames.SequenceEqual(states))
            {
                return null;
            }


            var ouStates = dc
                                .OUShapes
                                .Where(s => s.OUID == ouid
                                        && !s.IsDeleted
                                        && (s.ShapeTypeID == "state" || s.ShapeTypeID == "country")
                                        && s.Name != "New OUShape")
                                .ToList();

            var ouStateNames = ouStates
                                .Select(os => os.Name)
                                .ToList();

            var statesToRemove = ouStates
                                    .Where(s => !states.Contains(s.Name, StringComparer.OrdinalIgnoreCase))
                                    .ToList();

            var statesLeftAfterRemoval = ouStates
                                            .Where(s => !statesToRemove.Contains(s))
                                            .ToList();
            var statesToAdd = states
                                .Where(s => !ouStateNames.Contains(s, StringComparer.OrdinalIgnoreCase))
                                .ToList();

            var shapesUpdated = statesToRemove.Count > 0 || statesToAdd.Count > 0;

            foreach (var state in statesToRemove)
            {
                dc.OUShapes.Remove(state);
            }

            if (statesToAdd.Count > 0)
            {
                var geoStates = _geoBridge.Value.GetShapesForStates(statesToAdd);
                var stateShapes = geoStates
                            .Select(s => new OUShape
                            {
                                Name = s.ShapeName,
                                OUID = ouid,
                                CentroidLat = s.CentroidLat,
                                CentroidLon = s.CentroidLon,
                                CreatedByID = SmartPrincipal.UserId,
                                Radius = s.Radius,
                                WellKnownText = s.ShapeReduced,
                                ExternalID = s.ShapeID,
                                ShapeTypeID = s.ShapeTypeID,
                                ShapeID = new Guid(s.ShapeID),
                                ParentID = !string.IsNullOrWhiteSpace(s.ParentID) ? new Guid(s.ParentID) : (Guid?)null,
                            })
                            .ToList();
                foreach (var state in stateShapes)
                {
                    dc.OUShapes.Add(state);
                }
                statesLeftAfterRemoval.AddRange(stateShapes);
            }

            if (shapesUpdated)
            {
                // update the OU WKT
                var stateWKTs = statesLeftAfterRemoval.Select(s => DbGeography.FromText(s.WellKnownText)).ToList();
                var first = stateWKTs.First();

                foreach (var wkt in stateWKTs)
                {
                    if (wkt == first)
                    {
                        continue;
                    }
                    first = first.Union(wkt);
                }

                return first.WellKnownValue.WellKnownText;
            }

            return null;
        }

        public List<GuidNamePair> GetAncestorsForOU(Guid ouID)
        {
            return GetAncestors(ouID);
        }

        private List<GuidNamePair> GetAncestors(Guid ouid, DataContext dc = null)
        {
            var dcExists = dc != null;
            if (!dcExists)
            {
                dc = new DataContext();
            }
            var allAncestorIDs = dc
                                .Database
                                .SqlQuery<Guid>($"select * from OUTreeUP('{ouid}')")
                                .ToList();
            // need to reverse the list. Last is the root OU
            allAncestorIDs.Reverse();
            //var firstAncestor = -1;
            var foundFirst = false;
            var myIDs = new List<Guid>();

            var associationIds = dc
                                .OUAssociations
                                .Where(oua => allAncestorIDs.Contains(oua.OUID) && oua.PersonID == SmartPrincipal.UserId)
                                .Select(oua => oua.OUID)
                                .ToList();

            for (int i = 0; i < allAncestorIDs.Count; i++)
            {
                var ancestorId = allAncestorIDs[i];
                if (!foundFirst)
                {
                    foundFirst = associationIds.Contains(ancestorId);
                }

                if (foundFirst)
                {
                    myIDs.Add(ancestorId);
                }
            }

            var result = dc
                        .OUs
                        .Where(o => myIDs.Contains(o.Guid))
                        .ToList()
                        .Select(o => new GuidNamePair(o))
                        .OrderBy(o => myIDs.IndexOf(o.Guid))
                        .ToList();

            if (!dcExists)
            {
                dc.Dispose();
            }
            return result;
        }

        public IEnumerable<Person> GetPersonsAssociatedWithOUOrAncestor(Guid ouID, string name, string email)
        {
            using (var dc = new DataContext())
            {
                var allAncestorIDs = dc
                                .Database
                                .SqlQuery<Guid>($"select * from OUTreeUP('{ouID}')")
                                .ToList();

                var associationsQuery = dc
                                    .OUAssociations
                                    .Include(x => x.Person)
                                    .Where(oua => allAncestorIDs.Contains(oua.OUID) && !oua.IsDeleted);

                if (!string.IsNullOrEmpty(email))
                {
                    associationsQuery = associationsQuery.Where(x => x.Person.EmailAddressString.Contains(email));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    associationsQuery = associationsQuery.Where(x => x.Person.Name.Contains(name));
                }

                var result = associationsQuery
                            .Select(a => a.Person)
                            .Where(p => !p.IsDeleted)
                            .Distinct()
                            .OrderBy(p => p.Name)
                            .ToList();


                return result;
            }

        }

        public OU GetWithAncestors(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool summary = true, string query = "", bool deletedItems = false)
        {
            OU ou = GetOU(uniqueId, include, exclude, fields, deletedItems, true);
            //ou = OUBuilder(ou, include, exclude, fields, true, true);

            if (ou.Children != null)
            {
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var childIds = GetChildOUIDs(new List<Guid> { uniqueId }, query);
                    ou.Children = base.GetMany(childIds, include, exclude, fields, deletedItems);

                    // if Parent is present, it will break the serialization
                    foreach (var child in ou.Children)
                    {
                        child.Parent = null;
                    }
                }

                if (summary)
                {
                    foreach (var child in ou.Children)
                    {
                        PopulateOUSummary(child);
                    }
                }
            }

            return ou;
        }

        public List<OUWithAncestors> GetSubOUsForOuSetting(Guid parentOUID, string settingName)
        {
            using (DataContext dataContext = new DataContext())
            {
                var userOus = ListAllForPerson(SmartPrincipal.UserId) ?? new List<OU>();

                var ids = GetOUAndChildrenGuids(parentOUID);
                ids?.Remove(parentOUID);

                var ouSettings = dataContext.OUSettings.Where(o => ids.Contains(o.OUID) && o.Name.Equals(settingName) && !o.IsDeleted).ToList();

                return userOus
                    .Where(o => ids?.Contains(o.Guid) == true && ouSettings?.Any(os => os.OUID == o.Guid) == true)
                    .Select(o => new OUWithAncestors
                    {
                        Guid = o.Guid,
                        Name = o.Name,
                        Ancestors = GetAncestors(o.Guid, dataContext)
                    }).ToList();
            }
        }

        public Tuple<string, List<ActiveUserDTO>> GetActiveUsersForOrgID(Guid ouid)
        {

            using (DataContext dataContext = new DataContext())
            {
                var activeUsers = dataContext
                         .Database
                         .SqlQuery<ActiveUserDTO>(@"SELECT P.Guid AS Guid, FirstName, LastName, P.Name, EmailAddressString, R.Name AS RoleName
                                                    FROM People P
                                                    INNER JOIN OUAssociations OUA 
                                                    ON P.Guid = OUA.PersonID
                                                    INNER JOIN OURoles R
                                                    ON OUA.OURoleID = R.Guid
                                                    WHERE P.GUID IN 
                                        (
	                                        SELECT PersonID
	                                        FROM OUAssociations
	                                        WHERE OUID IN
	                                        (
		                                        select * from [dbo].[OUTree]({0})
	                                        ) AND IsDeleted = 0
                                        )
                                        AND P.IsDeleted = 0 AND OUA.IsDeleted = 0
                                        Order by Name", ouid)
                        .ToList();

                var users = activeUsers
                        .GroupBy(au => au.Guid)
                        .Select(g => g.HighestRole())
                        .ToList();

                var ouName = dataContext.OUs.FirstOrDefault(o => o.Guid == ouid)?.Name;
                return new Tuple<string, List<ActiveUserDTO>>(ouName, users);
            }
        }

        public OUActiveUsersCSV GetActiveUsersCSV(Guid ouid)
        {
            var data = GetActiveUsersForOrgID(ouid);

            var response = new OUActiveUsersCSV
            {
                OUName = data.Item1,
            };

            using (var memStream = new MemoryStream())
            {
                using (var writer = new StreamWriter(memStream))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.WriteRecords(data.Item2);
                        writer.Flush();
                    }
                }
                response.ActiveUsersCSV = memStream.ToArray();
            }

            return response;
        }

        //using (var writer = new StreamWriter("path\\to\\file.csv"))
        //using (var csv = new CsvWriter(writer))
        //{
        //    csv.WriteRecords(records);
        //}


        public IEnumerable<zapierOus> GetzapierOusList(float? Lat, float? Lon, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //-- exec usp_GetOUIdsNameForGeoCoordinates 29.973433, -95.243265, '1f82605d3fe666478f3f4f1ee25ae828'
                var zapierOusList = dc
             .Database
             .SqlQuery<zapierOus>("exec usp_GetOUIdsNameForGeoCoordinates @latitude, @longitude, @apiKey", new SqlParameter("@latitude", Lat), new SqlParameter("@longitude", Lon), new SqlParameter("@apiKey", apiKey))
             .ToList();

                return zapierOusList;
            }
        }

        public string GetApikeyByOU(Guid ouid)
        {
            // validate apiKey
            var sbSettings = _settingsService
                                .Value
                                .GetSettingsByOUID(ouid)
                                ?.FirstOrDefault(x => x.Name == SolarTrackerResources.SelectedSettingName)
                                ?.GetValue<ICollection<SelectedIntegrationOption>>()?
                                .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                .Data?
                                .SMARTBoard;

            return sbSettings.ApiKey;
        }

        public IEnumerable<Territories> GetTerritoriesListByOu(float? Lat, float? Lon, Guid ouid)
        {
            Lat = Lat == null ? 0 : Lat;
            Lon = Lon == null ? 0 : Lon;
            using (var dc = new DataContext())
            {
                //-- exec usp_GetTerritoryIdsNameByapiKey 29.920071,-95.498855,NULL,'1E9E5809-45F2-4CEE-AACB-6617DD232A40'
                var TerritoriesList = dc
             .Database
             .SqlQuery<Territories>("exec usp_GetTerritoryIdsNameByapiKey @latitude, @longitude, @apiKey, @ouid", new SqlParameter("@latitude", Lat), new SqlParameter("@longitude", Lon), new SqlParameter("@apiKey", "NULL"), new SqlParameter("@ouid", ouid))
             .ToList();

                return TerritoriesList;
            }
        }

        public IEnumerable<OURole> GetOuRoles()
        {
            using (var dc = new DataContext())
            {
                var ouRoles = dc.OURoles.Where(a => a.IsActive == true).ToList();
                return ouRoles;
            }
        }

        public void UpdateOuRoles(List<OURole> roles)
        {
            using (var dc = new DataContext())
            {
                foreach (var role in roles)
                {
                    var ourole = dc.OURoles.FirstOrDefault(a => a.Guid == role.Guid);
                    role.Permissions = ourole.Permissions;
                    ourole.Updated(role.Guid);
                }

                dc.SaveChanges();
            }
        }

        public IEnumerable<GuidNamePair> SBGetOuRoles()
        {
            using (var dc = new DataContext())
            {
                var ouRoles = dc.OURoles.Where(a => a.IsActive == true).Select(o => new GuidNamePair
                {
                    Guid = o.Guid,
                    Name = o.Name,
                }).ToList(); ;

                return ouRoles;
            }
        }


        public OURole GetOuRoleByID(Guid? roleid)
        {
            using (var dc = new DataContext())
            {
                var ouRoles = dc.OURoles.FirstOrDefault(a => a.Guid == roleid);
                if (ouRoles == null)
                {
                    throw new ApplicationException("OU does not exist!");
                }

                return ouRoles;
            }
        }

        public bool UpdateOuRolesPermission(List<OURole> roles)
        {
            try
            {
                using (var dc = new DataContext())
                {
                    foreach (var role in roles)
                    {
                        var ourole = dc.OURoles.FirstOrDefault(a => a.Guid == role.Guid);
                        ourole.Permissions = role.Permissions;
                        ourole.Updated(ourole.Guid);
                    }

                    dc.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void CreateNewOURole(OURole req)
        {
            using (var dc = new DataContext())
            {
                using (var transaction = dc.Database.BeginTransaction())
                {
                    try
                    {
                        // Create the OU
                        var ourole = new OURole
                        {
                            Name = req.Name,
                            IsActive = req.IsActive,
                            IsAdmin = req.IsAdmin
                        };

                        dc.OURoles.Add(ourole);
                        dc.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }


        public void EditOURole(Guid ouid, OURole req)
        {
            using (var dc = new DataContext())
            {
                var role = dc
                            .OURoles
                            .FirstOrDefault(o => o.Guid == ouid);

                if (role == null)
                {
                    throw new ApplicationException("OU Role does not exist!");
                }

                role.Name = req.Name;
                role.IsActive = req.IsActive;
                role.IsAdmin = req.IsAdmin;
                role.Updated(SmartPrincipal.UserId, "InternalPortal");

                dc.SaveChanges();
            }
        }

        /// <summary>
        /// This method insert Ou as a Favorite 
        /// </summary>
        public FavouriteOu InsertFavouriteOu(Guid ouID, Guid personID)
        {
            using (var dc = new DataContext())
            {
                var FavouriteOu = dc.FavouriteOus.FirstOrDefault(x => x.PersonID == personID && x.OUID == ouID);

                if (FavouriteOu != null)
                    throw new ApplicationException("Already Favourited");

                var ou = new FavouriteOu
                {
                    OUID = ouID,
                    PersonID = personID,
                    CreatedByID = SmartPrincipal.UserId
                };

                dc.FavouriteOus.Add(ou);
                dc.SaveChanges();

                return ou;
            }
        }

        /// <summary>
        /// This method remove Ou as a Favorite 
        /// </summary>
        public void RemoveFavouriteOu(Guid ouID, Guid personID)
        {
            using (var dc = new DataContext())
            {
                var FavouriteOu = dc.FavouriteOus.FirstOrDefault(x => x.PersonID == personID && x.OUID == ouID);

                if (FavouriteOu == null)
                    throw new ApplicationException("OU not found");

                dc.FavouriteOus.Remove(FavouriteOu);
                dc.SaveChanges();
            }
        }
    }

    //public void UpdateFinancing()
    //{
    //    using (var dc = new DataContext())
    //    {
    //        var ouIDs = dc
    //                    .OUFinanceAssociations
    //                    .Select(oua => oua.OUID)
    //                    .Distinct()
    //                    .ToList();

    //        var plans = dc
    //                    .OUFinanceAssociations
    //                    .ToList();

    //        var settings = dc
    //                    .OUSettings
    //                    .Where(ous => ouIDs.Contains(ous.Guid) && ous.Name == OUSetting.Financing_PlansOrder)
    //                    .ToList();

    //        var cashPlanId = dc
    //                    .FinancePlaneDefinitions
    //                    .FirstOrDefault(fp => fp.Type == FinancePlanType.Cash)
    //                    .Guid;
    //        var mortgagePlanId = dc
    //                    .FinancePlaneDefinitions
    //                    .FirstOrDefault(fp => fp.Type == FinancePlanType.Mortgage)
    //                    .Guid;

    //        var existingOUSettings = dc
    //                    .OUSettings
    //                    .Where(ous => ouIDs.Contains(ous.OUID) && ous.Name == OUSetting.Financing_Options)
    //                    .ToList();

    //        var newOUSettings = new List<OUSetting>();

    //        foreach (var id in ouIDs)
    //        {
    //            var ouSetting = settings
    //                                .FirstOrDefault(s => s.OUID == id);
    //            List<Guid> enabledPlans = null;

    //            if (ouSetting != null)
    //            {
    //                try
    //                {
    //                    enabledPlans = JsonConvert.DeserializeObject<List<Guid>>(ouSetting.Value);
    //                }
    //                catch (Exception)
    //                { }
    //            }
    //            var ouPlanIds = plans
    //                        .Where(p => p.OUID == id)
    //                        .Select(p => p.FinancePlanDefinitionID)
    //                        .ToList();

    //            var ouPlans = new HashSet<Guid>(ouPlanIds);
    //            ouPlans.Add(cashPlanId);
    //            ouPlans.Add(mortgagePlanId);

    //            var data = ouPlans
    //                    .Select(
    //                    (p, idx) => new FinancingSettingsDataView(p, (enabledPlans == null || enabledPlans?.Count == 0) || (enabledPlans?.Count > 0 && enabledPlans?.Contains(p) == true), idx, p))
    //                    .ToList();

    //            var value = JsonConvert.SerializeObject(data);

    //            existingOUSettings.HandleSetting(newOUSettings, OUSetting.Financing_Options, value, id, _auditService, OUSettingGroupType.DealerSettings);
    //        }

    //        if (newOUSettings.Count > 0)
    //        {
    //            dc.OUSettings.AddRange(newOUSettings);
    //        }
    //        dc.SaveChanges();
    //    }
    //}
}

