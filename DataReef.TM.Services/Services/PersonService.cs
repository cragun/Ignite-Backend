﻿using DataReef.Core.Classes;
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

        public PersonService(ILogger logger,
            IOUAssociationService ouAssociationService,
            Lazy<IMailChimpAdapter> mailChimpAdapter,
            Lazy<IOUService> ouService,
            Lazy<IOUSettingService> ouSettingsService,
            Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _ouAssociationService = ouAssociationService;
            _mailChimpAdapter = mailChimpAdapter;
            _ouService = ouService;
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
            var ret = base.Update(entity);

            //foreach (OUAssociation oua in entity.OUAssociations)
            //{
            //    base.ProcessApiWebHooks(entity, Models.Enums.EventDomain.User, Models.Enums.EventAction.Changed, oua.OUID);
            //}

            return ret;

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
        /// <param name="environment"></param>
        public void Reactivate(Guid personId)
        {
            using (DataContext dc = new DataContext())
            {
                var person = dc
                                .People
                                .SingleOrDefault(p => p.Guid == personId
                                                  && p.IsDeleted == true);
                if (person == null)
                {
                    throw new ArgumentException("Couldn't find the person among the deleted ones!");
                }

                person.IsDeleted = false;

                var user = dc
                            .Users
                            .SingleOrDefault(p => p.Guid == personId);
                if (user != null && user.IsDeleted)
                {
                    user.IsDeleted = false;
                }

                var credentials = dc
                                    .Credentials
                                    .Where(c => c.UserID == personId)
                                    .ToList();
                if (credentials != null && credentials.Any())
                {
                    credentials.ForEach(c => c.IsDeleted = false);
                }

                dc.SaveChanges();

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
                    .Select(d => new CRMDisposition { Disposition = d.Name, DisplayName = d.DisplayName, Icon = d.IconName, Color = d.Color})?
                    .ToList() ?? new List<CRMDisposition>();

            var distinctDispositions = new HashSet<CRMDisposition>(dispositions);
            distinctDispositions.Add(new CRMDisposition { Disposition = "With Proposal" });
            distinctDispositions.Add(new CRMDisposition { Disposition = "Appointments" });

            return distinctDispositions.ToList();
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
            using(var dc = new DataContext())
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
                if(defaultSurveySetting != null)
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
                if(property == null)
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
            using(var dc = new DataContext())
            {
                var person = dc.People.Include(x => x.PersonSettings).FirstOrDefault(x => !x.IsDeleted && x.Guid == personID);
                if(person == null)
                {
                    throw new Exception("Person not found");
                }

                var property = dc.Properties.FirstOrDefault(x => !x.IsDeleted && !x.IsArchive && x.Guid == propertyID);
                if(property == null)
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

        public PaginatedResult<Property> CRMGetProperties(CRMFilterRequest request)
        {
            var rootGuids = _ouService
                                .Value
                                .ListRootGuidsForPerson(SmartPrincipal.UserId);

            var ouIds = new List<Guid>();

            foreach (var guid in rootGuids)
            {
                ouIds.AddRange(_ouService
                                .Value
                                .GetOUAndChildrenGuids(guid)
                                .ToList());
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
                ouIds = ouIds
                        .Distinct()
                        .ToList();
            }



            using (var uow = UnitOfWorkFactory())
            {
                var propertiesQuery = uow
                            .Get<Property>()
                            .Where(p => ouIds.Contains(p.Territory.OUID));

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
                        var propIds = dataContext
                                .Properties
                                .SqlQuery($"SELECT * From Properties WHERE CONTAINS(Name, '{ parameters}') OR Address1 like @query", paramValue.ToArray())
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
                                propertiesQuery = propertiesQuery
                                            .Where(p => p.Appointments.Any(app => app.CreatedByID == SmartPrincipal.UserId && !app.IsDeleted));
                            }
                            break;
                        default:
                            propertiesQuery = propertiesQuery
                                            .Where(p => p.LatestDisposition == request.Disposition);
                            break;
                    }
                }

                //multiple dispositions query
                if (request.DispositionsQuery?.Any() == true)
                {
                    if(request.DispositionsQuery?.Any(x => x == "Appointments") == true)
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
                        propertiesQuery = propertiesQuery.Where(p => p.Inquiries.Any(i => request.DispositionsQuery.Contains(i.Disposition)));
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

                var result = propertiesQuery
                                    .Skip(request.PageIndex * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToList();

                if(result?.Any() == true)
                {
                    foreach(var prop in result)
                    {
                        prop.PropertyNotesCount = prop.PropertyNotes?.Where(x => !x.IsDeleted)?.Count();

                        if (!request.Include.Contains("PropertyNotes"))
                        {
                            prop.PropertyNotes = new List<PropertyNote>();
                        }
                    }
                }

                if (request.Include?.Split(",".ToCharArray())?.Contains("territory.ou.settings", StringComparer.InvariantCultureIgnoreCase) == true)
                {
                    var ous = result
                                .Where(p => p.Territory?.OU != null)
                                .Select(p => p.Territory.OU)
                                .Distinct()
                                .ToList();

                    foreach (var ou in ous)
                    {
                        ou.Settings = OUSettingService.GetOuSettings(ou.Guid);
                    }
                }

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

        private string ReplaceTokens(string source, Property property)
        {
            if(property == null)
            {
                return source;
            }
            var dest = source;
            var mainOccupant = property?.GetMainOccupant();
            dest = dest.Replace("$customerName$", mainOccupant != null ? $"{mainOccupant?.FirstName} {mainOccupant?.LastName}" : string.Empty);
            dest = dest.Replace("$propertyAddress$", $"{property?.Address1 ?? string.Empty}, {property?.Address2 ?? string.Empty}");

            return dest;
        }
    }
}