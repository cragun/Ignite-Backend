using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Inquiries;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.OUs;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Finance;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using WebApi.OutputCache.V2;
using GoogleMaps.LocationServices;
using System.Threading.Tasks;

namespace DataReef.TM.Api.Controllers
{
    [RoutePrefix("api/v1/ous")]
    [CacheOutput(ClientTimeSpan = 300, ServerTimeSpan = 300)]
    [AutoInvalidateCacheOutput]
    public class OUsController : EntityCrudController<OU>
    {
        private readonly IOUService ouService;
        private readonly IOUSettingService settingsService;
        private readonly IPropertyService propertyService;
        private readonly IDataService<User> userService;
        private readonly IPersonService personService;
        private readonly IDataService<OUAssociation> associationService;

        public OUsController(IOUService ouService,
                             IOUSettingService ouSettingsService,
                             IPropertyService propertyService,
                             IDataService<User> userService,
                             IPersonService personService,
                             IDataService<OUAssociation> associationService,
                             ILogger logger)
            : base(ouService, logger)
        {
            this.ouService = ouService;
            this.settingsService = ouSettingsService;
            this.propertyService = propertyService;
            this.userService = userService;
            this.personService = personService;
            this.associationService = associationService;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<OU>))]
        [Route("roots")]
        public async Task<IHttpActionResult> GetRootOUs(string include = "", string exclude = "", string fields = "", string query = "")
        {
            Guid personID = SmartPrincipal.UserId;
            ICollection<OU> ret = ouService.ListRootsForPerson(personID, include, exclude, fields, query);
            SetupSerialization(ret, include, exclude, fields);
            return Ok(ret);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Person>))]
        [Route("{ouID:guid}/people")]
        public async Task<IHttpActionResult> GetPeople(Guid ouID, string include = "", string fields = "", bool deepSearch = true)
        {
            var personIds = this.ouService.ConditionalGetActivePeopleIDsForCurrentAndSubOUs(ouID, deepSearch);
            var ret = personService.GetMany(personIds, include, fields);
            SetupSerialization(ret, include, string.Empty, fields);
            return Ok<ICollection<Person>>(ret);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<User>))]
        [Route("{ouID:guid}/users")]
        public async Task<IHttpActionResult> GetAllUsersForCurrentAndSubOUs(Guid ouID, string include = "", string fields = "", bool deepSearch = true)
        {
            var userIds = this.ouService.ConditionalGetActiveUserIDsForCurrentAndSubOUs(ouID, deepSearch);
            var ret = userService.GetMany(userIds, include, fields);
            SetupSerialization(ret, include, string.Empty, fields);
            return Ok<ICollection<User>>(ret);
        }

        [HttpGet, AllowAnonymous, InjectAuthPrincipal]
        [Route("{ouID:guid}/users/active")]
        public async Task<HttpResponseMessage> GetActiveUsers(Guid ouID)
        {
            var data = ouService.GetActiveUsersCSV(ouID);

            MemoryStream ms = new MemoryStream(data.ActiveUsersCSV);
            HttpResponseMessage response = new HttpResponseMessage { Content = new StreamContent(ms) };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = $"{data.OUName}_{DateTime.UtcNow:yyyy-MM-dd}.csv" };
            response.Content.Headers.ContentLength = ms.Length;
            return response;
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<OUAssociation>))]
        [Route("{ouID:guid}/ouassociations")]
        public async Task<IHttpActionResult> GetOUAssociations(Guid ouID, string include = "", string exclude = "", string fields = "", bool deepSearch = true)
        {
            var ouAssociationIds = this.ouService.ConditionalGetActiveOUAssociationIDsForCurrentAndSubOUs(ouID, deepSearch);
            var ret = associationService.GetMany(ouAssociationIds, include, exclude, fields);
            SetupSerialization(ret, include, exclude, fields);
            return Ok<ICollection<OUAssociation>>(ret);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<FinancePlanDefinition>))]
        [Route("{ouID:guid}/financeplandefinitions")]
        public async Task<IHttpActionResult> GetFinancePlanDefinitions(Guid ouID, string include = "", string exclude = "", string fields = "")
        {
            var result = ouService.GetFinancePlanDefinitions(ouID, include, exclude, fields);
            return Ok(result);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Territory>))]
        [Route("{ouID:guid}/territories/{territoryName}")]
        public async Task<IHttpActionResult> GetFilteredByNameTerritoriesForCurrentAndSubOUs(Guid ouID, string territoryName, string include = "", string fields = "", bool deletedItems = false)
        {
            ICollection<Territory> ret = this.ouService.GetOUTreeTerritories(ouID, territoryName, deletedItems);
            SetupSerialization(ret, include, string.Empty, fields);
            return Ok<ICollection<Territory>>(ret);
        }

        [HttpGet]
        [ResponseType(typeof(List<GuidNamePair>))]
        [Route("~/api/v1/subous")] // overwritten route prefix to change "api/v1/ous/subous" to "api/v1/subous" because of a mapping issue in the client app 
        public async Task<IHttpActionResult> GetAllSubOUsOfSpecifiedOus(string ouIDs)
        {
            if (String.IsNullOrWhiteSpace(ouIDs)) return BadRequest();
            return Ok(ouService.GetAllSubOUIdsAndNamesOfSpecifiedOus(ouIDs));
        }

        [HttpGet]
        [ResponseType(typeof(List<OUWithAncestors>))]
        [Route("{ouID:guid}/subous/setting")]
        public async Task<IHttpActionResult> GetSubOUsOfSpecifiedOuWithSetting(Guid ouID, string settingName)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                return BadRequest();
            }
            return Ok(ouService.GetSubOUsForOuSetting(ouID, settingName));
        }

        /// <summary>
        /// Retuns all OUs for the given user.  Does not return the OU shapes or WellKnownText. This api is meant for performanace
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ICollection<OU>))]
        [Route("mine")]
        public async Task<IHttpActionResult> GetAllOUsForUser()
        {
            Guid userID = SmartPrincipal.UserId;
            List<OU> ret = ouService.ListAllForPerson(userID).ToList();
            return Ok<ICollection<OU>>(ret);
        }

        /// <summary>
        /// Returns all OUs that a user has access to filtered by whether they have a certain setting
        /// </summary>
        /// <param name="settingName"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ICollection<OU>))]
        [Route("mine/setting")]
        public async Task<IHttpActionResult> GetAllOUsForUserBySetting(string settingName)
        {
            Guid userID = SmartPrincipal.UserId;
            List<OU> ret = ouService.ListAllForPersonAndSetting(userID, settingName).ToList();
            return Ok<ICollection<OU>>(ret);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Property>))]
        [Route("{ouID:guid}/properties")]
        public async Task<IHttpActionResult> GetPropertiesByStatus(Guid ouID, string disposition, string propertyNameSearch = null, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            var ret = propertyService.GetOUPropertiesByStatusPaged(ouID, disposition, propertyNameSearch, pageIndex, itemsPerPage, include, exclude);
            return Ok<ICollection<Property>>(ret);
        }

        [HttpGet]
        [ResponseType(typeof(ICollection<Property>))]
        [Route("{ouID:guid}/properties/withproposal")]
        public async Task<IHttpActionResult> GetPropertiesWithProposal(Guid ouID, string propertyNameSearch = null, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            var ret = propertyService.GetOUPropertiesWithProposal(ouID, propertyNameSearch, pageIndex, itemsPerPage, include, exclude);
            SetupSerialization(ret, include, exclude, null);
            return Ok(ret);
        }

        [HttpGet]
        [Route("{ouID:guid}/settings/{group?}")]
        [ResponseType(typeof(Dictionary<string, ValueTypePair<SettingValueType, string>>))]
        public async Task<IHttpActionResult> GetAllSettings(Guid ouID, OUSettingGroupType? group = null)
        {
            var data = settingsService.GetSettings(ouID, group);
            return Ok(data);
        }

        [HttpPost]
        [Route("{ouID:guid}/settings/{group?}")]
        [ResponseType(typeof(Dictionary<string, ValueTypePair<SettingValueType, string>>))]
        public async Task<IHttpActionResult> SetAllSettings([FromUri]Guid ouID, [FromBody]Dictionary<string, ValueTypePair<SettingValueType, string>> data, [FromUri]OUSettingGroupType? group = null)
        {
            settingsService.SetSettings(ouID, group, data);
            return Ok();
        }

        [HttpPost]
        [Route("people")]
        public async Task<IHttpActionResult> GetMembers([FromBody]OUMembersRequest request)
        {
            var data = ouService.GetMembers(request);
            return Ok(data);
        }

        [HttpPost]
        [Route("{ouID:guid}/move")]
        public async Task<IHttpActionResult> MoveOU(Guid ouID, Guid newParentOUID)
        {
            ouService.MoveOU(ouID, newParentOUID);
            return Ok();
        }

        [HttpGet]
        [Route("{ouID:Guid}/inquiryStats")]
        [ResponseType(typeof(ICollection<InquiryStatisticsForOrganization>))]
        [IgnoreCacheOutput]
        public async Task<IHttpActionResult> GetOUSummaryForCustomStatuses(Guid ouID, DateTime? specifiedDay = null)
        {
            var data = ouService.GetInquiryStatisticsForOrganization(ouID, null, specifiedDay);
            return Ok(data);
        }

        [HttpGet]
        [Route("{ouID:guid}/tokenPrice")]
        public async Task<IHttpActionResult> GetTokenPriceInDollars(Guid ouID)
        {
            var tokenPriceInDollars = this.ouService.GetTokenPriceInDollars(ouID);
            return Ok(new { PricePerToken = tokenPriceInDollars });
        }

        [HttpPost]
        [Route("{ouid:guid}/getbyshapesversion")]
        [ResponseType(typeof(OU))]
        public async Task<IHttpActionResult> GetByShapesVersion([FromBody] ICollection<OuShapeVersion> shapesVersion, Guid ouid, bool deletedItems = false, string include = "")
        {
            var ou = ouService.GetByShapesVersion(ouid, shapesVersion, deletedItems, include);

            return Ok(ou);
        }

        [HttpGet]
        [Route("{ouid:guid}/withancestors")]
        [ResponseType(typeof(OU))]
        public async Task<OU> GetWithAncestors(Guid ouid, string include = "", string exclude = "", string fields = "", bool summary = true, string query = "")
        {
            var entity = ouService.GetWithAncestors(ouid, include, exclude, fields, summary, query);
            entity.SetupSerialization(include, exclude, fields);
            return entity;
        }

        [HttpGet]
        [Route("{ouid:guid}/childrenandterritories")]
        [ResponseType(typeof(OUChildrenAndTerritories))]
        public async Task<OUChildrenAndTerritories> GetWithChildrenAndTerritories(Guid ouid)
        {
            return ouService.GetOUWithChildrenAnTerritories(ouid);
        }


        [HttpGet]
        [InjectAuthPrincipal]
        [AllowAnonymous]
        [Route("tree/{personID}")]
        public async Task<IHttpActionResult> TestOUTree(Guid personID)
        {
            var result = ouService.GetOUsRoleTree(personID);
            //var result = personService.CRMGetProperties(new Models.DTOs.Inquiries.CRMFilterRequest());
            return Ok(result);
        }

        /// <summary>
        /// Get method used by SmartBoard to retrieve the specified OU
        /// </summary>
        /// <param name="apiKey"></param>
        [HttpGet, Route("sb/roles/{apiKey}")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(IEnumerable<SBOURoleDTO>))]
        public async Task<IEnumerable<SBOURoleDTO>> GetRoles(string apiKey)
        {
            return ouService.GetAllRoles(apiKey);
        }

        /// <summary>
        /// Get method used by SmartBoard to retrieve the specified OU
        /// </summary>
        /// <param name="ouID"></param>
        /// <param name="apiKey"></param>
        [HttpGet, Route("sb/{ouID}/{apiKey}")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(SBOUDTO))]
        public async Task<SBOUDTO> GetOus(Guid ouID, string apiKey)
        {
            return ouService.GetSmartboardOus(ouID, apiKey);
        }

        /// <summary>
        /// Get method used by SmartBoard to retrieve all accessible OUs
        /// </summary>
        /// <param name="apiKey"></param>
        [HttpGet, Route("sb/{apiKey}")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(IEnumerable<SBOUDTO>))]
        public async Task<IEnumerable<SBOUDTO>> GetAllAccessibleOus(string apiKey)
        {
            return ouService.GetSmartboardAllOus(apiKey);
        }


        /// <summary>
        /// Gets all Ous by location Address for Zapier Leads
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <param name="Country"></param>
        /// <param name="Zip"></param>
        [HttpGet, Route("sbzapierOus")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(zapierOusModel))]

        // api/v1/ous/sbzapierOus?Address=&City=&State=&Country=&Zip=
        // api/v1/ous/sbzapierOus?Address=Rue du Cornet 6&City=VERVIERS&State=null&Country=Belgium&Zip=B-4800
        public async Task<IHttpActionResult> GetzapierOus(string Address, string City, string State, string Country, string Zip)
        {
            try
            {
                AddressData a = new AddressData { Address = Address, City = City, State = State, Country = Country, Zip = Zip };
                var gls = new GoogleLocationService(DataReef.Core.Constants.GoogleLocationApikey);
                var latlong = gls.GetLatLongFromAddress(a);
                if (latlong == null)
                    return null;
                zapierOusModel zapierou = new zapierOusModel();
                zapierou.Latitude = (float)latlong.Latitude;
                zapierou.Longitude = (float)latlong.Longitude;
                zapierou.ouslist = ouService.GetzapierOusList((float)latlong.Latitude, (float)latlong.Longitude, " ");
                return Ok(zapierou);
            }
            catch (System.Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// Gets all Territories by lat,long and ouid for Zapier Leads
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="ouid"></param>
        [HttpGet, Route("TerritoriesByOu")]
        [AllowAnonymous, InjectAuthPrincipal]
        [ResponseType(typeof(TerritoryModel))]


        // api/v1/ous/TerritoriesByOu?latitude=&longitude=&ouid=
        public async Task<IHttpActionResult> GetTerritoriesByOu(float? latitude, float? longitude, Guid ouid)
        {
            try
            {
                TerritoryModel teritory = new TerritoryModel();
                teritory.apikey = ouService.GetApikeyByOU(ouid);
                teritory.TerritorieswithLatLong = ouService.GetTerritoriesListByOu(latitude, longitude, ouid);
                teritory.Territories = ouService.GetTerritoriesListByOu(0, 0, ouid);
                return Ok(teritory);
            }
            catch (System.Exception)
            {
                throw;
            }
        }


        protected override string PrepareEntityForNavigationPropertiesAttachment(OU entity)
        {
            var includeProperties = new List<string>();

            if (entity.Shapes != null)
            {
                entity
                    .Shapes
                    .ToList()
                    .ForEach(s => s.OUID = entity.Guid);
                includeProperties.Add("Shapes");
            }
            if (entity.Territories != null)
            {
                entity
                    .Territories
                    .ToList()
                    .ForEach(t => t.OUID = entity.Guid);
                includeProperties.Add("Territories");
            }
            if (entity.Associations != null)
            {
                entity
                    .Associations
                    .ToList()
                    .ForEach(a => a.OUID = entity.Guid);
                includeProperties.Add("Associations");
            }

            return string.Join(",", includeProperties);
        }

        /// <summary>
        /// Checks whether an OU is valid: has WKT and Shapes.
        /// </summary>
        /// <param name="item"></param>
        public void CheckOUValidity(OU item)
        {
            if (item.Shapes == null || item.Shapes.Count == 0 || String.IsNullOrWhiteSpace(item.WellKnownText))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        public override async Task<OU> Get(Guid guid, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            return await base.Get(guid, include, exclude, fields, deletedItems);
        }

        public override async Task<ICollection<OU>> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "")
        {
            return await base.List(deletedItems, pageNumber, itemsPerPage, include, exclude, fields);
        }

        public override async Task<IEnumerable<OU>> GetMany(string delimitedStringOfGuids, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            return await base.GetMany(delimitedStringOfGuids, include, exclude, fields, deletedItems);
        }

        public override async Task<IHttpActionResult> GetCollection(Guid guid, string collectionName, int pageNumber = 1, int itemsPerPage = 20, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            return await base.GetCollection(guid, collectionName, pageNumber, itemsPerPage, include, exclude, fields, deletedItems);
        }

        public override async Task<HttpResponseMessage> DeleteByGuid(Guid guid)
        {
            return await base.DeleteByGuid(guid);
        }


        public override async Task<OU> Post(OU item)
        {
            CheckOUValidity(item);

            return await base.Post(item);
        }

        public override async Task<OU> Patch(System.Web.Http.OData.Delta<OU> item)
        {
            CheckOUValidity(item.GetEntity());

            return await base.Patch(item);
        }

        public override async Task<HttpResponseMessage> Delete(OU item)
        {
            return await base.Delete(item);
        }

        public override async Task<ICollection<OU>> PostMany(List<OU> items)
        {
            if (items == null || !items.Any())
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            items.ForEach(i => CheckOUValidity(i));

            return await base.PostMany(items);
        }

        public override async Task<OU> Put(OU item)
        {
            CheckOUValidity(item);

            return await base.Put(item);
        }
    }
}
