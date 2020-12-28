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
                    data[i].week = "Week " + i;

                    var quota = new List<object>{
                        data[i].week,
                        data[i].StartDate.ToShortDateString(),
                        data[i].EndDate.ToShortDateString(),
                        data[i].durations.ToString(),
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

        public List<List<object>> GetQuotasCommitementsReport(QuotasCommitment req)
        {
            using (DataContext dc = new DataContext())
            {
                var dispositions = _personService.CRMGetAvailableDispositionsQuotas();
                dispositions = dispositions.OrderBy(a => a.Disposition).ToList();

                var quota = dc.QuotasCommitments.FirstOrDefault(a => a.StartDate == req.StartDate && a.EndDate == req.EndDate);
                quota.Disposition = JsonConvert.DeserializeObject<List<CRMDisposition>>(quota.dispositions);
                quota.Disposition = quota.Disposition.OrderBy(a => a.Disposition).ToList();

                var commitment = dc.QuotasCommitments.FirstOrDefault(a => a.StartDate == req.StartDate && a.EndDate == req.EndDate && a.PersonID == req.UserID);

                if (commitment != null)
                {
                    commitment.Disposition = JsonConvert.DeserializeObject<List<CRMDisposition>>(commitment.dispositions);
                    commitment.Disposition = commitment.Disposition.OrderBy(a => a.Disposition).ToList();
                }

                List<List<object>> report = new List<List<object>>();

                List<object> header = new List<object>();
                header.Add("Metric");
                header.Add("Quota Today");
                header.Add("Commitment Today");
                header.Add("Quota This Week");
                header.Add("Commitment This Week");
                header.Add("Quota (" + req.StartDate.ToShortDateString() + " - " + req.EndDate.ToShortDateString() + ")");
                header.Add("Commitment (" + req.StartDate.ToShortDateString() + " - " + req.EndDate.ToShortDateString() + ")");

                report.Add(header);

                foreach (var item in quota.Disposition)
                {
                    item.TodayQuotas = (Convert.ToInt32(item.Quota) / Convert.ToInt32(quota.EndDate.Subtract(quota.StartDate).TotalDays)).ToString();
                    item.WeekQuotas = item.Quota;
                    item.RangeQuotas = item.Quota;

                    if (commitment != null)
                    {
                        var data = commitment.Disposition.FirstOrDefault(a => a.Disposition == item.Disposition);

                        item.TodayCommitments = data.TodayCommitments;
                        item.WeekCommitments = data.WeekCommitments;
                        item.RangeCommitments = data.RangeCommitments;
                    }
                    else
                    {
                        item.TodayCommitments = "";
                        item.WeekCommitments = "";
                        item.RangeCommitments = "";
                    }

                    var commitments = new List<object>{
                        item.DisplayName,
                        item.TodayQuotas,
                        item.TodayCommitments,
                        item.WeekQuotas,
                        item.WeekCommitments,
                        item.RangeQuotas,
                        item.RangeCommitments,
                    };

                    report.Add(commitments);
                }

                return report;
            }
        }

        public QuotasCommitment InsertCommitments(QuotasCommitment entity)
        {
            entity.CreatedByID = SmartPrincipal.UserId;

            List<CRMDisposition> dispositions = new List<CRMDisposition>();

            foreach (var item in entity.commitments)
            {
                var commitments = new CRMDisposition
                {
                    DisplayName = item[0].ToString(),
                    TodayQuotas = item[1].ToString(),
                    TodayCommitments = item[2].ToString(),
                    WeekQuotas = item[3].ToString(),
                    WeekCommitments = item[4].ToString(),
                    RangeQuotas = item[5].ToString(),
                    RangeCommitments = item[6].ToString(),
                };

                dispositions.Add(commitments);
            }
             
            entity.dispositions = JsonConvert.SerializeObject(dispositions);
            var ret = base.Insert(entity);

            if (ret == null)
            {
                entity.SaveResult = SaveResult.SuccessfulInsert;
                return entity;
            }

            return entity;
        }

        public List<List<object>> GetQuotasReportByPerson(QuotasCommitment req)
        {
            using (DataContext dc = new DataContext())
            { 
                List<List<object>> report = new List<List<object>>();

                var data = dc.QuotasCommitments.Where(a => a.RoleID == req.RoleID && a.PersonID == req.PersonID && a.Type == req.Type && a.StartDate >= req.StartDate && a.EndDate <= req.EndDate).ToList();

                if (data.Count > 0)
                { 
                    List<object> header = new List<object>();
                    header.Add("UserName");
                    header.Add("Position");
                    header.Add("Type");
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


                    for (int i = 0; i < data.Count; i++)
                    {
                        data[i].UserName = dc.People.FirstOrDefault(a => a.Guid == data[i].UserID)?.Name;
                        data[i].Position = dc.OURoles.FirstOrDefault(a => a.Guid == data[i].RoleID)?.Name;
                        data[i].Types = data[i].Type == 1 ? "Quotas" : "Commitments";

                        var quota = new List<object>{
                        data[i].UserName,
                        data[i].Position,
                        data[i].Types,
                        data[i].StartDate.ToShortDateString(),
                        data[i].EndDate.ToShortDateString()
                    };

                        data[i].Disposition = JsonConvert.DeserializeObject<List<DataReef.TM.Models.DTOs.Inquiries.CRMDisposition>>(data[i].dispositions);
                        data[i].Disposition = data[i].Disposition.OrderBy(a => a.Disposition).ToList();

                        foreach (var item in data[i].Disposition)
                        {
                            quota.Add(item.Quota);
                        }

                        report.Add(quota);
                    }
                } 

                return report;
            }
        }

    }
}
