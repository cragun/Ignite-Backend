using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations.MailChimp;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.Client;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs.Common;
using DataReef.TM.Models.DTOs.Inquiries;
using DataReef.TM.Models.DTOs.Persons;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services.Extensions;
using DataReef.TM.Services.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PersonService : DataService<Person>, IPersonService
    {
        private const int INTEGRATION_TOKEN_EXPIRATION_DAYS = 30;
        private readonly IOUAssociationService _ouAssociationService;
        private readonly Lazy<IMailChimpAdapter> _mailChimpAdapter;
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IOUSettingService> _ouSettingsService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _sbAdapter;


        public PersonService(ILogger logger,
            IOUAssociationService ouAssociationService,
            Lazy<IMailChimpAdapter> mailChimpAdapter,
            Lazy<IOUService> ouService,
            Lazy<IOUSettingService> ouSettingsService,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter,
            Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _ouAssociationService = ouAssociationService;
            _mailChimpAdapter = mailChimpAdapter;
            _ouService = ouService;
            _sbAdapter = sbAdapter;
            _ouSettingsService = ouSettingsService;
        }

        internal static void PopulateSummary(ICollection<Person> people)
        {
            using (DataContext dc = new DataContext())
            {
                foreach (Person p in people)
                {
                    PersonSummary summary = dc.Database.SqlQuery<PersonSummary>("exec proc_PersonAnalytics {0}", p.Guid).FirstOrDefault();
                    p.Summary = summary;
                }
            }
        }

        public override Person Insert(Person entity)
        {
            entity.ModifiedTime = DateTime.UtcNow;
            var ret = base.Insert(entity);

            //foreach(OUAssociation oua in entity.OUAssociations)
            //{
            //    base.ProcessApiWebHooks(entity, Models.Enums.EventDomain.User, Models.Enums.EventAction.Created, oua.OUID);
            //}

            return ret;
        }

        public override Person Update(Person entity)
        {
            if (entity.FullName != entity.Name)
            {
                entity.Name = entity.FullName;
            }
            entity.ModifiedTime = DateTime.UtcNow;
            var ret = base.Update(entity);

            //foreach (OUAssociation oua in entity.OUAssociations)
            //{
            //    base.ProcessApiWebHooks(entity, Models.Enums.EventDomain.User, Models.Enums.EventAction.Changed, oua.OUID);
            //}

            return ret;
        }

        public void UpdateStartDate()
        {
            using (DataContext dc = new DataContext())
            {
                var person = dc.People.Where(x => x.IsDeleted == false && x.Guid == SmartPrincipal.UserId).FirstOrDefault();

                if (person != null)
                {
                    person.StartDate = DateTime.UtcNow;
                    person.ModifiedTime = DateTime.UtcNow;
                    var ret = base.Update(person);

                    if (!string.IsNullOrEmpty(person.SmartBoardID))
                    {
                        _sbAdapter.Value.SBUpdateactivityUser(person.SmartBoardID, person.ActivityName, person.BuildVersion, person.LastActivityDate, person.Guid, person.StartDate);
                    }
                }
            }
        }


        public Person Updateactivity(Person prsn)
        {
            if(prsn.SmartBoardID != null)
            {
                using (DataContext dc = new DataContext())
                {
                    var person = dc.People.SingleOrDefault(p => (p.SmartBoardID == prsn.SmartBoardID) && p.IsDeleted == false);
                    if (person == null)
                    {
                        throw new ArgumentException("Couldn't find the person among the deleted ones!");
                    }

                    person.SBActivityName = prsn.SBActivityName;
                    person.BuildVersion = prsn.BuildVersion;
                    person.SBLastActivityDate = prsn.SBLastActivityDate;

                    person.ModifiedTime = DateTime.UtcNow;
                    var ret = base.Update(person);
                    return ret;
                }
            }


            if (prsn.Guid != null)
            {
                var prsndetails = Get(prsn.Guid);
                prsndetails.ActivityName = prsn.ActivityName;
                prsndetails.BuildVersion = prsn.BuildVersion;
                prsndetails.LastActivityDate = prsn.LastActivityDate;

                prsndetails.ModifiedTime = DateTime.UtcNow;
                var ret = base.Update(prsndetails);

                if (!string.IsNullOrEmpty(prsndetails.SmartBoardID))
                {
                    _sbAdapter.Value.SBUpdateactivityUser(prsn.SmartBoardID , prsndetails.ActivityName , prsndetails.BuildVersion, prsndetails.LastActivityDate, prsndetails.Guid, prsndetails.StartDate);
                }
               
                return ret;
            }

            return prsn;

        }


        public override ICollection<Person> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            ICollection<Person> ret = base.List(deletedItems, pageNumber, itemsPerPage, filter, include, exclude, fields);
            PopulateSummary(ret);
            return ret;
        }

        public override Person Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            return GetMayEdit(uniqueId, false, include, exclude, fields, deletedItems);
        }

        public Person GetMayEdit(Guid uniqueId, bool mayEdit, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            Person ret = base.Get(uniqueId, include, exclude, fields, deletedItems);
            if (mayEdit)
            {
                _ouAssociationService.PopulatePersonMayEdit(new List<Person> { ret });
            }
            PopulateSummary(ret);
            return ret;
        }

        public List<PersonLite> GetPeopleForOU(Guid ouID, bool deep)
        {
            var results = new List<PersonLite>();

            using (DataContext dc = new DataContext())
            {
                results = dc.Database.SqlQuery<PersonLite>("exec [proc_PeopleLiteForOU] {0},{1}", ouID, deep).ToList();
            }

            return results;
        }

        public List<Person> GetMine(OUMembersRequest request)
        {
            var result = new List<Person>();
            if (request.ExcludeOUs == null) request.ExcludeOUs = new List<Guid>();

            // get only the effective OU associations of the current user
            var currentUsersAssociations = _ouAssociationService.SmartList(include: "OURole,OU,OU.RootOrganization", filter: String.Format("Personid={0}", SmartPrincipal.UserId));

            using (DataContext dc = new DataContext())
            {
                foreach (var currentUserAssociation in currentUsersAssociations)
                {
                    if (request.ExcludeOUs.Contains(currentUserAssociation.OUID)) continue;

                    // get all sub-OUs for the current OU association
                    var currentOUSubOrganizationIds = dc
                        .Database
                        .SqlQuery<OU>("exec [proc_SelectOUHierarchy] {0}", currentUserAssociation.OUID)
                        .Where(o => !o.IsDeleted && !request.ExcludeOUs.Contains(o.Guid))
                        .Select(o => o.Guid)
                        .ToList();

                    var peopleQuery = dc.OUAssociations
                                    .Where(oua => currentOUSubOrganizationIds.Contains(oua.OUID))
                                    .Select(oua => oua.Person)
                                    .Where(p => (!request.IsDeleted && p.IsDeleted == false)
                                             || (request.IsDeleted && !request.OnlyDeleted)
                                             || (request.IsDeleted && request.OnlyDeleted && p.IsDeleted == true))
                                    .Distinct();

                    AssignIncludes<Person>(request.Include, ref peopleQuery);
                    AssignFilters<Person>(request.Filter, ref peopleQuery);
                    AssignFilters<Person>(request.Query, FilterLinkingOperator.Or, ref peopleQuery);
                    var peopleList = peopleQuery.ToList();
                    foreach (var person in peopleList)
                    {
                        if (person.IsDeleted == true)
                        {
                            continue;
                        }
                        if (result.Any(r => r.Guid == person.Guid))
                        {
                            continue;
                        }

                        result.Add(person);
                    }
                }
            }

            request.SortColumn = request.SortColumn ?? "FirstName";
            request.SortOrder = request.SortOrder ?? "asc";

            var sortCondition = string.Format("{0} {1}", request.SortColumn, request.SortOrder);

            var sortedResult = result
                        .OrderBy(sortCondition)
                        .Skip((request.PageNumber - 1) * request.ItemsPerPage)
                        .Take(request.ItemsPerPage)
                        .ToList();

            PopulateSummary(sortedResult);

            return sortedResult;
        }

        /// <summary>
        /// Method used to undelete a person, w/ associated User and Credential
        /// This method will also send an email letting the person know that the account has been reactivated
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="smartBoardId"></param>
        /// <param name="environment"></param>
        public void Reactivate(Guid personId, string smartBoardId)
        {

            using (DataContext dc = new DataContext())
            {
                var person = dc.People.Where(p => (p.Guid == personId || (smartBoardId != null && p.SmartBoardID.Equals(smartBoardId, StringComparison.InvariantCultureIgnoreCase))) && p.IsDeleted == true).FirstOrDefault();

                if (person == null)
                {
                    throw new ArgumentException("Couldn't find the person among the deleted ones!");
                }

                person.IsDeleted = false;
                person.ModifiedTime = DateTime.UtcNow;

                var user = dc
                            .Users
                            .SingleOrDefault(p => p.Guid == person.Guid);
                if (user != null && user.IsDeleted)
                {
                    user.IsDeleted = false;
                }

                var credentials = dc
                                    .Credentials
                                    .Where(c => c.UserID == person.Guid)
                                    .ToList();
                if (credentials != null && credentials.Any())
                {
                    credentials.ForEach(c => c.IsDeleted = false);
                }

                var LastAuthRecord = dc.Authentications.Where(a => a.UserID == person.Guid).ToList().OrderByDescending(x => x.DateAuthenticated).FirstOrDefault();
                if (LastAuthRecord != null)
                {
                    LastAuthRecord.DateAuthenticated = DateTime.UtcNow;
                }

                dc.SaveChanges();

                if (string.IsNullOrEmpty(smartBoardId))
                {
                    //the method also updates the Ignite user's SmartBoardId property
                    _sbAdapter.Value.SBActiveDeactiveUser(false, person.SmartBoardID);
                }

                var template = new ReactivateAccountTemplate
                {
                    ToPersonName = string.Format("{0} {1}", person.FirstName, person.LastName),
                    ToPersonEmail = person.EmailAddressString,
                    RecipientEmailAddress = person.EmailAddressString,
                    DownloadURL = ConfigurationManager.AppSettings["LegionDownloadURL"]
                };
                Mail.Library.SendUserReactivationEmail(template);

                //  register to MailChimp
                try
                {
                    foreach (var credential in credentials)
                    {
                        _mailChimpAdapter.Value.RegisterUser(credential.UserName);
                    }
                }
                catch (Exception)
                {
                }
            }
        }




        public void DeactivateUser(string smartBoardId)
        {
            using (DataContext dc = new DataContext())
            {
                var person = dc.People.SingleOrDefault(p => p.SmartBoardID == smartBoardId && p.IsDeleted == false);
                if (person == null)
                {
                    throw new ArgumentException("Couldn't find the person among the deleted ones!");
                }
                Delete(person.Guid);
            }
        }


        public void SBDeactivate(Guid personid)
        {
            using (DataContext dc = new DataContext())
            {
                var person = dc.People.SingleOrDefault(p => p.Guid == personid);
                if (person == null)
                {
                    throw new ArgumentException("Couldn't find the person among the deleted ones!");
                }
                _sbAdapter.Value.SBActiveDeactiveUser(true, person.SmartBoardID);
            }
        }



        public override SaveResult Delete(Guid uniqueId)
        {
            List<Credential> credentials;
            using (var uow = UnitOfWorkFactory())
            {
                var person = uow.Get<Person>()
                    .Include(p => p.ConnectionInvitationsReceived)
                    .FirstOrDefault(p => p.Guid == uniqueId);
                if (person == null)
                    return new SaveResult
                    {
                        Action = DataAction.None,
                        EntityUniqueId = uniqueId,
                        Success = false
                    };



                // Get All UserInvitation ids that need to be deleted
                var ids = person.ConnectionInvitationsReceived?.Select(ui => ui.Guid)
                    .ToList() ?? new List<Guid>();
                var sentIds = person.ConnectionInvitationsSent?.Where(c => c.Status != InvitationStatus.Accepted)
                    .Select(c => c.Guid) ?? new List<Guid>();
                ids.AddRange(sentIds);

                var invitations = uow.Get<UserInvitation>()
                    .Where(ui => ids.Contains(ui.Guid))
                    .ToList();
                invitations.ForEach(i => uow.Delete(i));

                uow.SaveChanges();

                credentials = uow.Get<Credential>().Where(c => c.UserID == uniqueId).ToList();
            }

            //  unregister user from MailChimp
            try
            {
                foreach (var credential in credentials)
                {
                    _mailChimpAdapter.Value.UnregisterUser(credential.UserName);
                }
            }
            catch (Exception)
            {
            }

            var result = base.Delete(uniqueId);
            return result;
        }

        public IntegrationToken GenerateIntegrationToken()
        {
            using (var uow = UnitOfWorkFactory())
            {
                // if the last token is valid for at least 5 hours, reuse it.
                var lastToken = uow
                                .Get<IntegrationToken>()
                                .Where(it => it.UserId == SmartPrincipal.UserId)
                                .OrderByDescending(it => it.ExpirationDate)
                                .FirstOrDefault();

                if (lastToken != null && lastToken.ExpirationDate > DateTime.UtcNow.AddHours(5))
                {
                    return lastToken;
                }

                lastToken = new IntegrationToken
                {
                    UserId = SmartPrincipal.UserId,
                    ExpirationDate = DateTime.UtcNow.AddDays(INTEGRATION_TOKEN_EXPIRATION_DAYS)
                };
                uow.Add(lastToken);
                uow.SaveChanges();

                return lastToken;
            }
        }

        public List<CRMDisposition> CRMGetAvailableDispositions()
        {
            // Get all the OUs for the logged in user

            var rootGuids = _ouService.Value.ListRootGuidsForPerson(SmartPrincipal.UserId);

            var settings = _ouSettingsService
                        .Value
                        .GetOuSettingsMany(rootGuids)?
                        .SelectMany(os => os.Value)?
                        .ToList();
            var dispSettings = settings?
                    .Where(s => s.Name == OUSetting.NewDispositions)?
                    .ToList();

            var dispositions = dispSettings?
                    .SelectMany(s => JsonConvert.DeserializeObject<List<DispositionV2DataView>>(s.Value))?
                    .Select(d => d.Name)?
                    .ToList() ?? new List<string>();

            var distinctDispositions = new HashSet<string>(dispositions);
            distinctDispositions.Add("With Proposal");
            distinctDispositions.Add("Appointments");

            return distinctDispositions.Select(d => new CRMDisposition { Disposition = d }).ToList();
        }

        public List<CRMDisposition> CRMGetAvailableNewDispositions()
        {
            // Get all the OUs for the logged in user

            var rootGuids = _ouService.Value.ListRootGuidsForPerson(SmartPrincipal.UserId);

            var settings = _ouSettingsService
                        .Value
                        .GetOuSettingsMany(rootGuids)?
                        .SelectMany(os => os.Value)?
                        .ToList();
            var dispSettings = settings?
                    .Where(s => s.Name == OUSetting.NewDispositions)?
                    .ToList();

            var dispositions = dispSettings?
                    .SelectMany(s => JsonConvert.DeserializeObject<List<DispositionV2DataView>>(s.Value))?
                    .Select(d => new CRMDisposition { Disposition = d.Name, DisplayName = d.DisplayName, Icon = d.IconName, Color = d.Color, SBTypeId = d.SBTypeId })?
                    .ToList() ?? new List<CRMDisposition>();

            var distinctDispositions = new HashSet<CRMDisposition>(dispositions);
            distinctDispositions.Add(new CRMDisposition { Disposition = "With Proposal" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "Appointments" });

            return distinctDispositions.ToList();
        }


        public List<CRMDisposition> CRMGetAvailableDispositionsQuotas()
        {
            // Get all the OUs for the logged in user

            //var rootGuids = _ouService.Value.ListRootGuidsForPerson(Guid.Parse("b3db3e22-7aed-4daa-888c-850b8c8ee0f2"));

            //var settings = _ouSettingsService
            //            .Value
            //            .GetOuSettingsMany(rootGuids)?
            //            .SelectMany(os => os.Value)?
            //            .ToList();
            //var dispSettings = settings?
            //        .Where(s => s.Name == OUSetting.NewDispositions)?
            //        .ToList();

            //var dispositions = dispSettings?
            //        .SelectMany(s => JsonConvert.DeserializeObject<List<DispositionV2DataView>>(s.Value))?
            //        .Select(d => new CRMDisposition { Disposition = d.Name, DisplayName = d.DisplayName, Quota = "" , Commitments = "" })?
            //        .ToList() ?? new List<CRMDisposition>();
             
            var distinctDispositions = new HashSet<CRMDisposition>();
            distinctDispositions.Add(new CRMDisposition { Disposition = "Hours Knocked", DisplayName = "Hours Knocked", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "Doors Knocked", DisplayName = "Doors Knocked", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "Approach Delivered", DisplayName = "Approach Delivered", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "New Contact", DisplayName = "New Contact", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "Appointments Set", DisplayName = "Appointments Set", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "CAPP", DisplayName = "CAPP", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "SS", DisplayName = "SS", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "FD", DisplayName = "FD", Quota = "", Commitments = "" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "INS", DisplayName = "INS", Quota = "", Commitments = "" });

            return distinctDispositions.ToList();
        }


        public List<CRMLeadSource> CRMGetAvailableLeadSources(Guid ouid)
        {
            // Get all the OUs for the logged in user

            var rootGuids = _ouService.Value.ListRootGuidsForPerson(SmartPrincipal.UserId);
            // rootGuids.Add(ouid);

            var settings = _ouSettingsService
                        .Value
                        .GetOuSettingsMany(rootGuids)?
                        .SelectMany(os => os.Value)?
                        .ToList();
            var leadSettings = settings?
                    .Where(s => s.Name == OUSetting.LegionOULeadSource)?
                    .ToList();

            if (leadSettings.Count == 0 && ouid != null)
            {
                leadSettings = _ouSettingsService.Value.GetSettingsByOUID(ouid)?.Where(x => x.Name == OUSetting.LegionOULeadSource)?.ToList();
            }

            var leadsources = leadSettings?
                    .SelectMany(s => JsonConvert.DeserializeObject<List<CRMLeadSource>>(s.Value))?
                    .ToList() ?? new List<CRMLeadSource>();

            var distinctLeadsources = new HashSet<CRMLeadSource>(leadsources);

            return distinctLeadsources.ToList();
        }






        //public PaginatedResult<Property> CRMGetProperties(CRMFilterRequest request)
        //{
        //    var userId = SmartPrincipal.UserId;



        //    var ouAndRolesTree = _ouService.Value.GetOUsRoleTree(userId);
        //    var ousTree = ouAndRolesTree.GetAll();

        //    var ouIDs = ousTree.Select(ou => ou.OUID).Distinct().ToList();

        //    //var ouIDsAsOwner = ousTree
        //    //                    .Where(o => o.IsOwnerOrAdmin())
        //    //                    .Select(o => o.OUID)
        //    //                    .ToList();

        //    using (var uow = UnitOfWorkFactory())
        //    {
        //        var propertiesQuery = uow
        //                    .Get<Property>()
        //                    .Where(p => ouIDs.Contains(p.Territory.OUID));

        //        if (request.PersonID.HasValue)
        //        {
        //            var assignedTerritories = uow
        //                    .Get<Assignment>()
        //                    .Where(a => a.PersonID == request.PersonID.Value && a.IsDeleted)
        //                    .Select(a => a.TerritoryID)
        //                    .ToList();

        //            //var ouIDsAsMember = ousTree
        //            //                    .Where(o => !o.IsOwnerOrAdmin())
        //            //                    .Select(o => o.OUID)
        //            //                    .ToList();
        //            var ouIDsAsOwner = ousTree
        //                                .Where(o => o.IsOwnerOrAdmin())
        //                                .Select(o => o.OUID)
        //                                .ToList();

        //            propertiesQuery = propertiesQuery
        //                    .Where(p => ouIDsAsOwner.Contains(p.Territory.OUID) || assignedTerritories.Contains(p.TerritoryID));
        //        }

        //        if (!string.IsNullOrWhiteSpace(request.Query))
        //        {
        //            propertiesQuery = propertiesQuery
        //                                .Where(p => (p.Name != null && p.Name.Contains(request.Query)) ||
        //                                            (p.Address1 != null && p.Address1.Contains(request.Query)));
        //        }

        //        if (request.Status.HasValue)
        //        {
        //            if (request.Status.Value == InquiryStatus.SYSTEM_WithProposal)
        //            {
        //                propertiesQuery = propertiesQuery
        //                                    .Where(p => p.Proposals.Any(prop => !prop.IsDeleted));
        //            }
        //            else
        //            {
        //                propertiesQuery = propertiesQuery
        //                                    .Where(p => p.LatestStatus == request.Status.Value);
        //            }
        //        }

        //        var total = propertiesQuery.Count();

        //        AssignIncludes(request.Include, ref propertiesQuery);

        //        if (!string.IsNullOrEmpty(request.SortColumn))
        //        {
        //            var sortDirection = request.SortAscending ? "" : " descending";
        //            propertiesQuery = propertiesQuery.OrderBy($"{request.SortColumn}{sortDirection}");
        //        }
        //        else
        //        {
        //            propertiesQuery = propertiesQuery.OrderByDescending(p => p.DateCreated);
        //        }

        //        var result = propertiesQuery
        //                            .Skip(request.PageIndex * request.PageSize)
        //                            .Take(request.PageSize)
        //                            .ToList();

        //        result.ForEach(r =>
        //        {
        //            r.Summary = null;
        //        });


        //        if (request.Include?.Split(",".ToCharArray())?.Contains("territory.ou.settings", StringComparer.InvariantCultureIgnoreCase) == true)
        //        {
        //            var ous = result
        //                        .Where(p => p.Territory?.OU != null)
        //                        .Select(p => p.Territory.OU)
        //                        .Distinct()
        //                        .ToList();

        //            foreach (var ou in ous)
        //            {
        //                ou.Settings = OUSettingService.GetOuSettings(ou.Guid);
        //            }
        //        }

        //        return new PaginatedResult<Property>
        //        {
        //            Data = result,
        //            PageIndex = request.PageIndex,
        //            PageSize = request.PageSize,
        //            Total = total
        //        };
        //    }
        //}

        public string GetUserSurvey(Guid personID, Guid? propertyID = null)
        {
            using (var dc = new DataContext())
            {
                var surveyPersonSetting = dc
                    .PersonSettings
                    .FirstOrDefault(p => !p.IsDeleted && p.Name == "Ignite.Survey" && p.PersonID == personID);

                Property property = null;
                if (propertyID.HasValue)
                {
                    property = dc.Properties.FirstOrDefault(p => !p.IsDeleted && !p.IsArchive && p.Guid == propertyID.Value);

                    //check if the survey has already been signed
                    var propSurvey = dc.PropertySurveys
                        ?.Where(p => p.PropertyID == propertyID.Value && p.PersonID == personID)
                        ?.OrderByDescending(p => p.DateCreated)
                        ?.FirstOrDefault();

                    if (propSurvey != null)
                    {
                        return ReplaceTokens(propSurvey.Value, property);
                    }
                }



                if (surveyPersonSetting != null)
                {
                    return ReplaceTokens(surveyPersonSetting.Value, property);
                }

                //try to fallback to the default survey from the db
                var defaultSurveySetting = dc.OUSettings.FirstOrDefault(x => !x.IsDeleted && x.Name == "Ignite.Survey.Default");
                if (defaultSurveySetting != null)
                {
                    return ReplaceTokens(defaultSurveySetting.Value, property);
                }
            }
            return string.Empty;
        }

        public string GetSurveyUrl(Guid personID, Guid propertyID)
        {
            using (var dc = new DataContext())
            {
                var property = dc.Properties.FirstOrDefault(p => !p.IsDeleted && !p.IsArchive && p.Guid == propertyID);
                if (property == null)
                {
                    throw new Exception("Property not found");
                }

                //get the form URL
                var ouSettings = _ouSettingsService.Value.GetSettingsByPropertyID(propertyID);
                var url = ouSettings.FirstOrDefault(x => x.Name == "Ignite.Survey.BaseUrl")?.Value;

                if (string.IsNullOrEmpty(url))
                {
                    throw new Exception("Could not build the survey URL");
                }

                return $"{url}{personID}/{propertyID}";
            }
        }

        public string SaveUserSurvey(Guid personID, string survey)
        {
            using (var dc = new DataContext())
            {
                var surveyPersonSetting = dc
                    .PersonSettings
                    .FirstOrDefault(p => !p.IsDeleted && p.Name == "Ignite.Survey" && p.PersonID == personID);

                if (surveyPersonSetting != null)
                {
                    surveyPersonSetting.Value = survey;
                    dc.SaveChanges();

                    return surveyPersonSetting.Value;
                }
                else
                {
                    var newSurvey = new PersonSetting
                    {
                        PersonID = personID,
                        Name = "Ignite.Survey",
                        ValueType = SettingValueType.String,
                        Value = survey,
                        Group = PersonSettingGroupType.Membership
                    };
                    dc.PersonSettings.Add(newSurvey);

                    dc.SaveChanges();

                    return newSurvey.Value;
                }

            }
        }

        public string SavePropertySurvey(Guid personID, Guid propertyID, string survey)
        {
            using (var dc = new DataContext())
            {
                var person = dc.People.Include(x => x.PersonSettings).FirstOrDefault(x => !x.IsDeleted && x.Guid == personID);
                if (person == null)
                {
                    throw new Exception("Person not found");
                }

                var property = dc.Properties.FirstOrDefault(x => !x.IsDeleted && !x.IsArchive && x.Guid == propertyID);
                if (property == null)
                {
                    throw new Exception("Property not found");
                }

                var propertySurvey = new PropertySurvey
                {
                    PersonID = personID,
                    PropertyID = propertyID,
                    Value = survey
                };

                dc.PropertySurveys.Add(propertySurvey);
                dc.SaveChanges();

                return propertySurvey.Value;
            }
        }

        public class summarymodel
        {
            public string Label { get; set; }
            public string Data { get; set; }
        }

        public string SendEmailSummarytoCustomer(Guid ProposalID, string summary)
        {
            using (var dc = new DataContext())
            {

                var Proposalsdata = dc.ProposalData.FirstOrDefault(x => x.Guid == ProposalID);
                if (Proposalsdata == null)
                {
                    throw new Exception("Proposal not found");
                }


                var Proposal = dc.Proposal.FirstOrDefault(x => x.Guid == ProposalID || x.Guid == Proposalsdata.ProposalID);
                if (Proposal == null)
                {
                    throw new Exception("Proposal not found");
                }

                var property = dc.Properties.FirstOrDefault(x => !x.IsDeleted && !x.IsArchive && x.Guid == Proposal.PropertyID);
                if (property == null)
                {
                    throw new Exception("Property not found");
                }

                var ProposalSummaryData = JsonConvert.DeserializeObject<List<summarymodel>>(summary);

                string mailbody = "<p>The following Data for the proposal : " + Proposal.Name + "</p>";

                foreach (var item in ProposalSummaryData)
                {
                    mailbody = mailbody + "<p><b> " + item.Label + ": </b> " + item.Data + "</p>";
                }

                var email = property.GetMainEmailAddress();

                Mail.Library.SendEmail("hevin.android@gmail.com", string.Empty, $"Proposal Summary Data", mailbody + email, true);

                if (email != null)
                {
                    Task.Factory.StartNew(() =>
                    {
                        Mail.Library.SendEmail(email, string.Empty, $"Proposal Summary Data", mailbody, true);
                    });
                }
                return summary;
            }
        }


        public PaginatedResult<Property> CRMGetProperties(CRMFilterRequest request)
        {
            var rootGuids = _ouService.Value.ListRootGuidsForPerson(SmartPrincipal.UserId);

            var ouIds = new List<Guid>();

            foreach (var guid in rootGuids)
            {
                ouIds.AddRange(_ouService.Value.GetOUAndChildrenGuids(guid).ToList());
            }

            //ou filter
            if (request.OUIds?.Any() == true)
            {
                if (ouIds.Intersect(request.OUIds).Count() == request.OUIds.Count())
                {
                    ouIds = request.OUIds.ToList();
                }
                else
                {
                    return PaginatedResult<Property>.GetDefault(request.PageIndex, request.PageSize);
                }
            }
            else
            {
                ouIds = ouIds.Distinct().ToList();
            }

            using (var uow = UnitOfWorkFactory())
            {
                var propertiesQuery = uow.Get<Property>().Where(p => ouIds.Contains(p.Territory.OUID));

                if (request.TerritoryIds?.Any() == true)
                {
                    propertiesQuery = propertiesQuery.Where(p => request.TerritoryIds.Contains(p.TerritoryID));
                }

                if (!string.IsNullOrWhiteSpace(request.Query))
                {
                    var paramValue = new List<SqlParameter> { new SqlParameter("@query", $"{request.Query}%") };

                    var words = request
                                .Query
                                .Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                                .Select(w => $"{w}*")
                                .ToList();


                    var parameters = string.Join(" AND ", words.Select(w => $"\"@{w}\""));

                    using (var dataContext = new DataContext())
                    {
                        var propIds = dataContext.Properties.SqlQuery($"SELECT * From Properties WHERE CONTAINS(Name, '{ parameters}') OR Address1 like @query OR Name like @query", paramValue.ToArray())
                                .Select(p => p.Guid)
                                .ToList();

                        propertiesQuery = propertiesQuery.Where(p => propIds.Contains(p.Guid));
                    }
                }

                if (!string.IsNullOrEmpty(request.Disposition))
                {
                    switch (request.Disposition)
                    {
                        case "With Proposal":
                            propertiesQuery = propertiesQuery
                                            .Where(p => p.Proposals.Any(prop => !prop.IsDeleted));
                            break;
                        case "Appointments":
                            if (request.HasInclude(nameof(Property.Appointments)))
                            {
                                propertiesQuery = propertiesQuery.Where(p => p.Appointments.Any(app => app.CreatedByID == SmartPrincipal.UserId && !app.IsDeleted));
                            }
                            break;
                        default:
                            propertiesQuery = propertiesQuery.Where(p => p.LatestDisposition == request.Disposition);
                            break;
                    }
                }

                //multiple dispositions queries
                if (request.DispositionsQuery?.Any() == true)
                {
                    if (request.DispositionsQuery?.Any(x => x == "Appointments") == true)
                    {
                        propertiesQuery = propertiesQuery
                                            .Where(p => p.Appointments.Any(app => app.CreatedByID == SmartPrincipal.UserId && !app.IsDeleted));
                    }

                    if (request.DispositionsQuery?.Any(x => x == "With Proposal") == true)
                    {
                        propertiesQuery = propertiesQuery
                                            .Where(p => p.Proposals.Any(prop => !prop.IsDeleted));
                    }

                    var customDispositionsList = new List<string> { "Appointments", "With Proposal" };

                    if (request.DispositionsQuery?.Any(x => !customDispositionsList.Contains(x)) == true)
                    {
                        //propertiesQuery = propertiesQuery.Where(p => p.Inquiries.Any(i => request.DispositionsQuery.Contains(i.Disposition)));
                        propertiesQuery = propertiesQuery.Where(p => request.DispositionsQuery.Contains(p.LatestDisposition));
                    }
                }

                //appointments created
                if (request.AppointmentCreatedIds?.Any() == true)
                {
                    propertiesQuery = propertiesQuery.Where(p => p.Appointments.Any(app => app.CreatedByID != null && request.AppointmentCreatedIds.Contains(app.CreatedByID.Value)));
                }

                //appointments assigned
                if (request.AppointmentAssignedIds?.Any() == true)
                {
                    propertiesQuery = propertiesQuery.Where(p => p.Appointments.Any(app => app.AssigneeID != null && request.AppointmentAssignedIds.Contains(app.AssigneeID.Value)));
                }

                //appointment Date
                if (request.AppointmentQuery != null)
                {
                    if (request.AppointmentQuery.Date.HasValue)
                    {
                        propertiesQuery = propertiesQuery.Where(p => p.Appointments.Any(app => app.StartDate.Date == request.AppointmentQuery.Date.Value));
                    }
                    if (request.AppointmentQuery.DateFrom.HasValue)
                    {
                        propertiesQuery = propertiesQuery.Where(p => p.Appointments.Any(app => app.StartDate > request.AppointmentQuery.DateFrom.Value));
                    }
                    if (request.AppointmentQuery.DateTo.HasValue)
                    {
                        propertiesQuery = propertiesQuery.Where(p => p.Appointments.Any(app => app.StartDate < request.AppointmentQuery.DateTo.Value));
                    }
                }

                var total = propertiesQuery.Count();

                //client needs the notes count
                var includeString = string.Empty;
                if (string.IsNullOrEmpty(request.Include))
                {
                    includeString = "PropertyNotes";
                }
                else
                {
                    if (!request.Include.Contains("PropertyNotes"))
                    {
                        includeString = $"{request.Include}&PropertyNotes";
                    }
                    else
                    {
                        includeString = request.Include;
                    }

                }

                AssignIncludes(includeString, ref propertiesQuery);

                if (!string.IsNullOrEmpty(request.SortColumn))
                {
                    var sortDirection = request.SortAscending ? "" : " descending";
                    propertiesQuery = propertiesQuery.OrderBy($"{request.SortColumn}{sortDirection}");
                }
                else
                {
                    propertiesQuery = propertiesQuery.OrderByDescending(p => p.DateCreated);
                }

                var result = propertiesQuery.Skip(request.PageIndex * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToList();

                //if (result?.Any() == true)
                //{
                //    foreach (var prop in result)
                //    {
                //        prop.PropertyNotesCount = prop.PropertyNotes?.Where(x => !x.IsDeleted)?.Count();

                //        if (!request.Include.Contains("PropertyNotes"))
                //        {
                //            prop.PropertyNotes = new List<PropertyNote>();
                //        }
                //    }
                //}

                //if (request.Include?.Split(",".ToCharArray())?.Contains("territory.ou.settings", StringComparer.InvariantCultureIgnoreCase) == true)
                //{
                //    var ous = result
                //                .Where(p => p.Territory?.OU != null)
                //                .Select(p => p.Territory.OU)
                //                .Distinct()
                //                .ToList();

                //    foreach (var ou in ous)
                //    {
                //        ou.Settings = OUSettingService.GetOuSettings(ou.Guid);
                //    }
                //}

                return new PaginatedResult<Property>
                {
                    Data = result,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Total = total
                };
            }
        }

        private void PopulateSummary(Person p)
        {
            using (DataContext dc = new DataContext())
            {
                PersonSummary summary = dc.Database.SqlQuery<PersonSummary>("exec proc_PersonAnalytics {0}", p.Guid).FirstOrDefault();
                p.Summary = summary;
            }
        }

        public PersonDTO GetPersonDTO(Guid personID, string include = "")
        {
            var person = Get(personID, include: include);
            var roleType = OURoleType.None;
            var permissionType = PermissionType.None;
            var currentUsersAssociations = _ouAssociationService.SmartList(filter: $"Personid={personID}");
            currentUsersAssociations
                    .ToList()
                    .ForEach(ouAssociation =>
                    {
                        roleType = roleType | ouAssociation.RoleType;
                        if (ouAssociation?.OURole?.Permissions != null)
                        {
                            permissionType = permissionType | ouAssociation.OURole.Permissions;
                        }
                    });
            var personJSON = JsonConvert.SerializeObject(person, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            var result = JsonConvert.DeserializeObject<PersonDTO>(personJSON);
            result.RoleType = roleType;
            result.PermissionType = permissionType;
            return result;
        }

        public PersonClockTime GetPersonClock(Guid personID, long min)
        {
            PersonClockTime person = new PersonClockTime();
            using (DataContext dc = new DataContext())
            {
                person = dc.PersonClockTime.Where(p => p.PersonID == personID).ToList().Where(p => p.DateCreated.Date == DateTime.UtcNow.Date).FirstOrDefault();

                if (person != null)
                {
                    if (person.EndDate.Value <= DateTime.UtcNow && person.ClockType == "ClockIn")
                    {
                        TimeSpan timespan = person.EndDate.Value - person.StartDate.Value;
                        long difMin = (long)Math.Floor(timespan.TotalMinutes);

                        person.ClockMin = person.ClockMin + difMin;
                        TimeSpan spWorkMin = TimeSpan.FromMinutes(person.ClockMin);
                        person.TagString = string.Format("{0:00}:{1:00}", (int)spWorkMin.TotalHours, spWorkMin.Minutes);
                        person.ClockHours = Convert.ToInt64(Math.Round(person.ClockMin / (double)60));
                        person.ClockDiff = 0;
                        person.ClockType = "ClockOut";
                        person.TenantID = 0;
                        person.Version += 1;
                        person.IsRemainFiveMin = false;
                        person.DateLastModified = DateTime.UtcNow;
                        dc.SaveChanges();

                    }
                    else if (person.ClockType == "ClockIn")
                    {
                        TimeSpan timespan = DateTime.UtcNow - person.StartDate.Value;
                        long diffMin = (long)Math.Floor(timespan.TotalMinutes);

                        TimeSpan FiveMin = person.EndDate.Value - DateTime.UtcNow;
                        long ReFiveMin = (long)Math.Floor(FiveMin.TotalMinutes);
                        person.IsRemainFiveMin = false;
                        if (ReFiveMin == 5)
                        {
                            person.IsRemainFiveMin = true;
                        }
                        person.ClockDiff = diffMin;
                        person.DateLastModified = DateTime.UtcNow;
                        dc.SaveChanges();
                    }
                    person = dc.PersonClockTime.Where(p => p.PersonID == personID).ToList().Where(p => p.DateCreated.Date == DateTime.UtcNow.Date).FirstOrDefault();
                }
            }
            if (person == null) { person = new PersonClockTime(); };
            return person;
        }

        public IEnumerable<Person> personDetails(Guid ouid, DateTime date)
        {
            using (DataContext dc = new DataContext())
            {
                var peopleIds = dc.OUAssociations.Where(x => x.IsDeleted == false && x.OUID == ouid).Select(y => y.PersonID);
                var peoples = dc.People?.Include(p => p.AssignedAppointments).Where(x => peopleIds.Contains(x.Guid) && x.IsDeleted == false).ToList();

                var result = new List<Person>();
                foreach (var person in peoples)
                {
                    if (person.AssignedAppointments != null)
                    {
                        person.AssignedAppointments = person.AssignedAppointments.Where(x => x.StartDate.Date == date.Date).ToList();
                    }

                    if (result.Any(r => r.Guid == person.Guid))
                    {
                        continue;
                    }
                    result.Add(person);
                }
                return result;

            }
        }

        private string ReplaceTokens(string source, Property property)
        {
            if (property == null)
            {
                return source;
            }
            var dest = source;
            var mainOccupant = property?.GetMainOccupant();
            dest = dest.Replace("$customerName$", mainOccupant != null ? $"{mainOccupant?.FirstName} {mainOccupant?.LastName}" : string.Empty);
            dest = dest.Replace("$propertyAddress$", $"{property?.Address1 ?? string.Empty}, {property?.Address2 ?? string.Empty}");

            return dest;
        }

        public IEnumerable<PersonOffboard> OuassociationRoleName(Guid personid)
        {
            using (DataContext dc = new DataContext())
            {
                var OUAssociation = dc.OUAssociations?.Include(p => p.OU).Include(p => p.OURole).Where(x => x.PersonID == personid && x.IsDeleted == false).ToList();
                var OUAss = OUAssociation?.Select(x => new PersonOffboard
                {
                    RoleID = x.OURoleID,
                    OUID = x.OUID,
                    AssociateOuName = x.OU?.Name,
                    RoleName = x.OURole?.Name
                });

                return OUAss;
            }
        }

        public async Task<IEnumerable<Person>> CalendarPageAppointMentsByOuid(Guid ouid, string CurrentDate, string type)
        {
            try
            {
                List<Person> ret = new List<Person>();
                using (DataContext dc = new DataContext())
                {
                    DateTime dt = Convert.ToDateTime(CurrentDate);
                    DateTime dtt = dt.AddDays(1);
                    var OUAssociationIds = (from oua in dc.OUAssociations
                                            where oua.OUID == ouid && ((oua.RoleType == OURoleType.Member || oua.RoleType == OURoleType.Manager || oua.RoleType == OURoleType.SuperAdmin)) && !oua.IsDeleted && !oua.Person.IsDeleted
                                            select oua.PersonID).Distinct().ToList();

                    var peoples = dc.People.Where(peo => OUAssociationIds.Contains(peo.Guid) && !peo.IsDeleted)
                    .IncludeOptimized(yt => yt.PersonSettings.Where(y => !y.IsDeleted))
                    .IncludeOptimized(ut => ut.PhoneNumbers.Where(u => !u.IsDeleted))
                    .IncludeOptimized(pt => pt.AssignedAppointments.Where(i => ((i.DateCreated >= dt && i.DateCreated < dtt) || (i.StartDate >= dt && i.StartDate < dtt)) && !i.IsDeleted))
                    .ToList();

                    var favouritePeopleIds = (from oua in dc.AppointmentFavouritePersons
                                           where oua.PersonID == SmartPrincipal.UserId
                                           select oua.FavouritePersonID).Distinct().ToList();
                    foreach (var item in peoples)
                    {
                        item.IsFavourite = favouritePeopleIds.Contains(item.Guid);
                    }
                    if (type == "Favourite")
                    {
                        peoples = peoples.Where(a => a.IsFavourite == true).ToList();
                    }

                    return peoples;
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// This method insert Person as a Favorite 
        /// </summary>
        public AppointmentFavouritePerson InsertFavoritePerson(Guid FavouritePersonID)
        {
            using (var dc = new DataContext())
            {
                var FavouritePerson = dc.AppointmentFavouritePersons.FirstOrDefault(x => x.PersonID == SmartPrincipal.UserId && x.FavouritePersonID == FavouritePersonID);

                if (FavouritePerson != null)
                    throw new ApplicationException("Already Favourited");

                var person = new AppointmentFavouritePerson
                {
                    FavouritePersonID = FavouritePersonID,
                    PersonID = SmartPrincipal.UserId,
                    CreatedByID = SmartPrincipal.UserId
                };

                dc.AppointmentFavouritePersons.Add(person);
                dc.SaveChanges();

                return person;
            }
        }


        /// <summary>
        /// This method remove Person as a Favorite 
        /// </summary>
        public void RemoveFavoritePerson(Guid FavouritePersonID)
        {
            using (var dc = new DataContext())
            {
                var FavouritePerson = dc.AppointmentFavouritePersons.FirstOrDefault(x => x.PersonID == SmartPrincipal.UserId && x.FavouritePersonID == FavouritePersonID);

                if (FavouritePerson == null)
                    throw new ApplicationException("Territory not found");

                dc.AppointmentFavouritePersons.Remove(FavouritePerson);
                dc.SaveChanges();
            }
        }
    }
}
