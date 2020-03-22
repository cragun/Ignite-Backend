using DataReef.Core.Classes;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using DataReef.Core.Infrastructure.Repository;
using System.Configuration;

namespace DataReef.TM.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class AssignmentService : DataService<Assignment>, IAssignmentService
    {
        //private string senderEmailAddress = ConfigurationManager.AppSettings["SenderEmail"] ?? "noreply@datareef.com";
        private string senderEmailAddress = ConfigurationManager.AppSettings["SenderEmail"] ?? "support@smartboardcrm.com";
        
        private readonly ILogger _logger;

        public AssignmentService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _logger = logger;
        }

        public override Assignment Insert(Assignment ass)
        {
            bool IsFromSmartBoard = false;
            if (ass.Notes == "FromSmartBoard")
            {
                IsFromSmartBoard = true;
                ass.Notes = null;
            }
            Assignment existingAssignment;
            // I saw lots of duplicate assignments, and I added a validation
            using (var dataContext = new DataContext())
            {
                existingAssignment = dataContext
                                        .Assignments
                                        .FirstOrDefault(a => a.PersonID == ass.PersonID
                                                          && a.TerritoryID == ass.TerritoryID
                                                          && a.Status == AssignmentStatus.Open);
                if (existingAssignment != null)
                {
                    existingAssignment.SaveResult = SaveResult.SuccessfulInsert;
                    if (ass.Territory != null)
                    {
                        // keep territory if already set
                        existingAssignment.Territory = ass.Territory;
                    }
                }
            }

            Assignment ret = existingAssignment ?? base.Insert(ass);
            
            if (IsFromSmartBoard == false)
            {
                this.SendTerritoryAssignmentNotification(new Assignment[] { ret });
            }


            return ret;
        }

        public override System.Collections.Generic.ICollection<Assignment> InsertMany(System.Collections.Generic.ICollection<Assignment> entities)
        {
            var insertedEntities = base.InsertMany(entities);

            this.SendTerritoryAssignmentNotification(insertedEntities);

            return insertedEntities;
        }

        public Task SendTerritoryAssignmentNotification(IEnumerable<Assignment> assignments)
        {
            return Task.Run(() =>
            {
                if ((assignments == null) || (assignments.Count() == 0))
                {
                    return;
                }

                using (var dataContext = new DataContext())
                {

                    foreach (var assignmentItem in assignments)
                    {
                        bool userIsSelfAssigned = assignmentItem.CreatedByID.HasValue && (assignmentItem.CreatedByID.Value == assignmentItem.PersonID);
                        if (!userIsSelfAssigned)
                        {
                            Territory territory = assignmentItem.Territory;
                            if ((territory == null) && (assignmentItem.TerritoryID != Guid.Empty))
                            {
                                territory = dataContext.Territories.FirstOrDefault(t => t.Guid.Equals(assignmentItem.TerritoryID));
                            }

                            Person person = assignmentItem.Person;
                            if ((person == null) && (assignmentItem.PersonID != Guid.Empty))
                            {
                                person = dataContext.People.FirstOrDefault(p => p.Guid.Equals(assignmentItem.PersonID));
                            }

                            if ((territory != null) && (person != null))
                            {
                                string emailAddress = person.EmailAddressString;

                                Mail.Library.SendTerritoryAssignmentNotification(
                                    template: new Classes.TerritoryAssignmentNotificationTemplate()
                                    {
                                        TerritoryName = territory.Name,
                                        PropertyCount = territory.Summary != null ? territory.Summary.PropertyCount : 0,
                                        TerritoryLink = string.Format("datareef-tm://territory?guid={0}", territory.Guid)
                                    },
                                    from: senderEmailAddress,
                                    fromName: Mail.Library.SenderName,
                                    subject: "New Territory Assigned",
                                    toPersonEmail: emailAddress);
                            }
                        }
                    }
                }
            });
        }

        public List<KeyValuePair<Guid, string>> ValidatePeopleOUs(List<KeyValuePair<Guid, Guid>> data)
        {
            var response = new List<KeyValuePair<Guid, string>>();
            using (var dataContext = new DataContext())
            {
                foreach (var item in data)
                {
                    var personId = item.Key;
                    var ouid = item.Value;

                    // Validate that the User to get assigned is associated to the Territory's OU.
                    // Get all Hierarchical OUs for Person
                    var personOUs = dataContext
                            .Database
                            .SqlQuery<OU>("exec proc_OUsForPerson {0}", personId)
                            .Select(o => o.Guid)
                            .ToList();

                    var ou = dataContext
                            .OUs
                            .FirstOrDefault(o => o.Guid == ouid);

                    // Get OU Hierarchy for Territory.OUID
                    var ouHierarchy = dataContext
                                        .Database
                                        .SqlQuery<Guid>("select Guid from dbo.OUTreeUp({0})", ouid)
                                        .ToList();

                    // if the OU is top level, it will not be in the list, that's why we add it below
                    ouHierarchy.Add(ouid);

                    if (!personOUs.Intersect(ouHierarchy).Any())
                    {
                        var person = dataContext
                                        .People
                                        .FirstOrDefault(p => p.Guid == personId);

                        var msg = "{FirstName} {LastName} cannot be assigned to the territory because (s)he does not have access to {ParentOUName}. Please invite him/her to {ParentOUName} and then, after accepting the invitation, assign him/her to the territory.";
                        msg = msg.FormatWith(new { FirstName = person.FirstName, LastName = person.LastName, ParentOUName = ou.Name });
                        response.Add(new KeyValuePair<Guid, string>(personId, msg));
                    }
                }
            }

            return response;
        }
    }
}
