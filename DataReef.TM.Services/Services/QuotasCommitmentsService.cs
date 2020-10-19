using DataReef.Core.Logging;
using DataReef.TM.Contracts.Services;
using DataReef.TM.DataAccess.Database;
using DataReef.TM.Models;
using EntityFramework.Extensions;
using System;
using System.Linq;
using DataReef.Core.Infrastructure.Repository;
using DataReef.Core.Infrastructure.Authorization;
using DataReef.Core.Classes;
using System.Collections.Generic;
using DataReef.TM.Models.DTOs.Signatures;
using DataReef.TM.Models.DTOs.QuotasCommitments;
using DataReef.TM.Models.DataViews;

namespace DataReef.TM.Services
{
    public class QuotasCommitmentsService : DataService<QuotasCommitment>, IQuotasCommitmentsService
    {
        private readonly IOUService _ouService;
        private readonly IPersonService _personService;
        private readonly IUserInvitationService _userInvitationService;

        public QuotasCommitmentsService(ILogger logger,
             IOUService ouService,
             IPersonService personService,
             IUserInvitationService userInvitationService,
            Func<IUnitOfWork> unitOfWorkFactory) : base(logger, unitOfWorkFactory)
        {
            _ouService = ouService;
            _personService = personService;
            _userInvitationService = userInvitationService;
        }

        public AdminQuotas GetQuotasType()
        {
            List<Models.DTOs.QuotasCommitments.Type> typeList = new List<Models.DTOs.QuotasCommitments.Type>();

            typeList.Add(new Models.DTOs.QuotasCommitments.Type() { Id = 1, Name = "Quotas" });
            typeList.Add(new Models.DTOs.QuotasCommitments.Type() { Id = 2, Name = "Commitments" });

            var ouRoles = _ouService.SBGetOuRoles();
            var dispositions = _personService.CRMGetAvailableDispositionsQuotas();

            var response = new AdminQuotas
            {
                type = typeList,
                user_type = ouRoles,
                dispositions = dispositions
            };

            return response;
        }

        public IEnumerable<UserInvitation> GetUsersFromRoleType(Guid roleid)
        {
            using (DataContext dc = new DataContext())
            {
                var users = dc.UserInvitations.Where(a => a.RoleID == roleid).ToList();
                return users;
            }
        }

        public QuotasCommitment InsertQuotas(QuotasCommitment entity)
        {
            entity.CreatedByID = SmartPrincipal.UserId;
            var ret = base.Insert(entity);

            if (ret == null)
            {
                entity.SaveResult = SaveResult.SuccessfulInsert;
                return entity;
            }

            return entity;
        }


        public List<QuotasCommitment> GetQuotasReport()
        {
            using (DataContext dc = new DataContext())
            {
                var data = dc.QuotasCommitments.ToList();





                return data;
            }
        }
    }
}
