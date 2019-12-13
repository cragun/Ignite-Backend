using DataReef.Core.Infrastructure.Authorization;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace DataReef.TM.Services.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]

    public class AdministrationService : IAdministrationService
    {
        public ICollection<Guid> FindUser(string userNamePart)
        {
            using (var dataContext = new DataContext())
            {
                return dataContext.Users.Where(u => u.Credentials.Any(uc => uc.UserName.Contains(userNamePart))).Select(u => u.Guid).ToList();
            }
        }

        public ICollection<AuthenticationSummary> GetAuthenticationSummaries(DateTime? fromDate, DateTime? toDate)
        {
            using (var dataContext = new DataContext())
            {
                #region temporary, we'll move this into an user role after we refactor the user roles
                var isAllowedToViewReport = dataContext.PersonSettings.Any(ps => ps.PersonID == SmartPrincipal.UserId && ps.Name == "ViewAuthenticationsReport" && ps.Value == "true");
                if (!isAllowedToViewReport) throw new Exception("You are not allowed to view this report");
                #endregion temporary, we'll move this into an user role after we refactor the user roles

                var authenticationSummaries = dataContext.Database.SqlQuery<AuthenticationSummary>("exec prAuthenticationSummary @fromDate, @toDate", new SqlParameter("fromDate", fromDate), new SqlParameter("toDate", toDate)).ToList();
                return authenticationSummaries;
            }
        }
    }
}
