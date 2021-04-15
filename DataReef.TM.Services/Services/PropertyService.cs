using DataReef.Core.Classes;
using DataReef.Core.Enums;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.Integrations;
using DataReef.Integrations.Common.Geo;
using DataReef.TM.Contracts.Services;
using DataReef.TM.Contracts.Services.FinanceAdapters;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DataViews.Settings;
using DataReef.TM.Models.DTOs;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.Properties;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.SmartBoard;
using DataReef.TM.Models.DTOs.Solar.Finance;
using DataReef.TM.Models.Enums;
using DataReef.TM.Models.Geo;
using DataReef.TM.Models.PubSubMessaging;
using DataReef.TM.Models.Solar;
using DataReef.TM.Services.Extensions;
using DataReef.TM.Services.InternalServices.Geo;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using EntityFramework.Extensions;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Property = DataReef.TM.Models.Property;
using PropertyAttribute = DataReef.TM.Models.PropertyAttribute;


namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class PropertyService : DataService<Property>, IPropertyService
    {
        private readonly IGeoProvider _geoProvider;
        private readonly Func<IGeographyBridge> _geographyBridgeFactory;
        private readonly Lazy<IDeviceService> _deviceService;
        private readonly Lazy<ISolarSalesTrackerAdapter> _sbAdapter;
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IOUSettingService> _ouSettingService;
        private readonly Lazy<ITerritoryService> _territoryService;
        private readonly Lazy<IAppointmentService> _appointmentService;
        private readonly Lazy<IInquiryService> _inquiryService;
        private readonly Lazy<ISunlightAdapter> _sunlightAdapter;
        private readonly Lazy<ISmsService> _smsService;
        private readonly IPersonService _peopleService;

        private static string BaseURL = "http://www.esiids.com/cgi-bin/esiids_xml.cgi?";

        private RestClient _client;
        private RestClient Client
        {
            get
            {

                if (_client == null)
                {
                    _client = new RestClient(BaseURL);
                }
                return _client;
            }
        }
        public PropertyService(ILogger logger,
            IGeoProvider geoProvider,
            Func<IGeographyBridge> geographyBridgeFactory,
            Func<IUnitOfWork> unitOfWorkFactory,
            Lazy<IDeviceService> deviceService,
            Lazy<ISolarSalesTrackerAdapter> sbAdapter,
            Lazy<ISunlightAdapter> sunlightAdapter,
            Lazy<IOUService> ouService,
            Lazy<IOUSettingService> ouSettingService,
            Lazy<ITerritoryService> territoryService,
            Lazy<IAppointmentService> appointmentService,
            Lazy<IInquiryService> inquiryService,
            Lazy<ISmsService> smsService,
            IPersonService peopleService)
            : base(logger, unitOfWorkFactory)
        {
            _geoProvider = geoProvider;
            _geographyBridgeFactory = geographyBridgeFactory;
            _deviceService = deviceService;
            _sbAdapter = sbAdapter;
            _sunlightAdapter = sunlightAdapter;
            _ouService = ouService;
            _ouSettingService = ouSettingService;
            _territoryService = territoryService;
            _appointmentService = appointmentService;
            _inquiryService = inquiryService;
            _smsService = smsService;
            _peopleService = peopleService;
        }

        public override ICollection<Property> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            var includeString = string.Empty;
            if (string.IsNullOrEmpty(include))
            {
                includeString = "PropertyNotes";
            }
            else
            {
                if (!include.Contains("PropertyNotes"))
                {
                    includeString = $"{include}&PropertyNotes";
                }
                else
                {
                    includeString = include;
                }

            }

            var ret = base.List(deletedItems, pageNumber, itemsPerPage, filter, includeString, exclude, fields);
            if (ret?.Any() == true)
            {
                foreach (var prop in ret)
                {
                    prop.PropertyNotesCount = prop.PropertyNotes?.Where(x => !x.IsDeleted)?.Count();
                    prop.PropertyNotes = new List<PropertyNote>();
                }
            }

            if (include.Contains("PropertyNotes"))
            {
                return ret;
            }




            return ret;
        }

        public override Property Get(Guid uniqueId, string include = "", string exclude = "", string fields = "", bool deletedItems = false)
        {
            var includeString = string.Empty;
            if (string.IsNullOrEmpty(include))
            {
                includeString = "PropertyNotes";
            }
            else
            {
                if (!include.Contains("PropertyNotes"))
                {
                    includeString = $"{include}&PropertyNotes";
                }
                else
                {
                    includeString = include;
                }

            }
            var ret = base.Get(uniqueId, includeString, exclude, fields, deletedItems);
            ret.PropertyNotesCount = ret.PropertyNotes?.Where(x => !x.IsDeleted)?.Count();

            if (include.Contains("PropertyNotes"))
            {
                return ret;
            }

            ret.PropertyNotes = new List<PropertyNote>();

            return ret;
        }


        public override Property Insert(Property entity)
        {

            Property ret = null;

            entity.PrepareNavigationProperties(SmartPrincipal.UserId);

            // IX_UniqueProperty requires unique TerritoryId & External Id pair.
            // There were cases when the Property that had this pair was soft deleted. The code below will check if this property exists, and hard deletes it.
            if (!string.IsNullOrWhiteSpace(entity.ExternalID))
            {
                using (var dataContext = new DataContext())
                {

                    ret = base.Update(entity, dataContext);
                    if (!ret.SaveResult.Success) throw new Exception(ret.SaveResult.Exception + " " + ret.SaveResult.ExceptionMessage);


                    var existingProp = dataContext
                                        .Properties
                                        .FirstOrDefault(p => p.TerritoryID == entity.TerritoryID
                                                     && p.ExternalID == entity.ExternalID);
                    if (existingProp != null && existingProp.IsDeleted)
                    {
                        var occupantIds = dataContext.Occupants.Where(o => o.PropertyID == existingProp.Guid).Select(o => o.Guid).ToList();

                        dataContext.Inquiries.RemoveRange(dataContext.Inquiries.Where(i => i.PropertyID == existingProp.Guid));
                        dataContext.Occupants.RemoveRange(dataContext.Occupants.Where(i => i.PropertyID == existingProp.Guid));
                        dataContext.PrescreenInstants.RemoveRange(dataContext.PrescreenInstants.Where(i => i.PropertyID == existingProp.Guid));
                        dataContext.PrescreenDetails.RemoveRange(dataContext.PrescreenDetails.Where(i => i.PropertyID == existingProp.Guid));
                        dataContext.Fields.RemoveRange(dataContext.Fields.Where(i => i.PropertyId == existingProp.Guid || (i.OccupantId.HasValue && occupantIds.Contains(i.OccupantId.Value))));
                        dataContext.PropertyAttributes.RemoveRange(dataContext.PropertyAttributes.Where(i => i.PropertyID == existingProp.Guid));
                        dataContext.Reminders.RemoveRange(dataContext.Reminders.Where(i => i.PropertyID == existingProp.Guid));

                        dataContext.Properties.Remove(existingProp);
                        dataContext.SaveChanges();
                    }
                    else
                    {
                        // if property with the same territoryID and externalID exists and was not deleted, patch it instead
                        if (existingProp != null)
                        {
                            entity.Guid = existingProp.Guid;
                            _inquiryService.Value.UpdatePersonClockTime(entity.Guid);
                            return Update(entity);
                        }
                    }
                }
            }

            var prop = base.InsertMany(new List<Property>(1) { entity }).FirstOrDefault();
            _inquiryService.Value.UpdatePersonClockTime(prop.Guid);
            prop.SBLeadError = "";

            //send new lead to SMARTBOARD
            //if (entity.LatestDisposition.Equals("DoorKnocked") || entity.GetMainPhoneNumber() != null || entity.GetMainEmailAddress() != null)
            //if (entity.LatestDisposition.Equals("DoorKnocked"))
            if (entity.GetMainPhoneNumber() != null || entity.GetMainEmailAddress() != null || entity.Name != null)
            {
                try
                {
                    var response = _sbAdapter.Value.SubmitLead(entity.Guid);

                    if (response != null && response.Message.Type.Equals("error"))
                    {
                        prop.SBLeadError = response.Message.Text + ". This lead will not be saved in SMARTBoard until it's added.";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Error submitting SB lead!", ex);
                }

            }

            //send new lead to SMARTBOARD



            //update territory date modified because a new property was added
            using (var dataContext = new DataContext())
            {
                var territory = dataContext.Territories.FirstOrDefault(t => t.Guid == prop.TerritoryID);
                if (territory != null)
                {
                    territory.Updated(SmartPrincipal.UserId);
                    dataContext.SaveChanges();
                }
            }


            if (entity.SaveResult != null && !String.IsNullOrWhiteSpace(entity.SaveResult.ExceptionMessage))
            {
                var msg = entity.SaveResult.ExceptionMessage;
                if (msg.IndexOf("IX_UniqueProperty") != -1 || msg.IndexOf("PK_dbo.Properties") != -1)
                {
                    throw new ApplicationException("Another user updated the information for this property. Please go to territory screen and perform a pull to refresh action on the properties list to get the latest available information.");
                }
            }

            if (entity.Survey != null)
            {
                try
                {
                    UpdateNavigationProperties(entity.Survey);
                }
                catch (Exception)
                { }
            }

            if (entity.Inquiries?.Any() == true)
            {
                //person clocktime 
                _inquiryService.Value.UpdatePersonClockTime(prop.Guid);

                using (var uow = UnitOfWorkFactory())
                {
                    var property = uow
                            .Get<Property>()
                            .Include(p => p.Territory)
                            .FirstOrDefault(p => p.Guid == entity.Guid);

                    // try to notify SB that a new inquiry has been added to a new property
                    _ouService.Value.ProcessEvent(new EventMessage
                    {
                        EventSource = "Inquiry",
                        EventAction = EventActionType.Insert,
                        EventEntity = entity.Inquiries.FirstOrDefault(),
                        OUID = property.Territory.OUID,
                        EventEntityGuid = entity.Guid
                    });
                }
            }

            Task.Factory.StartNew(() =>
            {
                _deviceService.Value.PushToSubscribers<Territory, Property>(prop.TerritoryID.ToString(), prop.Guid.ToString(), DataAction.Insert, alert: $"Property {prop.Name} has been created!");
            });

            _inquiryService.Value.UpdatePersonClockTime(prop.Guid);
            return prop;
        }

        public override Property Update(Property entity)
        {
            try
            {
                Property ret = null;

                Appointment appointmentBefore = null;
                using (var dc = new DataContext())
                {
                    appointmentBefore = dc.Appointments.Where(p => p.PropertyID == entity.Guid).OrderByDescending(a => a.DateCreated).FirstOrDefault();
                }
                using (var dataContext = new DataContext())
                {
                    using (var transaction = dataContext.Database.BeginTransaction())
                    {
                        try
                        {
                            var needToUpdateSB = false;
                            Property oldProp = null;
                            using (var dc = new DataContext())
                            {
                                oldProp = dc
                                            .Properties
                                            .Include(p => p.Occupants)
                                            .Include(p => p.PropertyBag)
                                            .Include(p => p.Inquiries)
                                            .Include(p => p.Territory)
                                            .Include(p => p.Appointments)
                                            .FirstOrDefault(p => p.Guid == entity.Guid);
                            }

                            needToUpdateSB = (oldProp.Name != entity.Name || oldProp.GetMainEmailAddress() != entity.GetMainEmailAddress()
                                                || oldProp.GetMainPhoneNumber() != entity.GetMainPhoneNumber() || oldProp.UtilityProviderID != entity.UtilityProviderID || oldProp.LatestDisposition != entity.LatestDisposition);

                            entity.PrepareNavigationProperties(SmartPrincipal.UserId);

                            // remove property bags
                            dataContext
                                    .Fields
                                    .Where(f => f.PropertyId == entity.Guid)
                                    .Delete();

                            // remove occupants property bags
                            var occupantIds = dataContext
                                                .Occupants
                                                .Where(o => o.PropertyID == entity.Guid)
                                                .Select(o => o.Guid)
                                                .ToList();
                            if (occupantIds.Count > 0)
                            {
                                dataContext
                                        .Fields
                                        .Where(f => f.OccupantId.HasValue && occupantIds.Contains(f.OccupantId.Value))
                                        .Delete();
                                // remove occupants
                                dataContext
                                        .Occupants
                                        .Where(o => occupantIds.Contains(o.Guid))
                                        .Delete();
                            }
                            // remove property attributes
                            dataContext
                                    .PropertyAttributes
                                    .Where(pa => pa.PropertyID == entity.Guid)
                                    .Delete();

                            dataContext.SaveChanges();



                            ret = base.Update(entity, dataContext);

                            if (oldProp.LatestDisposition != entity.LatestDisposition)
                            {
                                //Update StartDate and Sb User StartDate
                                _peopleService.UpdateStartDate();
                                //person clocktime 
                                _inquiryService.Value.UpdatePersonClockTime(ret.Guid);
                            }
                            if (!ret.SaveResult.Success) throw new Exception($"{ret.SaveResult.Exception} {ret.SaveResult.ExceptionMessage}");
                            ret.SBLeadError = "";
                            UpdateNavigationProperties(entity, dataContext: dataContext);

                            //update territory date modified because a new property was added
                            var territory = dataContext.Territories.FirstOrDefault(t => t.Guid == entity.TerritoryID);
                            if (territory != null)
                            {
                                territory.Updated(SmartPrincipal.UserId);
                                dataContext.SaveChanges();
                            }


                            transaction.Commit();

                            // handle new appointments
                            var newAppointments = entity
                                                    .Appointments?
                                                    .Where(ap => ap.IsNew == true)?
                                                    .ToList();

                            var creator = dataContext.People.FirstOrDefault(x => x.Guid == SmartPrincipal.UserId);

                            if (newAppointments != null && newAppointments.Count > 0)
                            {
                                var fstAppoint = newAppointments.FirstOrDefault(); 

                                if (fstAppoint != null)
                                {
                                    if (Convert.ToBoolean(fstAppoint.SendSmsToCust))
                                    {
                                        DateTime stDate = TimeZoneInfo.ConvertTime(fstAppoint.StartDate, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

                                        _smsService.Value.SendSms($"You have a solar appointment with {creator?.Name} on  {stDate.Date.ToShortDateString()} at {stDate.ToShortTimeString()} , https://calendar.google.com/calendar/u/0/r/{ stDate.Year}/{ stDate.Month}/{ stDate.Day}", entity.GetMainPhoneNumber());
                                    }

                                    if (Convert.ToBoolean(fstAppoint.SendSmsToEC))
                                    {
                                        DateTime stDate = TimeZoneInfo.ConvertTime(fstAppoint.StartDate, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));

                                        _smsService.Value.SendSms($"You have a solar appointment with {entity.Name} on  {stDate.Date.ToShortDateString()} at {stDate.ToShortTimeString()} , https://calendar.google.com/calendar/u/0/r/{ stDate.Year}/{ stDate.Month}/{ stDate.Day}", creator?.PhoneNumbers.FirstOrDefault()?.Number);
                                    }
                                }
                                _appointmentService.Value.VerifyUserAssignmentAndInvite(newAppointments);
                            }

                            // handle new inquiries
                            var newInquiries = entity
                                                    .Inquiries?
                                                    .Where(inq => inq.IsNew == true)?
                                                    .ToList();


                            var pushedToSB = false;
                            if (newInquiries != null && newInquiries.Count > 0)
                            {
                                var ouid = oldProp.Territory?.OUID;
                                newInquiries.ForEach(inquiry =>
                                {
                                    pushedToSB = pushedToSB || _ouService.Value.ProcessEvent(new EventMessage
                                    {
                                        EventSource = "Inquiry",
                                        EventAction = EventActionType.Insert,
                                        EventEntity = inquiry,
                                        OUID = ouid,
                                        EventEntityGuid = inquiry.Guid
                                    });
                                });

                            }

                            Task.Factory.StartNew(() =>
                            {
                                _deviceService.Value.PushToSubscribers<Territory, Property>(ret.TerritoryID.ToString(), ret.Guid.ToString(), DataAction.Update, alert: $"Property {ret.Name} has been updated!");
                            });

                            //if (needToUpdateSB && !pushedToSB)
                            if (needToUpdateSB)
                            {
                                bool IsdispositionChanged = false;
                                if (oldProp.LatestDisposition != entity.LatestDisposition && entity.LatestDisposition != "AppointmentSet")
                                {
                                    IsdispositionChanged = true;
                                }

                                try
                                {
                                    var response = _sbAdapter.Value.SubmitLead(entity.Guid, null, true, IsdispositionChanged);

                                    if (response != null && response.Message.Type.Equals("error"))
                                    {
                                        ret.SBLeadError = response.Message.Text + ". This lead will not be saved in SMARTBoard until it's added.";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logger.Error("Error submitting SB lead!", ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }

                    if (entity.Inquiries?.Where(inq => inq.IsNew == true)?.ToList()?.Any() == true)
                    {
                        using (var dc = new DataContext())
                        {
                            var appointmentAfter = dc.Appointments.Where(p => p.PropertyID == entity.Guid).OrderByDescending(a => a.DateCreated).FirstOrDefault();
                            if (appointmentBefore != null && appointmentAfter != null)
                            {
                                //check if the appointment was cancelled while processing the inquiries
                                if (appointmentBefore.Status != appointmentAfter.Status && appointmentAfter.Status == AppointmentStatus.Cancelled)
                                {
                                    ret.SaveResult.Payload = new PropertySaveResultPayload
                                    {
                                        AppointmentID = appointmentAfter.Guid,
                                        GoogleEventID = appointmentAfter.GoogleEventID
                                    };
                                }
                            }
                        }
                    }
                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public override ICollection<Property> InsertMany(ICollection<Property> entities)
        {
            var result = new List<Property>(entities.Count);

            foreach (var entity in entities)
            {
                var ret = Insert(entity);
                result.Add(ret);
            }
            return result;
        }

        public override ICollection<Property> UpdateMany(ICollection<Property> entities)
        {
            var result = new List<Property>(entities.Count);

            foreach (var entity in entities)
            {
                var ret = Update(entity);
                result.Add(ret);
            }
            return result;
        }


        protected override void OnDeletedMany(Property[] entities)
        {
            using (var dataContext = new DataContext())
            {
                var territoryIds = entities.Select(e => e.TerritoryID);
                dataContext
                    .Territories
                    .Where(t => !t.IsDeleted && territoryIds.Contains(t.Guid))
                    .ToList()
                    .ForEach(t =>
                    {
                        t.Updated(SmartPrincipal.UserId);
                    });

                dataContext.SaveChanges();
            }

        }

        public ICollection<Property> GetOUPropertiesByStatusPaged(Guid ouID, string disposition, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            List<Property> ret;

            using (DataContext dc = new DataContext())
            {
                var ouHierarchy = dc.Database.SqlQuery<Guid>("select Guid from dbo.OUTree({0})", ouID);

                IQueryable<Property> setQuery = dc.Properties.Where(p => ouHierarchy.Contains(p.Territory.OUID) &&
                                                                    (string.IsNullOrEmpty(propertyNameSearch) || p.Name.Contains(propertyNameSearch)) &&
                                                                    p.Inquiries.Any(i => i.Disposition == disposition))
                                                             .OrderBy(p => p.Guid).Skip(pageIndex * itemsPerPage).Take(itemsPerPage);
                AssignIncludes(include, ref setQuery);
                ret = new List<Property>(setQuery.Count());
                ret = setQuery.ToList();
            }

            return ret;
        }

        public ICollection<Property> GetOUPropertiesPaged(Guid ouID, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "")
        {
            List<Property> ret;

            using (DataContext dc = new DataContext())
            {
                var ouHierarchy = dc.Database.SqlQuery<Guid>("select Guid from dbo.OUTree({0})", ouID);

                IQueryable<Property> setQuery = dc.Properties.Where(p => ouHierarchy.Contains(p.Territory.OUID) &&
                                                                    (string.IsNullOrEmpty(propertyNameSearch) || p.Name.Contains(propertyNameSearch)))
                                                             .OrderBy(p => p.Guid).Skip(pageIndex * itemsPerPage).Take(itemsPerPage);
                AssignIncludes(include, ref setQuery);
                ret = new List<Property>(setQuery.Count());
                ret = setQuery.ToList();
            }

            return ret;
        }

        public ICollection<Property> GetOUPropertiesWithProposal(Guid ouID, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            List<Property> ret = null;

            using (DataContext dc = new DataContext())
            {
                int skipcnt = pageIndex * itemsPerPage;
                dc.Database.CommandTimeout = 60;
                //-- exec [dbo].[usp_GetProposalsByOuId] '3f78b1b0-c0c5-4987-a5d5-32ee1c893460', NULL, 0,20 
                var propIds = dc.Database.SqlQuery<Guid>(" exec usp_GetOUPropertiesIdwithProposal {0} ,{1}, {2}, {3}", ouID, propertyNameSearch, skipcnt, itemsPerPage).ToList();

                var retQuery = dc.Properties.Where(p => propIds.Contains(p.Guid));


                //var ouHierarchy = dc
                //                    .Database
                //                    .SqlQuery<Guid>("select Guid from dbo.OUTree({0})", ouID)
                //                    .ToList();

                //var setQuery = dc
                //                .Properties
                //                .Where(p => !p.IsDeleted &&
                //                            ouHierarchy.Contains(p.Territory.OUID) &&
                //                            (string.IsNullOrEmpty(propertyNameSearch) || p.Name.Contains(propertyNameSearch)));

                //var propIds = setQuery
                //                .Select(p => p.Guid)
                //                .ToList();

                //var propWithProposalIds = dc
                //                .Proposal
                //                .Where(p => !p.IsDeleted && propIds.Contains(p.PropertyID))
                //                .Select(p => p.PropertyID)
                //                .ToList();

                //var retQuery = setQuery
                //                .Where(p => propWithProposalIds.Contains(p.Guid))
                //                .OrderBy(p => p.Name)
                //                .Skip(pageIndex * itemsPerPage)
                //                .Take(itemsPerPage);

                AssignIncludes(include, ref retQuery);
                ret = retQuery.ToList();

                //var propIds = ret
                //            .Select(p => p.Guid)
                //            .ToList();

                // we use a different context not to get navitation properties for results
                using (var uow = UnitOfWorkFactory())
                {
                    var proposals = uow.Get<Proposal>()
                                    .Where(p => !p.IsDeleted && propIds.Contains(p.PropertyID))
                                    .GroupBy(p => p.PropertyID)
                                    .ToDictionary(p => p.Key, p => p.OrderByDescending(po => po.DateCreated).FirstOrDefault().DateCreated);

                    foreach (var item in ret)
                    {
                        item.LastProposalDate = proposals[item.Guid];
                    }
                }
            }
            return ret;
        }


        public ICollection<Property> GetTerritoryPropertiesByStatusPaged(Guid territoryID, string disposition, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            List<Property> ret;

            using (DataContext dc = new DataContext())
            {
                IQueryable<Property> setQuery = dc.Properties.Where(p => p.TerritoryID == territoryID &&
                                                                (string.IsNullOrEmpty(propertyNameSearch) || p.Name.Contains(propertyNameSearch)) &&
                                                                p.Inquiries.Any(i => i.Disposition == disposition))
                                                             .OrderBy(p => p.Guid).Skip(pageIndex * itemsPerPage).Take(itemsPerPage);
                AssignIncludes(include, ref setQuery);
                ret = new List<Property>(setQuery.Count());
                ret = setQuery.ToList();
            }

            return ret;
        }

        public ICollection<Property> GetTerritoryPropertiesWithProposal(Guid territoryID, string propertyNameSearch, int pageIndex = 0, int itemsPerPage = 20, string include = "", string exclude = "")
        {
            List<Property> ret;

            using (DataContext dc = new DataContext())
            {
                //var inquiryStatus = (InquiryStatus)Enum.Parse(typeof(InquiryStatus), status.ToString());
                var setQuery = dc
                                .Properties
                                .Where(p => !p.IsDeleted &&
                                            p.TerritoryID == territoryID &&
                                            (string.IsNullOrEmpty(propertyNameSearch) || p.Name.Contains(propertyNameSearch)));
                var propIds = setQuery
                                .Select(p => p.Guid)
                                .ToList();

                var propWithProposalIds = dc
                                .Proposal
                                .Where(p => !p.IsDeleted &&
                                            propIds.Contains(p.PropertyID))
                                .Select(p => p.PropertyID)
                                .ToList();

                var retQuery = setQuery
                                .Where(p => propWithProposalIds.Contains(p.Guid))
                                .OrderBy(p => p.Name)
                                .Skip(pageIndex * itemsPerPage)
                                .Take(itemsPerPage);

                AssignIncludes(include, ref retQuery);
                ret = new List<Property>(retQuery.Count());
                ret = retQuery.ToList();

                propIds = ret
                            .Select(p => p.Guid)
                            .ToList();

                // we use a different context not to get navitation properties for results
                using (var uow = UnitOfWorkFactory())
                {
                    var proposals = uow.Get<Proposal>()
                                    .Where(p => !p.IsDeleted && propIds.Contains(p.PropertyID))
                                    .GroupBy(p => p.PropertyID)
                                    .ToDictionary(p => p.Key, p => p.OrderByDescending(po => po.DateCreated).FirstOrDefault().DateCreated);

                    foreach (var item in ret)
                    {
                        item.LastProposalDate = proposals[item.Guid];
                    }
                }
            }

            return ret;
        }

        public ICollection<CustomerLite> ListCustomersLite(Guid ouID, bool deep, int pageNumber = 0, DateTime? startDate = default(DateTime?), DateTime? endDate = default(DateTime?))
        {
            List<CustomerLite> ret = new List<CustomerLite>();

            using (DataContext dc = new DataContext())
            {


            }


            return ret;

        }

        public ICollection<Integrations.Common.Geo.Property> GetPropertiesInShape(string wkt, int maxResults = 10000)
        {
            if (string.IsNullOrEmpty(wkt))
            {
                throw new ArgumentNullException(nameof(wkt));
            }
            if (maxResults > 10000)
            {
                throw new ArgumentOutOfRangeException(nameof(maxResults), "The maximum number of results allowed is 10000");
            }

            var shapeGeo = DbGeometry.FromText(wkt);
            var propertiesCount = _geoProvider.PropertiesCountForWKT(wkt);
            var pages = propertiesCount / maxResults + (propertiesCount % maxResults != 0 ? 1 : 0);
            var properties = new List<Integrations.Common.Geo.Property>();
            for (int i = 0; i < pages; i++)
            {
                properties.AddRange(_geographyBridgeFactory().GetPropertiesForWellKnownText(wkt, maxResults, i));
            }

            return properties;
        }

        public CanCreateAppointmentResponse CanAddAppointmentOnProperty(CanCreateAppointmentRequest request)
        {
            using (var dc = new DataContext())
            {
                string propExternalID = null;
                Guid? territoryID = null;
                if (request.PropertyID.HasValue)
                {
                    var prop = dc.Properties.FirstOrDefault(p => !p.IsDeleted && !p.IsArchive && p.Guid == request.PropertyID.Value);
                    propExternalID = prop?.ExternalID;
                    territoryID = prop?.TerritoryID;
                }

                propExternalID = propExternalID ?? request.ExternalID;
                territoryID = territoryID ?? request.TerritoryID;

                if (string.IsNullOrEmpty(propExternalID) || !territoryID.HasValue)
                {
                    return new CanCreateAppointmentResponse
                    {
                        Status = CanCreateAppointmentStatus.Error,
                        DisplayMessage = "There was an error processing your request"
                    };
                }

                //check for duplicates
                var propertiesWithSameExternalId = dc
                    .Properties
                    .Include(p => p.Appointments)
                    .Include(p => p.Appointments.Select(x => x.Creator))
                    .Include(p => p.Appointments.Select(x => x.Assignee))
                    .Where(p =>
                            !p.IsDeleted &&
                            !p.IsArchive &&
                            p.ExternalID != null &&
                            p.ExternalID.Equals(propExternalID, StringComparison.InvariantCultureIgnoreCase))
                    ?.ToList();

                var properties = propertiesWithSameExternalId.OrderByDescending(p => p.DateCreated).Where(p => p.Appointments?.Any(a => a.Status != AppointmentStatus.Cancelled) == true).ToList();
                if (properties.Any() == true)
                {
                    foreach (var property in properties)
                    {
                        //get the time limit setting
                        var daysString = _ouSettingService.Value.GetOUSettingForPropertyID<string>(property.Guid, "Ignite.Appointment.Validation.Days") ?? "30";
                        var days = Convert.ToInt32(daysString);

                        if (property.Appointments?.Any(a => a.Status != AppointmentStatus.Cancelled && (DateTime.UtcNow - a.DateCreated).Days < days) == true)
                        {
                            var lastDateCreated = property
                                .Appointments
                                .Where(a => a.Status != AppointmentStatus.Cancelled && (DateTime.UtcNow - a.DateCreated).Days < days)
                                .Max(a => a.DateCreated);

                            var daysPast = (DateTime.UtcNow - lastDateCreated).Days;

                            var myAppointment = property.Appointments?.FirstOrDefault(a => a.Status != AppointmentStatus.Cancelled && a.AssigneeID == SmartPrincipal.UserId && (DateTime.UtcNow - a.DateCreated).Days < days);
                            if (myAppointment != null)
                            {
                                daysPast = (DateTime.UtcNow - myAppointment.DateCreated).Days;
                                return new CanCreateAppointmentResponse
                                {
                                    Status = CanCreateAppointmentStatus.AlreadyAssignedToMe,
                                    DisplayMessage = $"An appointment has already been set for this property by {myAppointment.Creator.FullName} and assigned to you. A new appointment can be set in {days - daysPast} days",
                                    PropertyID = property.Guid,
                                    TerritoryID = property.TerritoryID
                                };
                            }



                            return new CanCreateAppointmentResponse
                            {
                                Status = CanCreateAppointmentStatus.AlreadyAssigned,
                                DisplayMessage = $"An appointment has already been set for this property - Contact an Admin.  A new appointment can be set in {days - daysPast} days",
                                PropertyID = property.Guid,
                                TerritoryID = property.TerritoryID
                            };
                        }
                    }
                }

                var AppointmentType = GetAppoinmentType();

                return new CanCreateAppointmentResponse
                {
                    Status = CanCreateAppointmentStatus.CanCreate,
                    AppointmentType = AppointmentType,
                    DisplayMessage = "There's no other entry for the current property. You can create an appointment"
                };
            }

        }

        public List<DocType> GetAppoinmentType()
        {
            List<DocType> typeList = new List<DocType>();

            typeList.Add(new DocType() { Id = 1, Name = "Proposal" });
            typeList.Add(new DocType() { Id = 2, Name = "Follow Up" });
            typeList.Add(new DocType() { Id = 3, Name = "Site Survey" });
            typeList.Add(new DocType() { Id = 4, Name = "Install" });

            return typeList;
        }

        public SBPropertyDTO CreatePropertyFromSmartBoard(SBCreatePropertyRequest request, string apiKey)
        {
            using (var dc = new DataContext())
            {
                var territory = dc.Territories.FirstOrDefault(t => !t.IsDeleted && !t.IsArchived && t.Guid == request.TerritoryID);
                if (territory == null)
                {
                    throw new Exception("Territory not found");
                }

                // validate apiKey
                var sbSettings = _ouSettingService
                                    .Value
                                    .GetSettingsByOUID(territory.OUID)
                                    ?.FirstOrDefault(x => x.Name == SolarTrackerResources.SelectedSettingName)
                                    ?.GetValue<ICollection<SelectedIntegrationOption>>()?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("You don't have permissions to add the property to the specified territory");
                }

                var property = new Property
                {
                    Address1 = request.AddressLine1,
                    Address2 = request.AddressLine2,
                    City = request.City,
                    State = _sunlightAdapter.Value.GetState(request.State, "fullState"),
                    ZipCode = request.ZipCode,
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Name = $"{request.FirstName} {request.MiddleNameInitial} {request.LastName}",
                    TerritoryID = request.TerritoryID,
                    SmartBoardId = request.LeadID
                };

                var propertyData = Insert(property);

                return new SBPropertyDTO(propertyData);
            }
        }

        public ICollection<Property> GetProperties(GetPropertiesRequest propertiesRequest)
        {
            if (propertiesRequest == null)
                throw new ArgumentNullException(nameof(propertiesRequest));

            if ((propertiesRequest.GeoPropertiesRequest == null && propertiesRequest.PropertiesRequest == null) || propertiesRequest.TerritoryID == Guid.Empty)
                return new List<Property>(0);

            //client app needs the notes count when using this API. Make sure to add PropertyNotes to the include query
            var includeString = string.Empty;
            if (string.IsNullOrEmpty(propertiesRequest?.PropertiesRequest?.Include))
            {
                includeString = "PropertyNotes";
            }
            else
            {
                if (!propertiesRequest.PropertiesRequest.Include.Contains("PropertyNotes"))
                {
                    includeString = $"{propertiesRequest.PropertiesRequest.Include}&PropertyNotes";
                }
                else
                {
                    includeString = propertiesRequest.PropertiesRequest.Include;
                }

            }
            //  need all properties from territory to see which are new or existing
            var territoryProperties =
                List(itemsPerPage: int.MaxValue, filter: $"TerritoryID={propertiesRequest.TerritoryID}",
                    include: propertiesRequest.PropertiesRequest != null ? propertiesRequest.PropertiesRequest.Include : string.Empty)
                    .ToList();

            if (territoryProperties?.Any() == true)
            {
                territoryProperties.ForEach(x =>
                {
                    x.PropertyNotesCount = x.PropertyNotes?.Where(p => !p.IsDeleted)?.Count();
                    if (propertiesRequest?.PropertiesRequest?.Include?.Contains("PropertyNotes") != true)
                    {
                        x.PropertyNotes = new List<PropertyNote>();
                    }
                });
            }

            if (propertiesRequest?.AreaViewBounds != null)
            {
                var boundsGeo = DbGeometry.FromText(propertiesRequest.AreaViewBounds.ToWKT());

                territoryProperties = territoryProperties
                        .Where(p => p.Location() != null)
                        .Where(p => boundsGeo.Contains(p.Location()))
                        .ToList();
            }

            //  get properties that will be returned to client from TM
            var properties = territoryProperties;
            if (propertiesRequest.PropertiesRequest != null)
            {
                if (propertiesRequest.PropertiesRequest.LastRetrievedDate.HasValue)
                {
                    properties = territoryProperties.Where(p =>
                        p.DateCreated >= propertiesRequest.PropertiesRequest.LastRetrievedDate.Value ||
                        p.DateLastModified >= propertiesRequest.PropertiesRequest.LastRetrievedDate.Value)
                        .ToList();
                }

                properties = properties.Skip((propertiesRequest.PropertiesRequest.PageNumber - 1) * propertiesRequest.PropertiesRequest.ItemsPerPage)
                        .Take(propertiesRequest.PropertiesRequest.ItemsPerPage)
                        .ToList();
            }

            // get geo properties
            var newProperties = new List<Property>();
            if (propertiesRequest.GeoPropertiesRequest != null)
            {
                var territory = UnitOfWorkFactory()
                                    .Get<Territory>()
                                    .FirstOrDefault(t => t.Guid == propertiesRequest.TerritoryID);

                foreach (var propReq in propertiesRequest.GeoPropertiesRequest)
                {
                    // if the client sends AreaWellKnownText as string.Empty, we'll use the Territory WKT
                    // if it's null, we don't replace it, it means they don't care about WKT
                    if ((propReq.AreaWellKnownTextArray == null || propReq.AreaWellKnownTextArray.Count == 0) && propReq.AreaWellKnownText == string.Empty)
                    {
                        propReq.AreaWellKnownText = territory.WellKnownText;
                    }

                    // set the viewBounds to the geo request as well
                    if (propertiesRequest.AreaViewBounds != null)
                    {
                        propReq.AreaViewBounds = propertiesRequest.AreaViewBounds;
                    }
                }

                var geoProperties = _geographyBridgeFactory().GetProperties(propertiesRequest.GeoPropertiesRequest).ToList();

                var commonProperties = geoProperties
                    .Where(gp => territoryProperties.Any(p => gp.Id.Equals(p.ExternalID, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

                geoProperties = geoProperties.Except(commonProperties).ToList();
                newProperties = geoProperties.Select(p => p.ToCoreProperty(propertiesRequest.TerritoryID)).ToList();
            }

            return propertiesRequest.PropertiesRequest != null
                ? properties.Union(newProperties).ToList()
                : newProperties;
        }


        public ICollection<Property> GetPropertiesSearch(Guid territoryid, string searchvalue)
        {
            // return new List<Property>();
            var territoryProperties = List(itemsPerPage: int.MaxValue, filter: $"TerritoryID={territoryid}", include: string.Empty).ToList();

            if (territoryProperties?.Any() == true)
            {
                territoryProperties.ForEach(x =>
                {
                    x.PropertyNotesCount = x.PropertyNotes?.Where(p => !p.IsDeleted)?.Count();
                });
            }

            var territory = UnitOfWorkFactory().Get<Territory>().FirstOrDefault(t => t.Guid == territoryid);

            // get geo properties
            var newProperties = new List<Property>();

            var geoProperties = _geographyBridgeFactory().GetPropertiesForWellKnownText(territory.WellKnownText, 5000).ToList();

            var commonProperties = geoProperties.Where(gp => territoryProperties.Any(p => gp.Id.Equals(p.ExternalID, StringComparison.InvariantCultureIgnoreCase))).ToList();
            geoProperties = geoProperties.Except(commonProperties).ToList();
            newProperties = geoProperties.Select(p => p.ToCoreProperty(territoryid)).ToList();

            var propertieslist = territoryProperties.Union(newProperties).Where(x => x.Name.ToLowerInvariant().Contains(searchvalue.ToLowerInvariant()) || x.Address1.ToLowerInvariant().Contains(searchvalue.ToLowerInvariant())).ToList();
            return propertieslist;
        }

        public void SyncPrescreenBatchPropertiesAttributes(Guid prescreenBatchId)
        {
            if (prescreenBatchId == Guid.Empty) throw new ArgumentException(nameof(prescreenBatchId));

            using (var context = new DataContext())
            {
                var prescreenBatch = context.PrescreenBatches.Include(pb => pb.Territory).FirstOrDefault(pb => pb.Guid == prescreenBatchId);
                if (prescreenBatch == null) throw new ApplicationException($"Invalid {nameof(prescreenBatchId)}");
                if (prescreenBatch.Territory == null) throw new ApplicationException("Invalid prescreenbatch territory");

                var territoryID = prescreenBatch.TerritoryID;

                var properties = List(itemsPerPage: int.MaxValue, filter: $"TerritoryID={territoryID}", include: "Attributes").ToList();
                var geoProperties = _geographyBridgeFactory().GetProperties(new List<PropertiesRequest>
                {
                    new PropertiesRequest
                    {
                        AreaWellKnownText = prescreenBatch.Territory.WellKnownText,
                        IncludedEntities = new List<string> {"Attributes"},
                        IncludedEntitiesFilter = new List<PropertyDataFilter>
                        {
                            new PropertyDataFilter
                            {
                                Name = "Attributes",
                                Filters = new Dictionary<string, string>
                                {
                                    {"DateCreated", DateTime.UtcNow.AddDays(-2).ToString("MM/dd/yyyy")},
                                    {"TerritoryID", prescreenBatch.Territory.Guid.ToString()}
                                }
                            }
                        }
                    }
                });
                geoProperties = geoProperties
                    .Where(gp => gp.Attributes != null
                                 && gp.Attributes.Any()
                                 && properties.Any(p => p.ExternalID.Equals(gp.Id, StringComparison.InvariantCultureIgnoreCase)))
                    .ToList();

                foreach (var geoProperty in geoProperties)
                {
                    var territoryProperty = properties.FirstOrDefault(p => p.ExternalID.Equals(geoProperty.Id, StringComparison.InvariantCultureIgnoreCase));
                    if (territoryProperty == null) continue;
                    var territoryPropertyMaxAttributeDate = territoryProperty.Attributes != null &&
                                                            territoryProperty.Attributes.Any()
                        ? territoryProperty.Attributes.OrderByDescending(a => a.DateCreated).First().DateCreated
                        : SqlDateTime.MinValue.Value;

                    var geoPropertyNewAttributes = geoProperty.Attributes
                        .Where(a => a.TerritoryID == territoryID && a.DateCreated >= territoryPropertyMaxAttributeDate)
                        .ToList();
                    if (!geoPropertyNewAttributes.Any()) continue;

                    var newPropertyAttributes = geoPropertyNewAttributes.Select(a => new PropertyAttribute
                    {
                        PropertyID = territoryProperty.Guid,
                        DisplayType = a.DisplayType,
                        Value = a.Value,
                        AttributeKey = a.AttributeKey,
                        ExpirationDate = a.ExpirationDate,
                        TerritoryID = territoryID
                    });

                    context.PropertyAttributes.AddRange(newPropertyAttributes);
                }

                context.SaveChanges();
            }
        }

        public void SyncInstantPrescreenPropertyAttributes(Guid prescreenInstantId)
        {
            if (prescreenInstantId == Guid.Empty) throw new ArgumentException(nameof(prescreenInstantId));

            using (var context = new DataContext())
            {
                var prescreenInstant = context
                                .PrescreenInstants
                                .Include(pi => pi.Property.Attributes)
                                .FirstOrDefault(pb => pb.Guid == prescreenInstantId);

                if (prescreenInstant == null) throw new ApplicationException($"Invalid {nameof(prescreenInstantId)}");

                var property = prescreenInstant.Property;

                if (property == null) throw new ApplicationException("Invalid prescreenInstant Property");


                if (string.IsNullOrEmpty(property.ExternalID))
                {
                    return;
                }

                var propertyId = prescreenInstant.PropertyID;
                var territoryID = property.TerritoryID;

                var geoProperties = _geographyBridgeFactory().GetProperties(new List<PropertiesRequest>
                {
                    new PropertiesRequest
                    {
                        OnlyActive = true,
                        IncludedLocationIDs = new List<string> {property.ExternalID},

                        IncludedEntities = new List<string> {"Attributes"},
                        IncludedEntitiesFilter = new List<PropertyDataFilter>
                        {
                            new PropertyDataFilter
                            {
                                Name = "Attributes",
                                Filters = new Dictionary<string, string>
                                {
                                    {"DateCreated", DateTime.UtcNow.AddDays(-2).ToString("MM/dd/yyyy")},
                                    {"TerritoryID", territoryID.ToString()}
                                }
                            }
                        }
                    }
                });
                var geoProperty = geoProperties
                    .FirstOrDefault(gp => gp.Attributes != null
                                 && gp.Attributes.Any()
                                 && property.ExternalID.Equals(gp.Id, StringComparison.InvariantCultureIgnoreCase));
                if (geoProperty == null)
                {
                    return;
                }

                var propertyMaxAttributeDate = property.Attributes != null && property.Attributes.Any()
                        ? property.Attributes.OrderByDescending(a => a.DateCreated).First().DateCreated
                        : SqlDateTime.MinValue.Value;

                var geoPropertyNewAttributes = geoProperty.Attributes
                        .Where(a => a.TerritoryID == territoryID && a.DateCreated >= propertyMaxAttributeDate)
                        .ToList();

                if (!geoPropertyNewAttributes.Any())
                {
                    return;
                }

                var newPropertyAttributes = geoPropertyNewAttributes.Select(a => new PropertyAttribute
                {
                    PropertyID = property.Guid,
                    DisplayType = a.DisplayType,
                    Value = a.Value,
                    AttributeKey = a.AttributeKey,
                    ExpirationDate = a.ExpirationDate,
                    TerritoryID = territoryID
                });

                context.SaveChanges();
            }
        }

        public Property SyncProperty(Guid propertyID, string include = "")
        {
            if (propertyID == Guid.Empty)
                throw new ArgumentException($"Invalid {nameof(propertyID)}");

            using (var uow = UnitOfWorkFactory())
            {
                //client needs the notes count
                var includeString = string.Empty;
                if (string.IsNullOrEmpty(include))
                {
                    includeString = "PropertyNotes";
                }
                else
                {
                    if (!include.Contains("PropertyNotes"))
                    {
                        includeString = $"{include}&PropertyNotes";
                    }
                    else
                    {
                        includeString = include;
                    }

                }
                var query = uow.Get<Property>();
                AssignIncludes(include, ref query);

                var property = query
                    .Include(p => p.Attributes)
                    .Include(p => p.PropertyBag)
                    .Include(p => p.Occupants.Select(o => o.PropertyBag))
                    .FirstOrDefault(p => p.Guid == propertyID);

                if (property == null)
                    throw new ApplicationException("Invalid property");
                if (string.IsNullOrWhiteSpace(property.ExternalID) || property.ExternalID?.ToLower() == property.Guid.ToString().ToLower())
                    return property;

                var geoProperties = _geographyBridgeFactory().GetProperties(new List<PropertiesRequest>
                {
                    new PropertiesRequest
                    {
                        OnlyActive = true,
                        IncludedLocationIDs = new List<string> {property.ExternalID},
                        IncludedEntitiesFilter = new List<PropertyDataFilter>
                        {
                            new PropertyDataFilter {Name = "Occupants"},
                            new PropertyDataFilter {Name = "PropertyBag"},
                            new PropertyDataFilter
                            {
                                Name = "Attributes",
                                Filters = new Dictionary<string, string> {{"TerritoryID", property.TerritoryID.ToString()}}
                            }
                        }
                    }
                });
                if (geoProperties == null || !geoProperties.Any(gp => gp.Id.Equals(property.ExternalID, StringComparison.InvariantCultureIgnoreCase)))
                    return property;

                var geoProperty = geoProperties.First(gp => gp.Id.Equals(property.ExternalID, StringComparison.InvariantCultureIgnoreCase))
                        .ToCoreProperty(property.TerritoryID);

                if (geoProperty.Occupants != null && geoProperty.Occupants.Any())
                {
                    if (property.Occupants == null || !property.Occupants.Any())
                        property.Occupants = geoProperty.Occupants;
                }
                if (geoProperty.PropertyBag != null && geoProperty.PropertyBag.Any())
                {
                    if (property.PropertyBag == null || !property.PropertyBag.Any())
                        property.PropertyBag = geoProperty.PropertyBag;
                }
                if (geoProperty.Attributes != null && geoProperty.Attributes.Any())
                {
                    if (property.Attributes == null || !property.Attributes.Any())
                        property.Attributes = geoProperty.Attributes;
                    else
                    {
                        var propertyMaxAttributeDate = property.Attributes
                          .OrderByDescending(a => a.DateCreated)
                          .First().DateCreated;
                        var newAttributes = geoProperty.Attributes.Where(a => a.DateCreated >= propertyMaxAttributeDate).ToList();
                        newAttributes.ForEach(a => property.Attributes.Add(a));
                    }
                }

                uow.SaveChanges();

                property.PropertyNotesCount = property.PropertyNotes?.Where(x => !x.IsDeleted)?.Count();

                if (!include.Contains("PropertyNotes"))
                {
                    property.PropertyNotes = new List<PropertyNote>();
                }

                //seems that the query is bringing deleted power consumptions from the db and client does not differentiate
                if (property?.PowerConsumptions?.Any() == true)
                {
                    property.PowerConsumptions = property.PowerConsumptions.Where(pc => !pc.IsDeleted).ToList();
                }
                return property;
            }
        }


        public async Task<Property> PropertyBagsbyID(Guid propertyID)
        {
            if (propertyID == Guid.Empty)
                throw new ArgumentException($"Invalid {nameof(propertyID)}");

            using (var db = new DataContext())
            {
                var property = await db.Properties.Include(p => p.PropertyBag).AsNoTracking().FirstOrDefaultAsync(p => p.Guid == propertyID);

                if (property == null)
                    throw new ApplicationException("Invalid property");

                return property;
            }
        }

        public SolarTariff GetTariffByGenabilityProviderAccountID(string id)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var property = uow
                                .Get<Property>()
                                .AsNoTracking()
                                .FirstOrDefault(p => p.GenabilityProviderAccountID == id);
                if (property == null)
                {
                    return null;
                }

                return uow
                        .Get<Proposal>()
                        .Where(p => p.PropertyID == property.Guid
                                 && p.Tariff != null
                                 && p.Tariff.TariffName != null)
                        .Select(p => p.Tariff)
                        .AsNoTracking()
                        .FirstOrDefault();
            }
        }



        public async Task<IEnumerable<Territories>> GetTerritoriesList(Guid propertyid, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = await dc.Properties.FirstOrDefaultAsync(x => x.Guid == propertyid);

                if (property == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.NotFound, ReasonPhrase = "No lead found with the specified ID(s)" });
                    //throw new Exception("No lead found with the specified ID(s)");
                }

                //-- exec usp_GetTerritoryIdsNameByapiKey 29.973433, -95.243265, '1f82605d3fe666478f3f4f1ee25ae828'
                var TerritoriesList = dc
             .Database
             .SqlQuery<Territories>("exec usp_GetTerritoryIdsNameByapiKey @latitude, @longitude, @apiKey", new SqlParameter("@latitude", property.Latitude), new SqlParameter("@longitude", property.Longitude), new SqlParameter("@apiKey", apiKey))
             .ToList();

                return TerritoriesList;
            }
        }

        public async Task<IEnumerable<Territories>> GetTerritoryListbyApikey(string apiKey, double Lat, double Long)
        {
            using (var dc = new DataContext())
            {
                //var TerritoriesList = dc.Database.SqlQuery<Territories>("exec usp_GetTerritoryListByapiKeyOnly @apiKey", new SqlParameter("@apiKey", apiKey)).ToList();

                var TerritoriesList = dc.Database.SqlQuery<Territories>("exec usp_GetTerritoryIdsNameByapiKey @latitude, @longitude, @apiKey", new SqlParameter("@latitude", Lat), new SqlParameter("@longitude", Long), new SqlParameter("@apiKey", apiKey)).ToList();

                return TerritoriesList;
            }
        }

        public async Task<IEnumerable<Territories>> GetTerritoryListbyApikeyMaster(string apiKey, double Lat, double Long)
        {
            using (var dc = new DataContext())
            {
                //var TerritoriesList = dc.Database.SqlQuery<Territories>("exec usp_GetTerritoryListByapiKeyOnly @apiKey", new SqlParameter("@apiKey", apiKey)).ToList();

                var TerritoriesList = dc.Database.SqlQuery<Territories>("exec usp_GetTerritoryIdsNameByapiKeyMaster @latitude, @longitude, @apiKey", new SqlParameter("@latitude", Lat), new SqlParameter("@longitude", Long), new SqlParameter("@apiKey", apiKey)).ToList();

                return TerritoriesList;
            }
        }

        public async Task<IEnumerable<TerritoryApikey>> TerritoryNApikey(double Lat, double Long)
        {
            using (var dc = new DataContext())
            {
                var TerritoriesList = dc.Database.SqlQuery<TerritoryApikey>("exec sp_GetTerritoryIdsForGeoCoordinates @latitude, @longitude", new SqlParameter("@latitude", Lat), new SqlParameter("@longitude", Long)).ToList();

                return TerritoriesList;
            }
        }

        public SBPropertyDTO EditPropertyNameFromSB(long igniteID, SBPropertyNameDTO Request)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = dc.Properties.Include(x => x.Occupants)
                                            .Include(x => x.PropertyBag)
                                            .FirstOrDefault(x => x.Id == igniteID);

                if (property == null)
                {
                    throw new Exception("No lead found with the specified ID");
                }

                //get the user who transfered the Lead Territory
                var user = dc.People.FirstOrDefault(x => !x.IsDeleted
                                               && ((x.EmailAddressString.Equals(Request.UserEmailId)) || (x.SmartBoardID == Request.UserId)));
                if (user == null)
                {
                    throw new Exception("No user found with the specified ID");
                }
                if (Request.DispositionTypeId != null && Request.DispositionTypeId > 0)
                {
                    if (Request.DispositionTypeId != property.DispositionTypeId)
                    {
                        var dispSettings = _ouSettingService.Value.GetSettingsByPropertyID(property.Guid)?.Where(s => s.Name == OUSetting.NewDispositions)?.ToList();
                        var dispositions = dispSettings?.SelectMany(s => JsonConvert.DeserializeObject<List<DispositionV2DataView>>(s.Value))?.ToList().Where(x => x.SBTypeId == Request.DispositionTypeId).FirstOrDefault();

                        property.DispositionTypeId = dispositions != null ? Request.DispositionTypeId : property.DispositionTypeId;
                        property.LatestDisposition = dispositions != null ? dispositions.Name : property.LatestDisposition;
                        property.Updated(user.Guid);
                        dc.SaveChanges();


                        var inquiry = new Inquiry
                        {
                            Guid = Guid.NewGuid(),
                            PropertyID = property.Guid,
                            PersonID = user.Guid,
                            Notes = "Sales Rap(SB): " + user.Name,
                            Lat = property.Latitude,
                            Lon = property.Longitude,
                            Name = property.LatestDisposition,
                            DateCreated = DateTime.UtcNow,
                            CreatedByID = property.CreatedByID,
                            Disposition = property.LatestDisposition,
                            DispositionTypeId = property.DispositionTypeId,
                            IsNew = true
                        };

                        _inquiryService.Value.Insert(inquiry);

                    }
                }

                var mainOccupant = property.GetMainOccupant();
                if (mainOccupant != null)
                {
                    if (mainOccupant.FirstName == Request.ExistFirstName && mainOccupant.LastName == Request.ExistLastName && mainOccupant.Guid != null)
                    {

                        using (var dataContext = new DataContext())
                        {
                            var occupant = dataContext.Occupants.Where(x => x.Guid == mainOccupant.Guid).FirstOrDefault();
                            occupant.FirstName = Request.NewFirstName;
                            occupant.LastName = Request.NewLastName;
                            dataContext.SaveChanges();
                        }

                        //update new fname - lname into occupant and Property 
                        if (property?.PropertyBag?.FirstOrDefault(f => f.DisplayName == "Email Address")?.Value == Request.ExistEmailAddress)
                        {
                            using (var dataContext = new DataContext())
                            {
                                // update propertybag 
                                var Fields = property?.PropertyBag?.FirstOrDefault(f => f.DisplayName == "Email Address");
                                Fields.Value = Request.NewEmailAddress;
                                dataContext.SaveChanges();
                            }
                        }

                        property.Name = $"{Request.NewFirstName} {mainOccupant.MiddleInitial} {Request.NewLastName}".Replace("  ", " ");
                        property.DateLastModified = DateTime.UtcNow;
                        dc.SaveChanges();

                    }
                }


                using (var data = new DataContext())
                {
                    property = data.Properties.Include(x => x.Occupants).Include(x => x.PropertyBag).FirstOrDefault(x => x.Id == igniteID);
                }
                // Update(Latestproperty); 

                return new SBPropertyDTO(property);
            }
        }

        public static string SerializeObject(object obj)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlDoc.Load(ms);
                return xmlDoc.InnerXml;
            }
        }

        public static string SerializeObject<T>(T dataObject)
        {
            if (dataObject == null)
            {
                return string.Empty;
            }
            try
            {
                using (StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string xml)
             where T : new()
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }
            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Error(typeof(T), "Failed to serialize xml to object: " + xml, ex);
                return new T();
            }
        }

        public async Task<EsIDResponse> GetEsidByAddress(Guid propertyid)
        {

            string Esidurl = System.Configuration.ConfigurationManager.AppSettings["EsIdUrl"];
            string Esidparams = System.Configuration.ConfigurationManager.AppSettings["EsIdParams"];

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Esidurl);

                using (var dataContext = new DataContext())
                {
                    var propty = dataContext.Properties.Where(x => x.Guid == propertyid).FirstOrDefault();
                    string address = propty != null ? propty.Address1 : "";
                    string zipcode = propty != null ? propty.ZipCode : "";
                    Esidparams = Esidparams.Replace("{address}", address).ToString();
                    Esidparams = Esidparams.Replace("{zip}", zipcode).ToString();
                    Esidparams = Esidparams.Replace("%26", "&").ToString();
                }
                HttpResponseMessage response = await client.GetAsync(Esidparams);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ApplicationException(await response.Content.ReadAsStringAsync());
                }

                var serializer = new XmlSerializer(typeof(EsIDResponse));
                EsIDResponse result;

                string resp = await response.Content.ReadAsStringAsync();
                using (TextReader reader = new StringReader(resp))
                {
                    result = (EsIDResponse)serializer.Deserialize(reader);
                }

                return result;
            }
        }


        public async Task<bool> IsPropertyAvailable(long igniteId)
        {
            using (var data = new DataContext())
            {
                var property = data.Properties.FirstOrDefault(x => x.Id == igniteId);
                if (property == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }   

        public List<Territory> GetTerritoriesFromAddress(Property req)
        {
            using (var dc = new DataContext())
            {
                //first get the property
                var property = dc.Properties.Where(x => x.ZipCode == req.ZipCode).Select(a => a.TerritoryID).ToList();
                var territories = dc
                                 .Territories
                                 .Where(o => property.Contains(o.Guid)).ToList();

                return territories;
            }
        }

        public Property AddProperty(Property entity)
        {
            Property ret = null;

            entity.PrepareNavigationProperties(SmartPrincipal.UserId);

            using (var dataContext = new DataContext())
            {
                ret = base.Update(entity, dataContext);
                if (!ret.SaveResult.Success) throw new Exception($"{ret.SaveResult.Exception} {ret.SaveResult.ExceptionMessage}");
            }

            return ret;
        }
    }
}
