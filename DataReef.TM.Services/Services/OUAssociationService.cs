using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using DataReef.Core.Infrastructure.Repository;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class OUAssociationService : DataService<OUAssociation>, IOUAssociationService
    {
        ILogger _logger;
        public OUAssociationService(ILogger logger, Func<IUnitOfWork> unitOfWorkFactory)
            : base(logger, unitOfWorkFactory)
        {
            _logger = logger;
        }

        public override ICollection<OUAssociation> List(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            var associations = base.List(deletedItems, pageNumber, itemsPerPage, filter, include, exclude, fields);

            return associations;
        }

        public async Task<ICollection<OUAssociation>> SmartList(bool deletedItems = false, int pageNumber = 1, int itemsPerPage = 20, string filter = "", string include = "", string exclude = "", string fields = "")
        {
            // Returned associations need to contain OU & OURole. We need to inject them in include if they are not present
            var includes = (include ?? string.Empty).Split(',', '|').ToList();

            if (!includes.Contains("OU"))
            {
                includes.Add("OU");
            }

            if (!includes.Contains("OURole"))
            {
                includes.Add("OURole");
            }

            var assocIncludes = includes.Where(i => i.IndexOf("Settings", StringComparison.InvariantCultureIgnoreCase) < 0);

            var assocInclude = string.Join(",", assocIncludes);

            var associations = List(deletedItems, pageNumber, itemsPerPage, filter, assocInclude, exclude, fields);

            if (associations != null)
            {
                associations = associations.Except(associations.Where(a => a.OU == null || a.OURole == null || a.OURole.IsActive == false || a.OU.IsArchived)).ToList();

                var associationsToExclude = new List<OUAssociation>();

                using (DataContext dc = new DataContext())
                {
                    foreach (var association in associations)
                    {
                        if (association.OU.IsRoot) continue; // the root associations will never be filtered

                        OU currentParentOU;
                        var currentParentID = association.OU.ParentID;

                        do
                        {
                            currentParentOU = await dc.OUs.AsNoTracking().SingleOrDefaultAsync(o => o.Guid == currentParentID);
                            if (currentParentOU == null) break;
                            if (currentParentOU.IsDeleted)
                            {
                                associationsToExclude.Add(association);
                                break;
                            }

                            var parentAssociation = associations.FirstOrDefault(a => a.OU.Guid == currentParentID);
                            if (parentAssociation != null && parentAssociation.OURole.IsGreaterThanOrEqual(association.OURole))
                            {
                                associationsToExclude.Add(association);
                                break;
                            }
                            currentParentID = currentParentOU.ParentID;
                        }
                        while (!currentParentOU.IsRoot);
                    }
                }

                associations = associations.Except(associationsToExclude).ToList();

                foreach (var association in associations)
                {
                    if (includes.Contains("OU.Settings", StringComparer.InvariantCultureIgnoreCase))
                    {

                        association.OU.Settings = await OUSettingService.GetOuSettings(association.OUID);
                        
                    }

                    if (association.OU.RootOrganization != null && includes.Contains("OU.RootOrganization.Settings", StringComparer.InvariantCultureIgnoreCase))
                    {
                        association.OU.RootOrganization.Settings = await OUSettingService.GetOuSettings(association.OU.RootOrganization.Guid);
                     }

                    if(association.OU.Settings != null)
                    {
                        using (DataContext dc = new DataContext())
                        {
                            var leftmenusetting = await dc.OUSettings.Where(x => x.Name == "Legion.LeftMenu.WebViewItems" && x.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();
                            association.OU.Settings.Add(leftmenusetting);
                        }

                    }
                    
                }
            }

            return associations;
        }

        public void SetPersonMayEdit(Person person, OUAssociation association)
        {
            if (person == null || association == null) return;

            if (person.MayEditSections == null) person.MayEditSections = new List<string>();
            if (association.OURole != null && (association.OURole.IsAdmin || association.OURole.IsOwner))
            {
                person.MayEditSections.Add("Prescreen");
            }
            if (association.OU != null && association.OU.RootOrganizationName.Equals("SunEdison", StringComparison.InvariantCultureIgnoreCase))
            {
                person.MayEditSections.Add("PPA");
            }
            person.MayEditSections = person.MayEditSections.Distinct().ToList();
        }

        // These method calls to bring as much data as possible in a single request will increase complexity and may cause performance issues
        // it would have been way better to have the app make a request for a person details when displaying its properties window and to have the server populate the MayEdit property only on that request, 
        // as that's the only place where the user may want to edit the properties and only for that specific person...
        public async void PopulatePersonMayEdit(ICollection<Person> people)
        {
            if (people == null || !people.Any()) return;
            var currentUsersAssociations = await SmartList(include: "OURole,OU,OU.RootOrganization", filter: String.Format("Personid={0}", SmartPrincipal.UserId));

            using (DataContext dc = new DataContext())
            {
                foreach (var currentUserAssociation in currentUsersAssociations)
                {
                    // get all sub-OUs for the current OU association
                    var currentOUSubOrganizationIds = dc
                            .Database
                            .SqlQuery<OU>("exec [proc_SelectOUHierarchy] {0}", currentUserAssociation.OUID)
                            .Where(o => !o.IsDeleted)
                            .Select(o => o.Guid)
                            .ToList();

                    foreach (var person in people)
                    {
                        if (dc.People.Any(p => p.Guid == person.Guid && p.OUAssociations.Any(poua => currentOUSubOrganizationIds.Contains(poua.OUID))))
                        {
                            SetPersonMayEdit(person, currentUserAssociation);
                        }
                    }
                }
            }
        }
    }
}
