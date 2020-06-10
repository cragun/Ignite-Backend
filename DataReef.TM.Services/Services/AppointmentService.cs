using DataReef.Core.Classes;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Logging;
using DataReef.TM.Classes;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using DataReef.TM.Models.DataViews;
using DataReef.TM.Models.DTOs.FinanceAdapters.SST;
using DataReef.TM.Models.DTOs.Integrations;
using DataReef.TM.Models.DTOs.SmartBoard;
using DataReef.TM.Models.Enums;
using DataReef.TM.Services.Services.FinanceAdapters.SolarSalesTracker;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class AppointmentService : DataService<Appointment>, IAppointmentService
    {
        private const int INTEGRATION_TOKEN_EXPIRATION_DAYS = 30;
        private readonly IOUAssociationService _ouAssociationService;
        private readonly Lazy<IOUService> _ouService;
        private readonly Lazy<IOUSettingService> _ouSettingsService;
        private readonly Lazy<IAssignmentService> _assignmentService;
        private readonly Lazy<IUserInvitationService> _userInvitationService;
        private readonly Lazy<IPropertyService> _propertyService;
        private readonly Lazy<IPersonService> _personService;

        public AppointmentService(ILogger logger,
            IOUAssociationService ouAssociationService,
            Lazy<IOUService> ouService,
            Lazy<IOUSettingService> ouSettingsService,
            Lazy<IAssignmentService> assignmentService,
            Lazy<IUserInvitationService> userInvitationService,
            Lazy<IPropertyService> propertyService,
            Lazy<IPersonService> personService,
            Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _ouAssociationService = ouAssociationService;
            _ouService = ouService;
            _ouSettingsService = ouSettingsService;
            _assignmentService = assignmentService;
            _userInvitationService = userInvitationService;
            _propertyService = propertyService;
            _personService = personService;
        }

        public Appointment SetAppointmentStatus(Guid appointmentID, AppointmentStatus status)
        {
            using (var uow = UnitOfWorkFactory())
            {
                var appointment = uow
                    .Get<Appointment>()
                    .Include(x => x.Property)
                    .Include(x => x.Creator)
                    .Include(x => x.Assignee)
                    .FirstOrDefault(x => x.Guid == appointmentID);

                if (appointment == null)
                {
                    throw new ArgumentException("No appointment with the specified id could be found.");
                }

                if (appointment.Status != status)
                {
                    appointment.Status = status;

                    if (status == AppointmentStatus.Declined)
                    {
                        appointment.AssigneeID = appointment.CreatedByID;
                    }

                    appointment.Updated(SmartPrincipal.UserId);
                    uow.SaveChanges();

                    if (status == AppointmentStatus.Accepted || status == AppointmentStatus.Declined)
                    {
                        //send email to creator
                        var template = new AppointmentStatusChangedTemplate
                        {
                            RecipientEmailAddress = appointment.Creator.EmailAddressString,
                            EmailSubject = $"Appointment for {appointment.Property?.Name} updated",
                            PropertyName = appointment.Property?.Name,
                            Address = $"{appointment?.Property?.Address1} {appointment?.Property?.City}, {appointment?.Property?.State}",
                            AppointmentStatus = status.ToString(),
                            AppointmentStartDate = appointment.StartDate,
                            AppointmentEndDate = appointment.EndDate,
                            AppointmentDetails = appointment.Details
                        };

                        Mail.Library.SendAppointmentStatusChangedEmail(template);
                    }
                }

                return appointment;
            }
        }

        public IEnumerable<SBAppointmentDTO> GetSmartboardAppointmentsForPropertyID(long propertyID, string apiKey)
        {
            using (var uow = UnitOfWorkFactory())
            {
                // we 1st get the property
                var property = uow
                        .Get<Property>()
                        .Include(p => p.Appointments)
                        .Where(p => !p.IsDeleted)
                        .FirstOrDefault(p => p.Id == propertyID);

                if (property == null || property?.Appointments?.Any(x => !x.IsDeleted) != true)
                {
                    yield break;
                }

                // validate apiKey
                var sbSettings = _ouSettingsService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(property.Guid, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    yield break;
                }

                if (property.Appointments?.Any(x => !x.IsDeleted) != true)
                {
                    yield break;
                }

                foreach(var app in property.Appointments.Where(a => !a.IsDeleted))
                {
                    yield return new SBAppointmentDTO(app);
                }
                
            }
            yield break;
        }

        public SBAppointmentDTO InsertNewAppointment(SBCreateAppointmentRequest request, string apiKey)
        {
            using(var dc = new DataContext())
            {
                // get creator and assignee in the same call to avoid going to the db twice
                var people = dc
                    .People
                    .Where(x => 
                        !x.IsDeleted && 
                        (x.SmartBoardID.Equals(request.CreatedByID, StringComparison.InvariantCultureIgnoreCase) 
                            || x.SmartBoardID.Equals(request.AssignedToID, StringComparison.InvariantCultureIgnoreCase)));

                var creator = people.FirstOrDefault(x => x.SmartBoardID.Equals(request.CreatedByID, StringComparison.InvariantCultureIgnoreCase));
                
                if (creator == null)
                {
                    throw new Exception("No account linked to the specified CreatedByID");
                }
                var assignee = people.FirstOrDefault(x => x.SmartBoardID.Equals(request.AssignedToID, StringComparison.InvariantCultureIgnoreCase));
                
                if (assignee == null)
                {
                    throw new Exception("No account linked to the specified AssignedToID");
                }

                //we get the property
                var property = dc
                    .Properties
                    .FirstOrDefault(p => !p.IsDeleted && p.SmartBoardId == request.LeadID);
                if (property == null)
                {
                    throw new Exception("No lead found with the specified ID");
                }

                //we validate the token
                var sbSettings = _ouSettingsService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(property.Guid, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("Please send Valid Apikey base on LeadId.");
                }

                var appointment = new Appointment
                {
                    PropertyID = property.Guid,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    CreatedByID = creator.Guid,
                    CreatedByName = creator.Name,
                    AssigneeID = assignee.Guid,
                    Details = request.AppointmentDescription,
                    Address = property.Address1,
                    Status = AppointmentStatus.Unknown,
                    GoogleEventID = request.GoogleEventID,
                    TimeZone = request.TimeZone
                };

                dc.Appointments.Add(appointment);
                dc.SaveChanges();

                return new SBAppointmentDTO(appointment);
            }
        }

        public SBAppointmentDTO EditSBAppointment(SBEditAppointmentRequest request, Guid appointmentID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //we get the specified appointment
                var appointment = dc
                    .Appointments
                    .FirstOrDefault(x => !x.IsDeleted && x.Guid == appointmentID);
                if(appointment == null)
                {
                    throw new Exception("Appointment not found");
                }

                // we get the person setting
                Person creator = null;
                if (!string.IsNullOrEmpty(request.CreatedByID))
                {
                    creator = dc.People.FirstOrDefault(x => x.SmartBoardID.Equals(request.CreatedByID, StringComparison.InvariantCultureIgnoreCase));

                    if (creator == null)
                    {
                        throw new Exception("No account linked to the specified CreatedByID");
                    }
                }

                Person assignee = null;
                if (!string.IsNullOrEmpty(request.AssignedToID))
                {
                    assignee = dc.People.FirstOrDefault(x => x.SmartBoardID.Equals(request.AssignedToID, StringComparison.InvariantCultureIgnoreCase));

                    if (assignee == null)
                    {
                        throw new Exception("No account linked to the specified AssignedToID");
                    }
                }

                //we get the property
                Property property = null;
                if (request.LeadID.HasValue)
                {
                    property = dc
                    .Properties
                    .FirstOrDefault(p => !p.IsDeleted && p.Id == request.LeadID);
                    if (property == null)
                    {
                        throw new Exception("No lead found with the specified ID");
                    }
                }

                var propertyID = property?.Guid ?? appointment.PropertyID;
                
                //we validate the token
                var sbSettings = _ouSettingsService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(property.Guid, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("Please send Valid Apikey base on LeadId.");
                }


                appointment.PropertyID = property?.Guid ?? appointment.PropertyID;
                appointment.StartDate = request.StartDate ?? appointment.StartDate;
                appointment.EndDate = request.EndDate ?? appointment.EndDate;
                appointment.CreatedByID = creator?.Guid ?? appointment.CreatedByID;
                appointment.CreatedByName = creator?.Name ?? appointment.CreatedByName;
                appointment.AssigneeID = assignee?.Guid ?? appointment.AssigneeID;
                appointment.Details = request.AppointmentDescription ?? appointment.Details;
                appointment.Address = property?.Address1 ?? appointment.Address;
                appointment.GoogleEventID = request.GoogleEventID ?? appointment.GoogleEventID;
                appointment.TimeZone = request.TimeZone ?? appointment.TimeZone;

                dc.SaveChanges();

                return new SBAppointmentDTO(appointment);
            }
        }

        public SBAppointmentDTO SetAppointmentStatusFromSmartboard(SBSetAppointmentStatusRequest request, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //we get the specified appointment
                var appointment = dc
                    .Appointments
                    .FirstOrDefault(x => !x.IsDeleted && x.Guid == request.AppointmentID);
                if (appointment == null)
                {
                    throw new Exception("Appointment not found");
                }

                var propertyID = appointment.PropertyID;
                
                //we validate the token
                var sbSettings = _ouSettingsService
                                    .Value
                                    .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(propertyID, SolarTrackerResources.SelectedSettingName)?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("Please send Valid Apikey base on LeadId.");
                }

                SetAppointmentStatus(request.AppointmentID, request.Status);

                dc.SaveChanges();

                return new SBAppointmentDTO(appointment);
            }
        }

        public bool DeleteSBAppointment(Guid appointmentID, string apiKey)
        {
            using (var dc = new DataContext())
            {
                //we get the specified appointment
                var appointment = dc
                    .Appointments
                    .FirstOrDefault(x => !x.IsDeleted && x.Guid == appointmentID);
                if (appointment == null)
                {
                    throw new Exception("Appointment not found");
                }

                // we validate the token
                 var sbSettings = _ouSettingsService
                                     .Value
                                     .GetOUSettingForPropertyID<ICollection<SelectedIntegrationOption>>(appointment.PropertyID, SolarTrackerResources.SelectedSettingName)?
                                     .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                     .Data?
                                     .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("Please send Valid Apikey base on LeadId.");
                }

                appointment.IsDeleted = true;
                dc.SaveChanges();

                return true;
            }
        }

        public IEnumerable<SBAppointmentDTO> GetSmartboardAppointmentsAssignedToUserId(string smartboardUserId, string apiKey)
        {
            using (var uow = UnitOfWorkFactory())
            {
                // we 1st get the person setting
                var person = uow
                    .Get<Person>()
                    .Include(x => x.AssignedAppointments)
                    .FirstOrDefault(x => !x.IsDeleted && x.SmartBoardID.Equals(smartboardUserId, StringComparison.InvariantCultureIgnoreCase));
                
                if (person == null)
                {
                    yield break;
                }

                // validate apiKey
                var userOus = _ouService.Value.ListAllForPerson(person.Guid);
                if(userOus?.Any() != true)
                {
                    yield break;
                }


                var userOUSettings = _ouSettingsService.Value.GetOuSettingsMany(userOus.Select(o => o.Guid))?.SelectMany(x => x.Value);
                if(userOUSettings?.Any(x => x.Name == SolarTrackerResources.IncomingApiKeySettingName && x.Value == apiKey) != true)
                {
                    yield break;
                }
                
                if(person?.AssignedAppointments?.Any(x => !x.IsDeleted) != true)
                {
                    yield break;
                }

                foreach(var app in person.AssignedAppointments.Where(x => !x.IsDeleted))
                {
                    yield return new SBAppointmentDTO(app);
                }

            }
            yield break;
        }

        public IEnumerable<SBAppointmentDTO> GetSmartboardAppointmentsForOUID(Guid ouID, int pageIndex, int itemsPerPage, string apiKey)
        {
            using (var uow = UnitOfWorkFactory())
            {
                // we validate the token
                var sbSettings = _ouSettingsService.Value.GetSettingsByOUID(ouID)?.FirstOrDefault(x => x.Name == SolarTrackerResources.SelectedSettingName)
                                    ?.GetValue<ICollection<SelectedIntegrationOption>>()?.FirstOrDefault(s => s.Data?.SMARTBoard != null)?.Data?.SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    yield break;
                }

                //get the properties
                var properties = _propertyService
                    .Value
                    .GetOUPropertiesPaged(ouID, null, pageIndex, itemsPerPage, "Appointments")
                    ?.Where(p => !p.IsDeleted);

                if (properties?.Any() != true)
                {
                    yield break;
                }

                foreach (var prop in properties)
                {
                    if(prop.Appointments?.Any(x => !x.IsDeleted) != true)
                    {
                        continue;
                    }

                    foreach(var app in prop.Appointments.Where(a => !a.IsDeleted))
                    {
                        yield return new SBAppointmentDTO(app);
                    }
                    
                }

            }
            yield break;
        }




        public ICollection<Person> GetMembersWithAppointment(OUMembersRequest request, Guid ouID, string apiKey ,DateTime date)
        {
            using (var uow = UnitOfWorkFactory())
            {
                // we validate the token
                var sbSettings = _ouSettingsService
                                    .Value
                                    .GetSettingsByOUID(ouID)
                                    ?.FirstOrDefault(x => x.Name == SolarTrackerResources.SelectedSettingName)
                                    ?.GetValue<ICollection<SelectedIntegrationOption>>()?
                                    .FirstOrDefault(s => s.Data?.SMARTBoard != null)?
                                    .Data?
                                    .SMARTBoard;

                if (sbSettings?.ApiKey != apiKey)
                {
                    throw new Exception("Please send Valid Apikey base on LeadId.");
                }
            }

            var Appointmentdata = _ouService.Value.GetMembers(request);
            return Appointmentdata;
        }

        public override ICollection<Appointment> InsertMany(ICollection<Appointment> entities)
        {
            var existIDs = Exist(entities);
            var newEntities = entities?
                            .Where(e => !existIDs.Contains(e.Guid))?
                            .ToList();

            VerifyUserAssignmentAndInvite(newEntities);
            return base.InsertMany(newEntities);
        }

        public override ICollection<Appointment> UpdateMany(ICollection<Appointment> entities)
        {
            VerifyUserAssignmentAndInvite(entities);
            return base.UpdateMany(entities);
        }

        public override Appointment Update(Appointment entity)
        {
            VerifyUserAssignmentAndInvite(entity);
            return base.Update(entity);
        }

        public override Appointment Insert(Appointment entity)
        {
            VerifyUserAssignmentAndInvite(entity);
            entity.CreatedByID = SmartPrincipal.UserId;
            var ret = base.Insert(entity);

            if (ret == null)
            {
                entity.SaveResult = SaveResult.SuccessfulInsert;
                return entity;
            }

            return ret;
        }

        public void VerifyUserAssignmentAndInvite(IEnumerable<Appointment> entities)
        {
            if (entities?.Any() != true)
            {
                return;
            }

            using (var uow = UnitOfWorkFactory())
            {
                var assigneeIds = entities.Select(x => x.AssigneeID);
                var assignedPeople =
                    uow
                    .Get<Person>()
                    ?.Include(p => p.Assignments)
                    ?.Where(p => assigneeIds.Contains(p.Guid))
                    ?.ToList();

                if (assignedPeople.Any() != true)
                {
                    throw new ArgumentException("Asignees not found", nameof(entities));
                }

                var propertyIds = entities.Select(x => x.PropertyID);
                var properties =
                    uow
                    .Get<Property>()
                    ?.Include(p => p.Territory)
                    ?.Where(p => propertyIds.Contains(p.Guid))
                    ?.ToList();

                if (properties.Any() != true)
                {
                    throw new ArgumentException("Properties not found", nameof(entities));
                }


                var ouInvitations = new List<UserInvitation>();
                var territoryAssignments = new List<Assignment>();
                foreach (var entity in entities)
                {
                    var assignee = assignedPeople.FirstOrDefault(p => p.Guid == entity.AssigneeID);
                    if (assignee == null)
                    {
                        throw new Exception("Assignee not found");
                    }

                    var property = properties.FirstOrDefault(p => p.Guid == entity.PropertyID);
                    if (property == null)
                    {
                        throw new Exception("Property not found.");
                    }

                    //check if the user is assigned to the territory
                    if (assignee.Assignments.Any(a => !a.IsDeleted && a.TerritoryID == property.TerritoryID) != true)
                    {
                        //check if the user is associated with the OU that contains the territory
                        var validations = _assignmentService.Value.ValidatePeopleOUs(new List<KeyValuePair<Guid, Guid>> { new KeyValuePair<Guid, Guid>(entity.AssigneeID.Value, property.Territory.OUID) });
                        if (validations.Any())
                        {
                            //user is not associated with the OU. send an invitation
                            ouInvitations.Add(new UserInvitation
                            {
                                FromPersonID = SmartPrincipal.UserId,
                                EmailAddress = assignee.EmailAddressString,
                                FirstName = assignee.FirstName,
                                LastName = assignee.LastName,
                                OUID = property.Territory.OUID,
                                RoleID = OURole.MemberRoleID,
                                CreatedByID = SmartPrincipal.UserId,
                            });

                        }
                        //assign user to the territory
                        territoryAssignments.Add(new Assignment
                        {
                            CreatedByID = SmartPrincipal.UserId,
                            PersonID = assignee.Guid,
                            TerritoryID = property.TerritoryID,
                            Status = AssignmentStatus.Open
                        });


                    }
                }
                if (ouInvitations.Any())
                {
                    _userInvitationService.Value.InsertMany(ouInvitations);
                }
                if (territoryAssignments.Any())
                {
                    _assignmentService.Value.InsertMany(territoryAssignments);
                }


            }
        }

        private void VerifyUserAssignmentAndInvite(Appointment entity)
        {
            if (entity == null)
            {
                return;
            }

            if (entity.AssigneeID == null)
            {
                throw new ArgumentNullException();
            }

            using (var ctx = UnitOfWorkFactory())
            {
                var assignee =
                    ctx
                    .Get<Person>()
                    .Include(p => p.Assignments)
                    .FirstOrDefault(p => p.Guid == entity.AssigneeID);

                if (assignee == null)
                {
                    throw new ArgumentException("Assignee not found");
                }

                var property =
                    ctx
                    .Get<Property>()
                    .Include(p => p.Territory)
                    .FirstOrDefault(p => p.Guid == entity.PropertyID);

                if (property == null)
                {
                    throw new ArgumentException("Property not found");
                }

                //check if the user is assigned to the territory
                if (assignee.Assignments.Any(a => !a.IsDeleted && a.TerritoryID == property.TerritoryID) != true)
                {
                    //check if the user is associated with the OU that contains the territory
                    var validations = _assignmentService.Value.ValidatePeopleOUs(new List<KeyValuePair<Guid, Guid>> { new KeyValuePair<Guid, Guid>(entity.AssigneeID.Value, property.Territory.OUID) });
                    if (validations.Any())
                    {
                        //user is not associated with the OU. send an invitation
                        var invite = new UserInvitation
                        {
                            FromPersonID = SmartPrincipal.UserId,
                            EmailAddress = assignee.EmailAddressString,
                            FirstName = assignee.FirstName,
                            LastName = assignee.LastName,
                            OUID = property.Territory.OUID,
                            RoleID = OURole.MemberRoleID,
                            CreatedByID = SmartPrincipal.UserId,
                        };
                        _userInvitationService.Value.Insert(invite);
                    }

                    //assign user to the territory
                    _assignmentService.Value.Insert(new Assignment
                    {
                        CreatedByID = SmartPrincipal.UserId,
                        PersonID = assignee.Guid,
                        TerritoryID = property.TerritoryID,
                        Status = AssignmentStatus.Open
                    });
                }
            }
        }
    }
}
