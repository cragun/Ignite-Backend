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

            return new AdminQuotas
            {
                type = typeList,
                user_type = ouRoles,
                dispositions = dispositions
            };
        }

        public IEnumerable<GuidNamePair> GetUsersFromRoleType(Guid roleid)
        {
            using (DataContext dc = new DataContext())
            {
                var peopleIds = dc.OUAssociations.Where(x => x.IsDeleted == false && x.OURoleID == roleid).Select(y => y.PersonID);
                var peoples = dc.People.Where(x => peopleIds.Contains(x.Guid) && x.IsDeleted == false).ToList();
                return peoples.Select(a => new GuidNamePair
                {
                    Name = $"{a.FirstName} {a.LastName}",
                    Guid = a.Guid,
                });
            }
        }

        public QuotasCommitment InsertQuotas(QuotasCommitment entity)
        {
            entity.CreatedByID = SmartPrincipal.UserId;
            entity.Flags = 1;
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
                header.Add($"Q1 {DateTime.Now.Year}");
                header.Add("Start");
                header.Add("End");
                header.Add("Duration(Days)");
                header.Add("Type");

                var dispositions = _personService.CRMGetAvailableDispositionsQuotas();
                dispositions = dispositions.OrderBy(a => a.Disposition).ToList();

                foreach (var item in dispositions)
                {
                    header.Add(item.Disposition);
                }

                report.Add(header);

                var data = dc.QuotasCommitments.Where(a => a.Flags == 1).ToList();

                for (int i = 0; i < data.Count; i++)
                {
                    data[i].durations = Convert.ToInt32(data[i].EndDate.Subtract(data[i].StartDate).TotalDays);
                    data[i].week = $"Week {i}";
                    data[i].Types = data[i].Type == 1 ? "Quota" : "Commitment";

                    var quota = new List<object>{
                        data[i].week,
                        data[i].StartDate.ToShortDateString(),
                        data[i].EndDate.ToShortDateString(),
                        data[i].durations.ToString(),
                        data[i].Type
                    };

                    data[i].Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(data[i].dispositions);
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

                var quota = dc.QuotasCommitments.FirstOrDefault(a => a.Flags == 1 && a.Type == 1 && a.StartDate == req.StartDate && a.EndDate == req.EndDate);

                List<List<object>> report = new List<List<object>>();

                if (quota != null)
                {
                    quota.Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(quota.dispositions);
                    quota.Disposition = quota.Disposition.OrderBy(a => a.Disposition).ToList();

                    List<object> header = new List<object>();
                    header.Add("Metric");
                    header.Add("Quota Today");
                    header.Add("Commitment Today");
                    header.Add("Quota This Week");
                    header.Add("Commitment This Week");
                    header.Add($"Quota ({req.StartDate.ToShortDateString()} - {req.EndDate.ToShortDateString()})");
                    header.Add($"Commitment ({req.StartDate.ToShortDateString()} - {req.EndDate.ToShortDateString()})");

                    report.Add(header);

                    var isUserSetCommitment = dc.QuotasCommitments.FirstOrDefault(a => a.Flags == 2 && a.Type == 2 && a.StartDate == req.StartDate && a.EndDate == req.EndDate && a.PersonID == req.PersonID);

                    var isAdminSetCommitment = dc.QuotasCommitments.FirstOrDefault(a => a.Flags == 1 && a.Type == 2 && a.StartDate == req.StartDate && a.EndDate == req.EndDate);

                    if (isUserSetCommitment != null)
                    {
                        isUserSetCommitment.Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(isUserSetCommitment.dispositions);
                        isUserSetCommitment.Disposition = isUserSetCommitment.Disposition.OrderBy(a => a.Disposition).ToList();
                    }
                    else
                    {
                        if (isAdminSetCommitment != null)
                        {
                            isAdminSetCommitment.Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(isAdminSetCommitment.dispositions);
                            isAdminSetCommitment.Disposition = isAdminSetCommitment.Disposition.OrderBy(a => a.Disposition).ToList();
                        }
                    }

                    var commitment = isUserSetCommitment != null ? isUserSetCommitment : (isAdminSetCommitment != null ? isAdminSetCommitment : null);

                    foreach (var item in quota.Disposition)
                    {
                        if (string.IsNullOrEmpty(item.Quota))
                        {
                            item.Quota = "0";
                        }
                        item.TodayQuotas = (Convert.ToInt32(item.Quota) / Convert.ToInt32(quota.EndDate.Subtract(quota.StartDate).TotalDays)).ToString();
                        item.WeekQuotas = item.Quota;
                        item.RangeQuotas = item.Quota;

                        if (isAdminSetCommitment != null && isUserSetCommitment == null)
                        {
                            var adminCommitment = isAdminSetCommitment.Disposition.FirstOrDefault(a => a.Disposition == item.Disposition);

                            item.TodayCommitments = (Convert.ToInt32(adminCommitment.Commitments) / Convert.ToInt32(commitment.EndDate.Subtract(commitment.StartDate).TotalDays)).ToString();

                            item.WeekCommitments = adminCommitment.Commitments;
                            item.RangeCommitments = adminCommitment.Commitments;
                        }
                        else if (isUserSetCommitment != null)
                        {
                            var userCommitment = isUserSetCommitment.Disposition.FirstOrDefault(a => a.Disposition == item.Disposition);

                            item.TodayCommitments = (Convert.ToInt32(userCommitment.Commitments) / Convert.ToInt32(commitment.EndDate.Subtract(commitment.StartDate).TotalDays)).ToString();

                            item.WeekCommitments = userCommitment.Commitments;
                            item.RangeCommitments = userCommitment.Commitments;
                        }
                        else
                        {
                            item.TodayCommitments = "0";
                            item.WeekCommitments = "0";
                            item.RangeCommitments = "0";
                        }

                        var commitments = new List<object>{
                        item.Disposition,
                        item.TodayQuotas,
                        item.TodayCommitments,
                        item.WeekQuotas,
                        item.WeekCommitments,
                        item.RangeQuotas,
                        item.RangeCommitments,
                    };

                        report.Add(commitments);
                    }
                }

                return report;
            }
        }

        public QuotasCommitment InsertCommitments(QuotasCommitment entity)
        {
            entity.CreatedByID = SmartPrincipal.UserId;

            List<QuotaCommitementsDisposition> dispositions = new List<QuotaCommitementsDisposition>();

            foreach (var item in entity.commitments)
            {
                dispositions.Add(new QuotaCommitementsDisposition
                {
                    DisplayName = item[0]?.ToString(),
                    Disposition = item[0]?.ToString(),
                    TodayQuotas = item[1]?.ToString(),
                    TodayCommitments = item[2]?.ToString(),
                    WeekQuotas = item[3]?.ToString(),
                    WeekCommitments = item[4]?.ToString(),
                    RangeQuotas = item[5]?.ToString(),
                    RangeCommitments = item[6]?.ToString(),
                });
            }

            entity.dispositions = JsonConvert.SerializeObject(dispositions);
            entity.Flags = 2;
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

                var data = dc.QuotasCommitments.Where(a => (a.RoleID == req.RoleID && a.PersonID == req.PersonID && a.Type == req.Type)
                && a.StartDate >= req.StartDate && a.EndDate <= req.EndDate).ToList();

                if (data.Count > 0)
                {
                    List<object> header = new List<object>() { "UserName", "Position", "Type", "Start", "End" };

                    var dispositions = _personService.CRMGetAvailableDispositionsQuotas();
                    dispositions = dispositions.OrderBy(a => a.Disposition).ToList();

                    foreach (var item in dispositions)
                    {
                        header.Add(item.Disposition);
                    }

                    report.Add(header);

                    for (int i = 0; i < data.Count; i++)
                    {
                        var PersonID = data[i].PersonID;
                        var RoleID = data[i].RoleID;

                        data[i].UserName = dc.People.FirstOrDefault(a => a.Guid == PersonID)?.Name;
                        data[i].Position = dc.OURoles.FirstOrDefault(a => a.Guid == RoleID)?.Name;
                        data[i].Types = data[i].Type == 1 ? "Quotas" : "Commitments";

                        var quota = new List<object>{
                        data[i].UserName,
                        data[i].Position,
                        data[i].Types,
                        data[i].StartDate.ToShortDateString(),
                        data[i].EndDate.ToShortDateString(),
                        };

                        data[i].Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(data[i].dispositions);
                        data[i].Disposition = data[i].Disposition.OrderBy(a => a.Disposition).ToList();

                        foreach (var item in data[i].Disposition)
                        {
                            if (data[i].Type == 1)
                            {
                                quota.Add(item.Quota);
                            }
                            else
                            {
                                quota.Add(item.Commitments);
                            }
                        }

                        report.Add(quota);
                    }
                }

                return report;
            }
        }

        public List<List<object>> GetQuotasDateRange(QuotasCommitment req)
        {
            try
            {
                using (DataContext dc = new DataContext())
                {
                    req.EndDate = req.CurrentDate.AddMonths(3);
                    List<List<object>> reportSet = new List<List<object>>();

                    var data = dc.QuotasCommitments.Where(a => a.StartDate >= req.CurrentDate && a.EndDate <= req.EndDate && a.PersonID == req.PersonID).ToList();
                    List<QuotaCommitementsDisposition> allDispositions = new List<QuotaCommitementsDisposition>();

                    var quotas = data.Where(a => a.Flags == 1 && a.Type == 1).ToList();
                    foreach (var item in quotas)
                    {
                        if (item != null)
                        {
                            item.Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(item.dispositions);
                            item.Disposition = item.Disposition.OrderBy(a => a.Disposition).ToList();

                            var isUserSetCommitment = data.FirstOrDefault(a => a.Flags == 2 && a.Type == 2);
                            var isAdminSetCommitment = data.FirstOrDefault(a => a.Flags == 1 && a.Type == 2);

                            if (isUserSetCommitment != null)
                            {
                                isUserSetCommitment.Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(isUserSetCommitment.dispositions);
                            }
                            else
                            {
                                if (isAdminSetCommitment != null)
                                {
                                    isAdminSetCommitment.Disposition = JsonConvert.DeserializeObject<List<QuotaCommitementsDisposition>>(isAdminSetCommitment.dispositions);
                                }
                            }

                            var commitment = isUserSetCommitment != null ? isUserSetCommitment : (isAdminSetCommitment != null ? isAdminSetCommitment : null);

                            foreach (var disposition in item.Disposition)
                            {

                                disposition.TodayQuotas = (Convert.ToInt32(disposition.Quota) / Convert.ToInt32(item.EndDate.Subtract(item.StartDate).TotalDays)).ToString();
                                disposition.WeekQuotas = disposition.Quota;
                                disposition.RangeQuotas = disposition.Quota;

                                if (isAdminSetCommitment != null && isUserSetCommitment == null)
                                {
                                    var adminCommitment = isAdminSetCommitment.Disposition.FirstOrDefault(a => a.Disposition == disposition.Disposition);

                                    disposition.TodayCommitments = (Convert.ToInt32(adminCommitment.Commitments) / Convert.ToInt32(commitment.EndDate.Subtract(commitment.StartDate).TotalDays)).ToString();

                                    disposition.WeekCommitments = adminCommitment.Commitments;
                                    disposition.RangeCommitments = adminCommitment.Commitments;
                                }
                                else if (isUserSetCommitment != null)
                                {
                                    var userCommitment = isUserSetCommitment.Disposition.FirstOrDefault(a => a.Disposition == disposition.Disposition);

                                    disposition.TodayCommitments = (Convert.ToInt32(userCommitment.Commitments) / Convert.ToInt32(commitment.EndDate.Subtract(commitment.StartDate).TotalDays)).ToString();

                                    disposition.WeekCommitments = userCommitment.Commitments;
                                    disposition.RangeCommitments = userCommitment.Commitments;
                                }
                                else
                                {
                                    disposition.TodayCommitments = "0";
                                    disposition.WeekCommitments = "0";
                                    disposition.RangeCommitments = "0";
                                }

                                allDispositions.Add(disposition);
                            }

                        }
                    }

                    var dispositions = _personService.CRMGetAvailableDispositionsQuotas();

                    List<object> header = new List<object>() { "Metric" , "Quota Today" , "Commitment Today" , "Quota This Week"  , "Commitment This Week" , "Quota (" + req.CurrentDate.ToShortDateString() + " - " + req.EndDate.ToShortDateString() + ")" , "Commitment (" + req.CurrentDate.ToShortDateString() + " - " + req.EndDate.ToShortDateString() + ")" }; 

                    reportSet.Add(header);

                    foreach (var item in dispositions)
                    {
                        var itemDis = allDispositions.Where(a => a.Disposition == item.Disposition).ToList().GroupBy(r => r.Disposition)
                                                        .Select(group => new QuotaCommitementsDisposition
                                                        {
                                                            Disposition = group.Key,
                                                            TodayQuotas = group.Sum(rp => Convert.ToInt32(rp.TodayQuotas)).ToString(),
                                                            WeekQuotas = group.Sum(rp => Convert.ToInt32(rp.WeekQuotas)).ToString(),
                                                            RangeQuotas = group.Sum(rp => Convert.ToInt32(rp.RangeQuotas)).ToString(),
                                                            TodayCommitments = group.Sum(rp => Convert.ToInt32(rp.TodayCommitments)).ToString(),
                                                            WeekCommitments = group.Sum(rp => Convert.ToInt32(rp.WeekCommitments)).ToString(),
                                                            RangeCommitments = group.Sum(rp => Convert.ToInt32(rp.RangeCommitments)).ToString()
                                                        }).FirstOrDefault();

                        reportSet.Add(new List<object>{
                        item.Disposition,
                        itemDis?.TodayQuotas != null ? itemDis?.TodayQuotas : "0",
                        itemDis?.WeekQuotas  != null ? itemDis?.WeekQuotas : "0",
                        itemDis?.RangeQuotas  != null ? itemDis?.RangeQuotas : "0",
                        itemDis?.TodayCommitments  != null ? itemDis?.TodayCommitments : "0",
                        itemDis?.WeekCommitments  != null ? itemDis?.WeekCommitments : "0",
                        itemDis?.RangeCommitments  != null ? itemDis?.RangeCommitments : "0"
                    });
                    }

                    return reportSet;
                }
            }
            catch (Exception ex)
            {
                return new List<List<object>>();
            }
        }

        public bool IsCommitmentsSetByUser(QuotasCommitment req)
        {
            using (DataContext dc = new DataContext())
            {
                bool isSet = false;
                var data = dc.QuotasCommitments.Where(a => a.StartDate >= req.CurrentDate && a.PersonID == req.PersonID).ToList();
                if (data.Count > 0)
                {
                    isSet = true;
                }
                return isSet;
            }
        }
    }
}
