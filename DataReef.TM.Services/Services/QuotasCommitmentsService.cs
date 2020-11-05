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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DataReef.TM.Models.DTOs.Inquiries;

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
            entity.dispositions = JsonConvert.SerializeObject(entity.Disposition); 
            var ret = base.Insert(entity);

            if (ret == null)
            {
                entity.SaveResult = SaveResult.SuccessfulInsert;
                return entity;
            }

            return entity;
        }

        public List<List<object>> GetQuotasReport()
        {
            using (DataContext dc = new DataContext())
            {
                List<List<object>> report = new List<List<object>>();

                List<object> header = new List<object>();
                header.Add("Q1 " + DateTime.Now.Year);
                header.Add("Start");
                header.Add("End");
                header.Add("Duration(Days)");

                var dispositions = _personService.CRMGetAvailableDispositionsQuotas();
                dispositions = dispositions.OrderBy(a => a.Disposition).ToList();

                foreach (var item in dispositions)
                {
                    header.Add(item.DisplayName);
                }

                report.Add(header);

                var data = dc.QuotasCommitments.ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].durations = Convert.ToInt32(data[i].EndDate.Subtract(data[i].StartDate).TotalDays);
                    data[i].week = "Week" + i;

                    var quota = new List<object>{
                        data[i].week,
                        data[i].StartDate.ToShortDateString(),
                        data[i].EndDate.ToShortDateString(),
                        data[i].durations,
                    };

                    data[i].Disposition = JsonConvert.DeserializeObject<List<DataReef.TM.Models.DTOs.Inquiries.CRMDisposition>>(data[i].dispositions);
                    data[i].Disposition = data[i].Disposition.OrderBy(a => a.Disposition).ToList();

                    foreach (var item in data[i].Disposition)
                    {
                        quota.Add(item.Quota);
                    }
                     
                    report.Add(quota); 
                }

                return report;
            }
        }
         
        //public List<List<object>> GetQuotasCommitementsReport(QuotasCommitment req)
        //{
        //    using (DataContext dc = new DataContext())
        //    {  
        //        var dispositions = _personService.CRMGetAvailableDispositionsQuotas();
        //        dispositions = dispositions.OrderBy(a => a.Disposition).ToList();
                 
        //        var quota = dc.QuotasCommitments.FirstOrDefault(a => a.StartDate == req.StartDate && a.EndDate == req.EndDate);
        //        quota.Disposition = JsonConvert.DeserializeObject<List<CRMDisposition>>(quota.dispositions);
        //        quota.Disposition = quota.Disposition.OrderBy(a => a.Disposition).ToList();
       
        //        var commitment = dc.QuotasCommitments.FirstOrDefault(a => a.StartDate == req.StartDate && a.EndDate == req.EndDate && a.PersonID == req.PersonID);

        //        if (commitment != null)
        //        {
        //            commitment.Disposition = JsonConvert.DeserializeObject<List<CRMDisposition>>(commitment.dispositions);
        //            commitment.Disposition = commitment.Disposition.OrderBy(a => a.Disposition).ToList(); 
        //        } 

        //        foreach (var item in quota.Disposition)
        //        {
        //            item.TodayQuotas = (Convert.ToInt32(item.Quota) / Convert.ToInt32(quota.EndDate.Subtract(quota.StartDate).TotalDays)).ToString();
        //            item.WeekQuotas = item.Quota / ( item.TodayQuotas * 7 );
        //            item.RangeQuotas = item.Quota;
                      
        //            DateTime startDate = quota.StartDate;
        //            DateTime toDate = quota.EndDate;

        //            do
        //            {
        //                Console.WriteLine(quota.StartDate.ToString("dd/MM/yyyy") + "-" + startDate.AddDays(6).ToString("dd/MM/yyyy"));
        //                startDate = startDate.AddDays(7);
        //            }

        //            while (startDate < toDate.AddDays(7));

        //            if (commitment != null)
        //            {
        //                item.TodayCommitments = JsonConvert.DeserializeObject<List<CRMDisposition>>(commitment.dispositions);
        //                commitment.Disposition = commitment.Disposition.OrderBy(a => a.Disposition).ToList();
        //            }
        //        }

        //        return report;
        //    }
        }
    //}
}
